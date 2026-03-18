namespace FinanceApp.API.Extensions;

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

        return services;
    }
}
