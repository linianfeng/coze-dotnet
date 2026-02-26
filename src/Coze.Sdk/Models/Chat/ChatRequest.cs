using Newtonsoft.Json;

namespace Coze.Sdk.Models.Chat;

/// <summary>
/// 创建 Chat 的请求。
/// 对应 Java SDK 中的 CreateChatReq.java。
/// </summary>
public record ChatRequest
{
    /// <summary>
    /// 获取会话 ID。
    /// </summary>
    [JsonProperty("conversation_id")]
    public string? ConversationId { get; init; }

    /// <summary>
    /// 获取 Bot ID。
    /// </summary>
    [JsonProperty("bot_id")]
    public required string BotId { get; init; }

    /// <summary>
    /// 获取用户 ID。
    /// </summary>
    [JsonProperty("user_id")]
    public required string UserId { get; init; }

    /// <summary>
    /// 获取会话的附加消息。
    /// </summary>
    [JsonProperty("additional_messages")]
    public IReadOnlyList<Message>? Messages { get; init; }

    /// <summary>
    /// 获取一个值，指示是否流式返回响应。
    /// </summary>
    [JsonProperty("stream")]
    public bool? Stream { get; init; }

    /// <summary>
    /// 获取自定义变量。
    /// </summary>
    [JsonProperty("custom_variables")]
    public IReadOnlyDictionary<string, string>? CustomVariables { get; init; }

    /// <summary>
    /// 获取一个值，指示是否自动保存历史记录。
    /// </summary>
    [JsonProperty("auto_save_history")]
    public bool? AutoSaveHistory { get; init; }

    /// <summary>
    /// 获取附加元数据。
    /// </summary>
    [JsonProperty("meta_data")]
    public IReadOnlyDictionary<string, string>? MetaData { get; init; }

    /// <summary>
    /// 获取工作流的自定义参数。
    /// </summary>
    [JsonProperty("parameters")]
    public IReadOnlyDictionary<string, object?>? Parameters { get; init; }

    /// <summary>
    /// 获取一个值，指示是否启用卡片类型响应。
    /// </summary>
    [JsonProperty("enable_card")]
    public bool? EnableCard { get; init; }

    /// <summary>
    /// 创建此请求的流式版本。
    /// </summary>
    /// <returns>Stream 设置为 true 的新 ChatRequest。</returns>
    public ChatRequest WithStream()
    {
        return this with { Stream = true };
    }

    /// <summary>
    /// 创建此请求的非流式版本。
    /// </summary>
    /// <returns>Stream 设置为 false 的新 ChatRequest。</returns>
    public ChatRequest WithoutStream()
    {
        return this with { Stream = false, AutoSaveHistory = AutoSaveHistory ?? true };
    }
}
