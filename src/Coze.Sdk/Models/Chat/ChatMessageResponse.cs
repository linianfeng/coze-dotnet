using Newtonsoft.Json;

namespace Coze.Sdk.Models.Chat;

/// <summary>
/// 列出 Chat 消息的请求。
/// </summary>
public record ListMessagesRequest
{
    /// <summary>
    /// 获取会话 ID。
    /// </summary>
    [JsonProperty("conversation_id")]
    public required string ConversationId { get; init; }

    /// <summary>
    /// 获取 Chat ID。
    /// </summary>
    [JsonProperty("chat_id")]
    public required string ChatId { get; init; }
}

/// <summary>
/// 列出消息的响应。
/// </summary>
public record ListMessagesResponse
{
    /// <summary>
    /// 获取消息列表。
    /// </summary>
    [JsonIgnore]
    public IReadOnlyList<Message>? Messages { get; init; }

    /// <summary>
    /// 获取用于追踪的日志 ID。
    /// </summary>
    [JsonIgnore]
    public string? LogId { get; init; }
}
