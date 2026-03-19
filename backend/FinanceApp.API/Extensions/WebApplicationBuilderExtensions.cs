using DotNetEnv;

namespace FinanceApp.API.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddEnvironmentConfiguration(this WebApplicationBuilder builder)
    {
        var dotEnvPath = Path.Combine(builder.Environment.ContentRootPath, ".env");
        if (File.Exists(dotEnvPath))
        {
            Env.Load(dotEnvPath);
            builder.Configuration.AddEnvironmentVariables();
        }

        return builder;
    }
}
