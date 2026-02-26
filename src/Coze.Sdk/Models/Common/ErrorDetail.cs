using Newtonsoft.Json;

namespace Coze.Sdk.Models.Common;

/// <summary>
/// 错误详情信息。
/// 对应 Java SDK 中的 Detail.java。
/// </summary>
public record ErrorDetail
{
    /// <summary>
    /// 获取用于请求追踪的日志 ID。
    /// </summary>
    [JsonProperty("logid")]
    public string? LogId { get; init; }
}
