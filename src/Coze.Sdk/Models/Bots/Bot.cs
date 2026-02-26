using Newtonsoft.Json;

namespace Coze.Sdk.Models.Bots;

/// <summary>
/// 完整的 Bot 信息。
/// </summary>
public record Bot
{
    /// <summary>
    /// 获取 Bot ID。
    /// </summary>
    [JsonProperty("bot_id")]
    public string? Id { get; init; }

    /// <summary>
    /// 获取 Bot 名称。
    /// </summary>
    [JsonProperty("name")]
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
    /// 获取提示词信息。
    /// </summary>
    [JsonProperty("prompt_info")]
    public BotPromptInfo? PromptInfo { get; init; }

    /// <summary>
    /// 获取插件信息。
    /// </summary>
    [JsonProperty("plugin_info")]
    public BotPluginInfo? PluginInfo { get; init; }

    /// <summary>
    /// 获取知识库信息。
    /// </summary>
    [JsonProperty("knowledge_info")]
    public BotKnowledgeInfo? KnowledgeInfo { get; init; }

    /// <summary>
    /// 获取模型信息。
    /// </summary>
    [JsonProperty("model_info")]
    public BotModelInfo? ModelInfo { get; init; }

    /// <summary>
    /// 获取创建时间。
    /// </summary>
    [JsonProperty("create_time")]
    public long? CreateTime { get; init; }

    /// <summary>
    /// 获取更新时间。
    /// </summary>
    [JsonProperty("update_time")]
    public long? UpdateTime { get; init; }
}

/// <summary>
/// Bot 提示词信息。
/// </summary>
public record BotPromptInfo
{
    /// <summary>
    /// 获取提示词文本。
    /// </summary>
    [JsonProperty("prompt")]
    public string? Prompt { get; init; }
}

/// <summary>
/// Bot 插件信息。
/// </summary>
public record BotPluginInfo
{
    /// <summary>
    /// 获取插件 ID 列表。
    /// </summary>
    [JsonProperty("plugin_ids")]
    public IReadOnlyList<string>? PluginIds { get; init; }
}

/// <summary>
/// Bot 知识库信息。
/// </summary>
public record BotKnowledgeInfo
{
    /// <summary>
    /// 获取知识库 ID 列表。
    /// </summary>
    [JsonProperty("knowledge_ids")]
    public IReadOnlyList<string>? KnowledgeIds { get; init; }
}

/// <summary>
/// Bot 模型信息。
/// </summary>
public record BotModelInfo
{
    /// <summary>
    /// 获取模型 ID。
    /// </summary>
    [JsonProperty("model_id")]
    public string? ModelId { get; init; }
}
