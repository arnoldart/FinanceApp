namespace FinanceApp.API.Extensions;

public static class CorsPolicies
{
    public const string Frontend = "FrontendPolicy";
    public const string Development = "DevPolicy";
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppLogging(this IServiceCollection services, ILoggingBuilder logging)
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddDebug();

        return services;
    }

    public static IServiceCollection AddAppCors(this IServiceCollection services, IConfiguration configuration)
    {
        var frontendOrigins = configuration.GetSection("Cors:FrontendOrigins").Get<string[]>() ?? [];
        var devOrigins = configuration.GetSection("Cors:DevOrigins").Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicies.Frontend, policy =>
            {
                policy.WithOrigins(frontendOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });

            options.AddPolicy(CorsPolicies.Development, policy =>
            {
                policy.WithOrigins(devOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }
}