

using FinanceApp.API.Services;

namespace FinanceApp.API.Bootstrap.DependencyInjection;

public static class CoreServicesCollectionExtensions
{
    public static IServiceCollection AddAppCoreServices(this IServiceCollection services)
    {
        services.AddScoped<PasswordService>();
        services.AddScoped<EmailService>();
        services.AddScoped<JwtService>();
        services.AddSingleton<AbuseProtectionService>();

        return services;
    }
}
