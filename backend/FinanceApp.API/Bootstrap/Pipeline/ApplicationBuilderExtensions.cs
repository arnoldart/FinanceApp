using FinanceApp.API.Bootstrap.Constans;

namespace FinanceApp.API.Bootstrap.Pipeline;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseAppPipeline(this WebApplication app)
    {
        var corsPolicy = app.Environment.IsDevelopment()
            ? AppCorsPolicies.Development
            : AppCorsPolicies.Frontend;

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

        return app;
    }
}
