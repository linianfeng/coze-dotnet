namespace Coze.Sdk.Authentication;

/// <summary>
/// OAuth 客户端创建 CozeClient 实例的扩展方法。
/// </summary>
public static class OAuthClientExtensions
{
    /// <summary>
    /// 从 OAuth Token 创建 CozeClient（静态 Token，不刷新）。
    /// </summary>
    public static CozeClient CreateClient(this OAuthToken token, string? baseUrl = null)
    {
        if (token == null) throw new ArgumentNullException(nameof(token));
        if (string.IsNullOrEmpty(token.AccessToken))
            throw new InvalidOperationException("OAuth token 不包含访问 Token。");

        return new CozeClient(new CozeOptions
        {
            Auth = new TokenAuth(token.AccessToken!),
            BaseUrl = baseUrl ?? "https://api.coze.cn"
        });
    }

    /// <summary>
    /// 从此 OAuth 客户端和初始 Token 创建 RefreshableTokenAuth。
    /// 返回的 auth 可用于 CozeClient 实现 Token 自动刷新。
    /// </summary>
    public static RefreshableTokenAuth CreateRefreshableAuth(this WebOAuthClient client, OAuthToken initialToken)
    {
        return RefreshableTokenAuth.Create(
            initialToken,
            (refreshToken, ct) => client.RefreshTokenAsync(refreshToken, ct));
    }

    /// <summary>
    /// 从此 PKCE OAuth 客户端和初始 Token 创建 RefreshableTokenAuth。
    /// </summary>
    public static RefreshableTokenAuth CreateRefreshableAuth(this PkceOAuthClient client, OAuthToken initialToken)
    {
        return RefreshableTokenAuth.Create(
            initialToken,
            (refreshToken, ct) => client.RefreshTokenAsync(refreshToken, ct));
    }

    /// <summary>
    /// 创建带有自动刷新 OAuth Token 的 CozeClient。
    /// </summary>
    public static CozeClient CreateRefreshableClient(
        this WebOAuthClient client,
        OAuthToken initialToken,
        string? baseUrl = null)
    {
        var auth = client.CreateRefreshableAuth(initialToken);
        return new CozeClient(new CozeOptions
        {
            Auth = auth,
            BaseUrl = baseUrl ?? client.BaseUrl
        });
    }

    /// <summary>
    /// 创建带有自动刷新 PKCE OAuth Token 的 CozeClient。
    /// </summary>
    public static CozeClient CreateRefreshableClient(
        this PkceOAuthClient client,
        OAuthToken initialToken,
        string? baseUrl = null)
    {
        var auth = client.CreateRefreshableAuth(initialToken);
        return new CozeClient(new CozeOptions
        {
            Auth = auth,
            BaseUrl = baseUrl ?? client.BaseUrl
        });
    }
}

/// <summary>
/// 使用 OAuth 认证创建 CozeClient 实例的工厂类。
/// </summary>
public static class OAuthCozeClientFactory
{
    /// <summary>
    /// 使用 Web OAuth 流程创建 CozeClient（静态 Token，不刷新）。
    /// </summary>
    public static async Task<(CozeClient Client, OAuthToken Token)> CreateFromWebOAuthAsync(
        OAuthOptions options,
        string authorizationCode,
        string redirectUri,
        CancellationToken cancellationToken = default)
    {
        using var oauthClient = new WebOAuthClient(options);
        var token = await oauthClient.GetAccessTokenAsync(authorizationCode, redirectUri, cancellationToken);
        var client = token.CreateClient(options.BaseUrl);
        return (client, token);
    }

    /// <summary>
    /// 创建带有自动刷新 Web OAuth Token 的 CozeClient。
    /// </summary>
    public static async Task<(CozeClient Client, RefreshableTokenAuth Auth, OAuthToken Token)> CreateRefreshableFromWebOAuthAsync(
        OAuthOptions options,
        string authorizationCode,
        string redirectUri,
        CancellationToken cancellationToken = default)
    {
        var oauthClient = new WebOAuthClient(options);
        var token = await oauthClient.GetAccessTokenAsync(authorizationCode, redirectUri, cancellationToken);
        var auth = oauthClient.CreateRefreshableAuth(token);
        var client = new CozeClient(new CozeOptions { Auth = auth, BaseUrl = options.BaseUrl });
        return (client, auth, token);
    }

    /// <summary>
    /// 使用 PKCE OAuth 流程创建 CozeClient。
    /// </summary>
    public static async Task<(CozeClient Client, OAuthToken Token)> CreateFromPkceOAuthAsync(
        OAuthOptions options,
        string authorizationCode,
        string redirectUri,
        string codeVerifier,
        CancellationToken cancellationToken = default)
    {
        using var oauthClient = new PkceOAuthClient(options);
        var token = await oauthClient.GetAccessTokenAsync(authorizationCode, redirectUri, codeVerifier, cancellationToken);
        var client = token.CreateClient(options.BaseUrl);
        return (client, token);
    }

    /// <summary>
    /// 创建带有自动刷新 PKCE OAuth Token 的 CozeClient。
    /// </summary>
    public static async Task<(CozeClient Client, RefreshableTokenAuth Auth, OAuthToken Token)> CreateRefreshableFromPkceOAuthAsync(
        OAuthOptions options,
        string authorizationCode,
        string redirectUri,
        string codeVerifier,
        CancellationToken cancellationToken = default)
    {
        var oauthClient = new PkceOAuthClient(options);
        var token = await oauthClient.GetAccessTokenAsync(authorizationCode, redirectUri, codeVerifier, cancellationToken);
        var auth = oauthClient.CreateRefreshableAuth(token);
        var client = new CozeClient(new CozeOptions { Auth = auth, BaseUrl = options.BaseUrl });
        return (client, auth, token);
    }

    /// <summary>
    /// 使用设备 OAuth 流程创建 CozeClient。
    /// </summary>
    public static async Task<(CozeClient Client, OAuthToken Token)> CreateFromDeviceOAuthAsync(
        OAuthOptions options,
        Action<DeviceAuthResponse> onVerificationRequired,
        CancellationToken cancellationToken = default)
    {
        using var oauthClient = new DeviceOAuthClient(options);
        var deviceAuth = await oauthClient.GetDeviceCodeAsync(cancellationToken);
        onVerificationRequired(deviceAuth);
        var token = await oauthClient.PollAccessTokenAsync(deviceAuth.DeviceCode!, cancellationToken);
        var client = token.CreateClient(options.BaseUrl);
        return (client, token);
    }

    /// <summary>
    /// 从 JWT OAuth 客户端创建 CozeClient（自动刷新）。
    /// </summary>
    public static CozeClient CreateFromJwtOAuth(OAuthOptions options)
    {
        var auth = new JwtOAuthClient(options);
        return new CozeClient(new CozeOptions { Auth = auth, BaseUrl = options.BaseUrl });
    }
}
