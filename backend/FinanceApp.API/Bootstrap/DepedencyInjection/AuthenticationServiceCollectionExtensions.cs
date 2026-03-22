// Bootstrap/DependencyInjection/AuthenticationServiceCollectionExtensions.cs
using System.Security.Claims;
using System.Text;
using FinanceApp.API.Bootstrap.Constans;
using FinanceApp.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace FinanceApp.API.Bootstrap.DependencyInjection;

public static class AuthenticationServiceCollectionExtensions
{
    public static IServiceCollection AddAppAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing.");
                var jwtIssuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is missing.");
                var jwtAudience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is missing.");

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
                    OnMessageReceived = context =>
                    {
                        if (string.IsNullOrWhiteSpace(context.Token)
                            && context.Request.Cookies.TryGetValue("accessToken", out var accessToken))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },
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
                    OnTokenValidated = async context =>
                    {
                        var db = context.HttpContext.RequestServices.GetRequiredService<FinanceDbContext>();
                        var sub = context.Principal?.FindFirstValue(AppClaims.JwtSubject)
                            ?? context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                        var tvClaim = context.Principal?.FindFirstValue("tv");

                        if (!Guid.TryParse(sub, out var userId) || !int.TryParse(tvClaim, out var tv))
                        {
                            context.Fail("Invalid token claims");
                            return;
                        }

                        var user = await db.Users.FindAsync(userId);
                        if (user is null || user.TokenVersion != tv)
                        {
                            context.Fail("Token revoked");
                        }
                    }
                };
            });

        services.AddAuthorization();
        return services;
    }
}
