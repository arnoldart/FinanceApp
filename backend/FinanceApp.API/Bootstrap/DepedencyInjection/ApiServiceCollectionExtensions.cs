namespace FinanceApp.API.Bootstrap.DependencyInjection;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddAppApi(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}
