namespace Coze.Sdk.Authentication;

/// <summary>
/// 认证提供者的抽象基类。
/// </summary>
public abstract class Auth
{
    /// <summary>
    /// 获取 Token 类型（例如："Bearer"）。
    /// </summary>
    public virtual string TokenType => "Bearer";

    /// <summary>
    /// 获取访问 Token。
    /// </summary>
    /// <returns>访问 Token 字符串。</returns>
    public abstract string GetToken();
}
