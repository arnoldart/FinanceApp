using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using DotNetEnv;
using FinanceApp.API.Data;
using FinanceApp.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var dotEnvPath = Path.Combine(builder.Environment.ContentRootPath, ".env");
if (File.Exists(dotEnvPath))
{
    Env.Load(dotEnvPath);
    builder.Configuration.AddEnvironmentVariables();
}

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var frontendOrigins = builder.Configuration.GetSection("Cors:FrontendOrigins").Get<string[]>() ?? [];
var devOrigins = builder.Configuration.GetSection("Cors:DevOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(frontendOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });

    options.AddPolicy("DevPolicy", policy =>
    {
        policy.WithOrigins(devOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is missing.");

builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseNpgsql(defaultConnection));
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing.");
    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is missing.");
    var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is missing.");

    var key = Encoding.UTF8.GetBytes(jwtKey);

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,

        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,

        IssuerSigningKey = new SymmetricSecurityKey(key)
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"message\":\"Akses ditolak. Silakan login terlebih dahulu.\"}");
        },
        OnForbidden = async context =>
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"message\":\"Kamu tidak punya izin untuk mengakses resource ini.\"}");
        },
        OnTokenValidated = async ctx =>
        {
            var db = ctx.HttpContext.RequestServices.GetRequiredService<FinanceDbContext>();
            var sub = ctx.Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? ctx.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            var tvClaim = ctx.Principal?.FindFirstValue("tv");

            if(!Guid.TryParse(sub, out var userId) || !int.TryParse(tvClaim, out var tv))
            {
                ctx.Fail("Invalid token claims");
                return;
            }

            var user = await db.Users.FindAsync(userId);
            if(user is null || user.TokenVersion != tv)
            {
                ctx.Fail("Token revoked");
            }
        }
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRateLimiter(RateLimiterOptions =>
{
    RateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    RateLimiterOptions.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsync(
            "{\"success\":false,\"message\":\"Terlalu banyak request. Coba lagi sebentar.\"}",
            cancellationToken);
    };

    RateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.Window = TimeSpan.FromSeconds(10);
        options.PermitLimit = 3;
        options.QueueLimit = 0;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

var app = builder.Build();

var corsPolicy = app.Environment.IsDevelopment() ? "DevPolicy" : "FrontendPolicy";
app.UseCors(corsPolicy);
app.UseSwagger();
app.UseSwaggerUI();
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapControllers();

app.Run();
