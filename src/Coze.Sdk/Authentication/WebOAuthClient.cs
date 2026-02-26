namespace Coze.Sdk.Authentication;

/// <summary>
/// 使用授权码流程的 Web 应用 OAuth 客户端。
/// </summary>
public class WebOAuthClient : OAuthClient
{
    /// <summary>
    /// 初始化 <see cref="WebOAuthClient"/> 类的新实例。
    /// </summary>
    public WebOAuthClient(OAuthOptions options) : base(options)
    {
    }

    /// <summary>
    /// 生成 OAuth 授权 URL。
    /// </summary>
    public string GetOAuthUrl(string redirectUri, string state)
    {
        return BuildOAuthUrl(redirectUri, state);
    }

    /// <summary>
    /// 生成指定工作空间的 OAuth 授权 URL。
    /// </summary>
    public string GetOAuthUrl(string redirectUri, string state, string workspaceId)
    {
        return BuildOAuthUrl(redirectUri, state, workspaceId: workspaceId);
    }

    /// <summary>
    /// 将授权码交换为访问 Token。
    /// </summary>
    public Task<OAuthToken> GetAccessTokenAsync(string code, string redirectUri, CancellationToken cancellationToken = default)
    {
        return ExchangeCodeForTokenAsync(code, redirectUri, null, cancellationToken);
    }

    // 注意：RefreshTokenAsync 继承自 OAuthClient 基类
}
