using System.Security.Cryptography;
using System.Text;

namespace Coze.Sdk.Authentication;

/// <summary>
/// PKCE 的码挑战方法。
/// </summary>
public enum CodeChallengeMethod
{
    /// <summary>
    /// 纯文本挑战。
    /// </summary>
    Plain,

    /// <summary>
    /// S256 (SHA-256) 挑战。
    /// </summary>
    S256
}

/// <summary>
/// 生成 PKCE OAuth URL 的响应。
/// </summary>
public record PkceAuthUrlResponse
{
    /// <summary>
    /// 获取码验证器，必须保存并在交换码时使用。
    /// </summary>
    public string CodeVerifier { get; init; } = string.Empty;

    /// <summary>
    /// 获取用于重定向用户的授权 URL。
    /// </summary>
    public string AuthorizationUrl { get; init; } = string.Empty;
}

/// <summary>
/// 使用 PKCE 的公共客户端（移动端、原生应用）OAuth 客户端。
/// </summary>
public class PkceOAuthClient : OAuthClient
{
    private const int CodeVerifierLength = 16;

    /// <summary>
    /// 初始化 <see cref="PkceOAuthClient"/> 类的新实例。
    /// </summary>
    public PkceOAuthClient(OAuthOptions options) : base(options)
    {
    }

    /// <summary>
    /// 生成 PKCE OAuth 授权 URL。
    /// </summary>
    public PkceAuthUrlResponse GenerateOAuthUrl(string redirectUri, string state, CodeChallengeMethod method = CodeChallengeMethod.Plain)
    {
        return GenerateOAuthUrl(redirectUri, state, method, null);
    }

    /// <summary>
    /// 生成指定工作空间的 PKCE OAuth 授权 URL。
    /// </summary>
    public PkceAuthUrlResponse GenerateOAuthUrl(string redirectUri, string state, CodeChallengeMethod method, string? workspaceId)
    {
        var codeVerifier = GenerateCodeVerifier();
        var codeChallenge = GetCodeChallenge(codeVerifier, method);
        var codeChallengeMethod = method == CodeChallengeMethod.Plain ? "plain" : "S256";

        var url = BuildOAuthUrl(redirectUri, state, codeChallenge, codeChallengeMethod, workspaceId);

        return new PkceAuthUrlResponse
        {
            CodeVerifier = codeVerifier,
            AuthorizationUrl = url
        };
    }

    /// <summary>
    /// 使用 PKCE 将授权码交换为访问 Token。
    /// </summary>
    public Task<OAuthToken> GetAccessTokenAsync(string code, string redirectUri, string codeVerifier, CancellationToken cancellationToken = default)
    {
        return ExchangeCodeForTokenAsync(code, redirectUri, codeVerifier, cancellationToken);
    }

    // 注意：RefreshTokenAsync 继承自 OAuthClient 基类

    private static string GenerateCodeVerifier()
    {
        var bytes = new byte[CodeVerifierLength];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }

    private static string GetCodeChallenge(string codeVerifier, CodeChallengeMethod method)
    {
        if (method == CodeChallengeMethod.Plain)
        {
            return codeVerifier;
        }

        return GenerateS256CodeChallenge(codeVerifier);
    }

    /// <summary>
    /// 从码验证器生成 S256 码挑战。
    /// </summary>
    public static string GenerateS256CodeChallenge(string codeVerifier)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(codeVerifier));
        return Convert.ToBase64String(hash)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}
