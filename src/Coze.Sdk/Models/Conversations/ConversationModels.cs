using Newtonsoft.Json;

namespace Coze.Sdk.Models.Conversations;

/// <summary>
/// 会话模型。
/// 对应 Java SDK 中的 Conversation.java。
/// </summary>
public record Conversation
{
    /// <summary>
    /// 获取会话 ID。
    /// </summary>
    [JsonProperty("id")]
    public string? Id { get; init; }

    /// <summary>
    /// 获取创建时间戳。
    /// </summary>
    [JsonProperty("created_at")]
    public long? CreatedAt { get; init; }

    /// <summary>
    /// 获取元数据。
    /// </summary>
    [JsonProperty("meta_data")]
    public IReadOnlyDictionary<string, string>? MetaData { get; init; }

    /// <summary>
    /// 获取最后分段 ID。
    /// </summary>
    [JsonProperty("last_section_id")]
    public string? LastSectionId { get; init; }
}

/// <summary>
/// 创建会话的请求。
/// </summary>
public record CreateConversationRequest
{
    /// <summary>
    /// 获取元数据。
    /// </summary>
    [JsonProperty("meta_data")]
    public IReadOnlyDictionary<string, string>? MetaData { get; init; }

    /// <summary>
    /// 获取会话中包含的消息。
    /// </summary>
    [JsonProperty("messages")]
    public IReadOnlyList<ConversationMessage>? Messages { get; init; }

    /// <summary>
    /// 获取是否复用上一个会话。
    /// </summary>
    [JsonProperty("reuse_last_conversation")]
    public bool? ReuseLastConversation { get; init; }
}

/// <summary>
/// 会话创建时的消息。
/// </summary>
public record ConversationMessage
{
    /// <summary>
    /// 获取角色。
    /// </summary>
    [JsonProperty("role")]
    public string? Role { get; init; }

    /// <summary>
    /// 获取内容。
    /// </summary>
    [JsonProperty("content")]
    public string? Content { get; init; }

    /// <summary>
    /// 获取内容类型。
    /// </summary>
    [JsonProperty("content_type")]
    public string? ContentType { get; init; }
}

/// <summary>
/// 创建会话的响应。
/// </summary>
public record CreateConversationResponse
{
    /// <summary>
    /// 获取会话。
    /// </summary>
    [JsonProperty("data")]
    public Conversation? Conversation { get; init; }
}

/// <summary>
/// 获取会话的请求。
/// </summary>
public record RetrieveConversationRequest
{
    /// <summary>
    /// 获取会话 ID。
    /// </summary>
    [JsonProperty("conversation_id")]
    public required string ConversationId { get; init; }
}

/// <summary>
/// 获取会话的响应。
/// </summary>
public record RetrieveConversationResponse
{
    /// <summary>
    /// 获取会话。
    /// </summary>
    [JsonProperty("data")]
    public Conversation? Conversation { get; init; }
}

/// <summary>
/// 列出会话的请求。
/// </summary>
public record ListConversationsRequest
{
    /// <summary>
    /// 获取 Bot ID（必填）。
    /// </summary>
    [JsonProperty("bot_id")]
    public required string BotId { get; init; }

    /// <summary>
    /// 获取页码。
    /// </summary>
    [JsonProperty("page_num")]
    public int? PageNumber { get; init; } = 1;

    /// <summary>
    /// 获取每页数量。
    /// </summary>
    [JsonProperty("page_size")]
    public int? PageSize { get; init; } = 20;
}

/// <summary>
/// 列出会话的响应。
/// </summary>
public record ListConversationsResponse
{
    /// <summary>
    /// 获取会话列表。
    /// </summary>
    [JsonProperty("data")]
    public IReadOnlyList<Conversation>? Conversations { get; init; }

    /// <summary>
    /// 获取是否还有更多会话。
    /// </summary>
    [JsonProperty("has_more")]
    public bool HasMore { get; init; }
}

/// <summary>
/// 清除会话的请求。
/// </summary>
public record ClearConversationRequest
{
    /// <summary>
    /// 获取会话 ID。
    /// </summary>
    [JsonProperty("conversation_id")]
    public required string ConversationId { get; init; }
}
