using Newtonsoft.Json;

namespace Coze.Sdk.Models.Chat;

/// <summary>
/// 失败 Chat 的错误信息。
/// </summary>
public record ChatError
{
    /// <summary>
    /// 获取错误码。
    /// </summary>
    [JsonProperty("code")]
    public int? Code { get; init; }

    /// <summary>
    /// 获取错误消息。
    /// </summary>
    [JsonProperty("msg")]
    public string? Message { get; init; }
}
