namespace Coze.Sdk.Authentication;

/// <summary>
/// 使用个人访问 Token (PAT) 的 Token 认证。
/// </summary>
public class TokenAuth : Auth
{
    private readonly string _accessToken;

    /// <summary>
    /// 初始化 <see cref="TokenAuth"/> 类的新实例。
    /// </summary>
    /// <param name="accessToken">个人访问 Token。</param>
    /// <exception cref="ArgumentNullException">当 accessToken 为 null 时抛出。</exception>
    public TokenAuth(string accessToken)
    {
        _accessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
    }

    /// <inheritdoc/>
    public override string TokenType => "Bearer";

    /// <inheritdoc/>
    public override string GetToken() => _accessToken;
}
