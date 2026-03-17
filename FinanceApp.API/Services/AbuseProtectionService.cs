using System.Collections.Concurrent;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace FinanceApp.API.Services;

public sealed class AbuseProtectionService
{
    private readonly ILogger<AbuseProtectionService> _logger;
    private readonly ConcurrentDictionary<string, TokenBucketRateLimiter> _loginIpLimiters = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, SlidingWindowRateLimiter> _loginEmailLimiters = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, TokenBucketRateLimiter> _forgotIpLimiters = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, SlidingWindowRateLimiter> _forgotEmailLimiters = new(StringComparer.OrdinalIgnoreCase);

    public AbuseProtectionService(ILogger<AbuseProtectionService> logger)
    {
        _logger = logger;
    }

    public async ValueTask<AbuseDecision> CheckLoginAsync(string ipAddress, string? email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(email);
        var ipLease = await GetLoginIpLimiter(ipAddress).AcquireAsync(1, cancellationToken);
        if (!ipLease.IsAcquired)
        {
            return Reject("login", "ip", ipAddress, email, ipLease);
        }

        var emailLease = await GetLoginEmailLimiter(normalizedEmail).AcquireAsync(1, cancellationToken);
        if (!emailLease.IsAcquired)
        {
            return Reject("login", "email", ipAddress, email, emailLease);
        }

        return AbuseDecision.Allow();
    }

    public async ValueTask<AbuseDecision> CheckForgotPasswordAsync(string ipAddress, string? email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(email);
        var ipLease = await GetForgotIpLimiter(ipAddress).AcquireAsync(1, cancellationToken);
        if (!ipLease.IsAcquired)
        {
            return Reject("forgot-password", "ip", ipAddress, email, ipLease);
        }

        var emailLease = await GetForgotEmailLimiter(normalizedEmail).AcquireAsync(1, cancellationToken);
        if (!emailLease.IsAcquired)
        {
            return Reject("forgot-password", "email", ipAddress, email, emailLease);
        }

        return AbuseDecision.Allow();
    }

    public void LogMiddlewareRejection(HttpContext httpContext, TimeSpan? retryAfter)
    {
        _logger.LogWarning(
            "security.rate_limit.rejected path={Path} method={Method} ip={Ip} userId={UserId} retryAfterSeconds={RetryAfterSeconds}",
            httpContext.Request.Path,
            httpContext.Request.Method,
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            httpContext.User?.Identity?.Name ?? httpContext.User?.FindFirst("sub")?.Value ?? "anonymous",
            retryAfter.HasValue ? Math.Ceiling(retryAfter.Value.TotalSeconds) : null);
    }

    public void LogAccountLockout(string email, string ipAddress, DateTime lockoutEndAt)
    {
        _logger.LogWarning(
            "security.account.locked email={Email} ip={Ip} lockoutEndAt={LockoutEndAt}",
            email,
            ipAddress,
            lockoutEndAt);
    }

    private AbuseDecision Reject(string endpoint, string dimension, string ipAddress, string? email, RateLimitLease lease)
    {
        var retryAfter = TryGetRetryAfter(lease);

        _logger.LogWarning(
            "security.abuse.detected endpoint={Endpoint} dimension={Dimension} ip={Ip} email={Email} retryAfterSeconds={RetryAfterSeconds}",
            endpoint,
            dimension,
            ipAddress,
            string.IsNullOrWhiteSpace(email) ? "unknown" : email,
            retryAfter.HasValue ? Math.Ceiling(retryAfter.Value.TotalSeconds) : null);

        return AbuseDecision.Block("Terlalu banyak request mencurigakan. Coba lagi sebentar.", retryAfter);
    }

    private TokenBucketRateLimiter GetLoginIpLimiter(string ipAddress)
    {
        var key = NormalizeIp(ipAddress);
        return _loginIpLimiters.GetOrAdd(key, static _ => new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
        {
            TokenLimit = 10,
            TokensPerPeriod = 10,
            ReplenishmentPeriod = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            AutoReplenishment = true
        }));
    }

    private SlidingWindowRateLimiter GetLoginEmailLimiter(string email)
    {
        return _loginEmailLimiters.GetOrAdd(email, static _ => new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(10),
            SegmentsPerWindow = 5,
            QueueLimit = 0,
            AutoReplenishment = true
        }));
    }

    private TokenBucketRateLimiter GetForgotIpLimiter(string ipAddress)
    {
        var key = NormalizeIp(ipAddress);
        return _forgotIpLimiters.GetOrAdd(key, static _ => new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
        {
            TokenLimit = 5,
            TokensPerPeriod = 5,
            ReplenishmentPeriod = TimeSpan.FromMinutes(5),
            QueueLimit = 0,
            AutoReplenishment = true
        }));
    }

    private SlidingWindowRateLimiter GetForgotEmailLimiter(string email)
    {
        return _forgotEmailLimiters.GetOrAdd(email, static _ => new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
        {
            PermitLimit = 3,
            Window = TimeSpan.FromMinutes(15),
            SegmentsPerWindow = 5,
            QueueLimit = 0,
            AutoReplenishment = true
        }));
    }

    private static string NormalizeEmail(string? email)
    {
        return string.IsNullOrWhiteSpace(email) ? "unknown" : email.Trim().ToLowerInvariant();
    }

    private static string NormalizeIp(string? ipAddress)
    {
        return string.IsNullOrWhiteSpace(ipAddress) ? "unknown" : ipAddress;
    }

    private static TimeSpan? TryGetRetryAfter(RateLimitLease lease)
    {
        return lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter) ? retryAfter : null;
    }
}

public sealed record AbuseDecision(bool Allowed, string? Message, TimeSpan? RetryAfter)
{
    public static AbuseDecision Allow() => new(true, null, null);

    public static AbuseDecision Block(string message, TimeSpan? retryAfter) => new(false, message, retryAfter);
}
