// Bootstrap/DependencyInjection/RateLimitingServiceCollectionExtensions.cs
using System.Security.Claims;
using System.Threading.RateLimiting;
using FinanceApp.API.Bootstrap.Constans;
using FinanceApp.API.Services;
using Microsoft.AspNetCore.RateLimiting;

namespace FinanceApp.API.Bootstrap.DependencyInjection;

public static class RateLimitingServiceCollectionExtensions
{
    public static IServiceCollection AddAppRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, cancellationToken) =>
            {
                var abuseProtection = context.HttpContext.RequestServices.GetRequiredService<AbuseProtectionService>();

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter = Math.Ceiling(retryAfter.TotalSeconds).ToString();
                }

                abuseProtection.LogMiddlewareRejection(
                    context.HttpContext,
                    context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfterValue) ? retryAfterValue : null);

                context.HttpContext.Response.ContentType = "application/json";
                await context.HttpContext.Response.WriteAsync(
                    "{\"success\":false,\"message\":\"Terlalu banyak request. Coba lagi sebentar.\"}",
                    cancellationToken);
            };

            options.AddPolicy(AppRateLimitPolicies.AuthAnon, httpContext =>
            {
                var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetSlidingWindowLimiter(partitionKey, _ => new SlidingWindowRateLimiterOptions
                {
                    Window = TimeSpan.FromMinutes(1),
                    PermitLimit = 20,
                    SegmentsPerWindow = 4,
                    QueueLimit = 0,
                    AutoReplenishment = true
                });
            });

            options.AddPolicy(AppRateLimitPolicies.AuthUser, httpContext =>
            {
                var partitionKey = httpContext.User.FindFirstValue(AppClaims.JwtSubject)
                    ?? httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? httpContext.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown";

                return RateLimitPartition.GetSlidingWindowLimiter(partitionKey, _ => new SlidingWindowRateLimiterOptions
                {
                    Window = TimeSpan.FromMinutes(1),
                    PermitLimit = 60,
                    SegmentsPerWindow = 6,
                    QueueLimit = 0,
                    AutoReplenishment = true
                });
            });

            options.AddPolicy(AppRateLimitPolicies.AuthRefresh, httpContext =>
            {
                var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetTokenBucketLimiter(partitionKey, _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 12,
                    TokensPerPeriod = 12,
                    ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                    AutoReplenishment = true
                });
            });

            options.AddPolicy(AppRateLimitPolicies.WalletRead, httpContext =>
            {
                var partitionKey = httpContext.User.FindFirstValue(AppClaims.JwtSubject)
                    ?? httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? httpContext.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown";

                return RateLimitPartition.GetSlidingWindowLimiter(partitionKey, _ => new SlidingWindowRateLimiterOptions
                {
                    Window = TimeSpan.FromMinutes(1),
                    PermitLimit = 120,
                    SegmentsPerWindow = 6,
                    QueueLimit = 0,
                    AutoReplenishment = true
                });
            });

            options.AddPolicy(AppRateLimitPolicies.WalletWrite, httpContext =>
            {
                var partitionKey = httpContext.User.FindFirstValue(AppClaims.JwtSubject)
                    ?? httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? httpContext.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown";

                return RateLimitPartition.GetTokenBucketLimiter(partitionKey, _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 20,
                    TokensPerPeriod = 20,
                    ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                    AutoReplenishment = true
                });
            });
        });

        return services;
    }
}
