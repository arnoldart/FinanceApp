namespace FinanceApp.API.Bootstrap.Endpoints;

public static class EndpointRouteBuilderExtensions
{
    public static WebApplication MapAppEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => Results.Redirect("/swagger"));
        app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
        app.MapControllers();

        return app;
    }
}
