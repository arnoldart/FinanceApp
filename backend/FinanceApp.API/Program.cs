using FinanceApp.API.Bootstrap.DependencyInjection;
using FinanceApp.API.Bootstrap.Endpoints;
using FinanceApp.API.Bootstrap.Pipeline;
using FinanceApp.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddEnvironmentConfiguration();
builder.Services
    .AddAppLogging(builder.Logging)
    .AddAppCors(builder.Configuration)
    .AddAppDatabase(builder.Configuration)
    .AddAppCoreServices()
    .AddAppAuthentication(builder.Configuration)
    .AddAppRateLimiting()
    .AddAppApi();

var app = builder.Build();

app.UseAppPipeline();
app.MapAppEndpoints();

app.Run();
