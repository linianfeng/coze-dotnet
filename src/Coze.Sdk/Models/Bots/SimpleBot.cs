using Newtonsoft.Json;

namespace Coze.Sdk.Models.Bots;

/// <summary>
/// 用于列表响应的简单 Bot 信息。
/// </summary>
public record SimpleBot
{
    /// <summary>
    /// 获取 Bot ID。
    /// </summary>
    [JsonProperty("bot_id")]
    public string? Id { get; init; }

    /// <summary>
    /// 获取 Bot 名称。
    /// </summary>
    [JsonProperty("bot_name")]
    public string? Name { get; init; }

    /// <summary>
    /// 获取描述。
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; init; }

    /// <summary>
    /// 获取图标 URL。
    /// </summary>
    [JsonProperty("icon_url")]
    public string? IconUrl { get; init; }

    /// <summary>
    /// 获取发布时间。
    /// </summary>
    [JsonProperty("publish_time")]
    public string? PublishTime { get; init; }
}
