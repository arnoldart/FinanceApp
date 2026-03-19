using FinanceApp.API.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.API.Bootstrap.DependencyInjection;

public static class DatabaseServiceCollection
{
    public static IServiceCollection AddAppDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var defaultConnection = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is missing.");

        services.AddDbContext<FinanceDbContext>(options =>
            options.UseNpgsql(defaultConnection));

        return services;
    }
}