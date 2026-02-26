using Newtonsoft.Json;

namespace Coze.Sdk.Models.Chat;

/// <summary>
/// Chat 的 Token 使用信息。
/// </summary>
public record ChatUsage
{
    /// <summary>
    /// 获取总 Token 数量。
    /// </summary>
    [JsonProperty("token_count")]
    public int? TokenCount { get; init; }

    /// <summary>
    /// 获取输入 Token 数量。
    /// </summary>
    [JsonProperty("input_count")]
    public int? InputCount { get; init; }

    /// <summary>
    /// 获取输出 Token 数量。
    /// </summary>
    [JsonProperty("output_count")]
    public int? OutputCount { get; init; }

    /// <summary>
    /// 获取输入 Token 数量（另一个字段）。
    /// </summary>
    [JsonProperty("input_tokens")]
    public int? InputTokens { get; init; }

    /// <summary>
    /// 获取输出 Token 数量（另一个字段）。
    /// </summary>
    [JsonProperty("output_tokens")]
    public int? OutputTokens { get; init; }
}
