using FinanceApp.API.Bootstrap.Constans;

namespace FinanceApp.API.Bootstrap.DependencyInjection;

public static class CorsServiceCollectionExtensions
{
    public static IServiceCollection AddAppCors(this IServiceCollection services, IConfiguration configuration)
    {
        var frontendOrigins = configuration.GetSection("Cors:FrontendOrigins").Get<string[]>() ?? [];
        var devOrigins = configuration.GetSection("Cors:DevOrigins").Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy(AppCorsPolicies.Frontend, policy =>
            {
                policy.WithOrigins(frontendOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });

            options.AddPolicy(AppCorsPolicies.Development, policy =>
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
