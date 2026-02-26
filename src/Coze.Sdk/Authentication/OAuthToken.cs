using Newtonsoft.Json;

namespace Coze.Sdk.Authentication;

/// <summary>
/// OAuth Token 响应。
/// 对应 Java SDK 中的 OAuthToken.java。
/// </summary>
public record OAuthToken
{
    /// <summary>
    /// 获取访问 Token。
    /// </summary>
    [JsonProperty("access_token")]
    public string? AccessToken { get; init; }

    /// <summary>
    /// 获取刷新 Token。
    /// </summary>
    [JsonProperty("refresh_token")]
    public string? RefreshToken { get; init; }

    /// <summary>
    /// 获取 Token 类型。
    /// </summary>
    [JsonProperty("token_type")]
    public string? TokenType { get; init; }

    /// <summary>
    /// 获取过期时间（秒）。
    /// </summary>
    [JsonProperty("expires_in")]
    public int? ExpiresIn { get; init; }

    /// <summary>
    /// 获取权限范围。
    /// </summary>
    [JsonProperty("scope")]
    public string? Scope { get; init; }

    /// <summary>
    /// 获取日志 ID。
    /// </summary>
    [JsonIgnore]
    public string? LogId { get; init; }

    /// <summary>
    /// 获取以 DateTimeOffset 表示的过期时间。
    /// </summary>
    [JsonIgnore]
    public DateTimeOffset? ExpiresAt => ExpiresIn.HasValue
        ? DateTimeOffset.UtcNow.AddSeconds(ExpiresIn.Value)
        : null;

    /// <summary>
    /// 获取一个值，指示 Token 是否已过期。
    /// </summary>
    [JsonIgnore]
    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value <= DateTimeOffset.UtcNow;
}
