namespace FinanceApp.API.Bootstrap.Constans;

public static class AppClaims
{
    public const string JwtSubject = "sub";
}

public static class AppCorsPolicies
{
    public const string Frontend = "FrontendPolicy";
    public const string Development = "DevPolicy";
}

public static class AppRateLimitPolicies
{
    public const string AuthAnon = "auth-anon";
    public const string AuthUser = "auth-user";
    public const string AuthRefresh = "auth-refresh";
    public const string WalletRead = "wallet-read";
    public const string WalletWrite = "wallet-write";
}