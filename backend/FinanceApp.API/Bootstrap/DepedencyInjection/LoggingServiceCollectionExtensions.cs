namespace FinanceApp.API.Bootstrap.DependencyInjection;

public static class LoggingServiceCollectionExtensions
{
    public static IServiceCollection AddAppLogging(this IServiceCollection services, ILoggingBuilder logging)
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.AddDebug();

        return services; 
    }
}