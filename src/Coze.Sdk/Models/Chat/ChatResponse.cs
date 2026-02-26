using Newtonsoft.Json;

namespace Coze.Sdk.Models.Chat;

/// <summary>
/// 创建 Chat 的响应。
/// </summary>
public record ChatResponse
{
    /// <summary>
    /// 获取 Chat 数据。
    /// </summary>
    [JsonIgnore]
    public Chat? Chat { get; init; }

    /// <summary>
    /// 获取用于追踪的日志 ID。
    /// </summary>
    [JsonIgnore]
    public string? LogId { get; init; }
}

/// <summary>
/// 检索 Chat 的响应。
/// </summary>
public record RetrieveChatResponse
{
    /// <summary>
    /// 获取 Chat 数据。
    /// </summary>
    [JsonIgnore]
    public Chat? Chat { get; init; }

    /// <summary>
    /// 获取用于追踪的日志 ID。
    /// </summary>
    [JsonIgnore]
    public string? LogId { get; init; }
}

/// <summary>
/// 取消 Chat 的请求。
/// </summary>
public record CancelChatRequest
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
/// 取消 Chat 的响应。
/// </summary>
public record CancelChatResponse
{
    /// <summary>
    /// 获取 Chat 数据。
    /// </summary>
    [JsonIgnore]
    public Chat? Chat { get; init; }

    /// <summary>
    /// 获取用于追踪的日志 ID。
    /// </summary>
    [JsonIgnore]
    public string? LogId { get; init; }
}

/// <summary>
/// 轮询结果，包含 Chat 和消息列表。
/// </summary>
public record ChatPollResult
{
    /// <summary>
    /// 获取 Chat 数据。
    /// </summary>
    public Chat? Chat { get; init; }

    /// <summary>
    /// 获取消息列表。
    /// </summary>
    public IReadOnlyList<Message>? Messages { get; init; }
}
