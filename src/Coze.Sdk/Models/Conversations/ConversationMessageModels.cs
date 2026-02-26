using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Coze.Sdk.Models.Conversations;

#region Enums

/// <summary>
/// 会话中的消息角色。
/// 对应 Java SDK 中的 MessageRole.java。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum ConversationMessageRole
{
    /// <summary>
    /// 未知角色。
    /// </summary>
    [EnumMember(Value = "unknown")]
    Unknown,

    /// <summary>
    /// 用户角色。
    /// </summary>
    [EnumMember(Value = "user")]
    User,

    /// <summary>
    /// 助手角色。
    /// </summary>
    [EnumMember(Value = "assistant")]
    Assistant
}

/// <summary>
/// 会话中的消息类型。
/// 对应 Java SDK 中的 MessageType.java。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum ConversationMessageType
{
    /// <summary>
    /// 未知类型。
    /// </summary>
    [EnumMember(Value = "")]
    Unknown,

    /// <summary>
    /// 用户问题/输入。
    /// </summary>
    [EnumMember(Value = "question")]
    Question,

    /// <summary>
    /// Bot 回答。
    /// </summary>
    [EnumMember(Value = "answer")]
    Answer,

    /// <summary>
    /// 函数调用中间结果。
    /// </summary>
    [EnumMember(Value = "function_call")]
    FunctionCall,

    /// <summary>
    /// 工具输出结果。
    /// </summary>
    [EnumMember(Value = "tool_output")]
    ToolOutput,

    /// <summary>
    /// 工具响应结果。
    /// </summary>
    [EnumMember(Value = "tool_response")]
    ToolResponse,

    /// <summary>
    /// 后续建议。
    /// </summary>
    [EnumMember(Value = "follow_up")]
    FollowUp,

    /// <summary>
    /// 详细消息。
    /// </summary>
    [EnumMember(Value = "verbose")]
    Verbose
}

/// <summary>
/// 会话中的消息内容类型。
/// 对应 Java SDK 中的 MessageContentType.java。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum ConversationMessageContentType
{
    /// <summary>
    /// 未知类型。
    /// </summary>
    [EnumMember(Value = "unknown")]
    Unknown,

    /// <summary>
    /// 文本内容。
    /// </summary>
    [EnumMember(Value = "text")]
    Text,

    /// <summary>
    /// 多模态内容（文本 + 文件/图片）。
    /// </summary>
    [EnumMember(Value = "object_string")]
    ObjectString,

    /// <summary>
    /// 卡片内容。
    /// </summary>
    [EnumMember(Value = "card")]
    Card,

    /// <summary>
    /// 音频内容。
    /// </summary>
    [EnumMember(Value = "audio")]
    Audio
}

/// <summary>
/// 多模态内容的消息对象字符串类型。
/// 对应 Java SDK 中的 MessageObjectStringType.java。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum MessageObjectStringType
{
    /// <summary>
    /// 未知类型。
    /// </summary>
    [EnumMember(Value = "unknown")]
    Unknown,

    /// <summary>
    /// 文本内容。
    /// </summary>
    [EnumMember(Value = "text")]
    Text,

    /// <summary>
    /// 文件内容。
    /// </summary>
    [EnumMember(Value = "file")]
    File,

    /// <summary>
    /// 图片内容。
    /// </summary>
    [EnumMember(Value = "image")]
    Image,

    /// <summary>
    /// 音频内容。
    /// </summary>
    [EnumMember(Value = "audio")]
    Audio
}

#endregion

#region Message Object String

/// <summary>
/// 多模态内容的消息对象字符串。
/// 对应 Java SDK 中的 MessageObjectString.java。
/// </summary>
public record MessageObjectString
{
    /// <summary>
    /// 获取内容类型。
    /// </summary>
    [JsonProperty("type")]
    public MessageObjectStringType? Type { get; init; }

    /// <summary>
    /// 获取文本内容（类型为文本时必填）。
    /// </summary>
    [JsonProperty("text")]
    public string? Text { get; init; }

    /// <summary>
    /// 获取文件 ID（用于文件/图片类型）。
    /// </summary>
    [JsonProperty("file_id")]
    public string? FileId { get; init; }

    /// <summary>
    /// 获取文件 URL（用于文件/图片类型）。
    /// </summary>
    [JsonProperty("file_url")]
    public string? FileUrl { get; init; }

    /// <summary>
    /// 创建文本对象。
    /// </summary>
    public static MessageObjectString BuildText(string text)
    {
        return new MessageObjectString
        {
            Type = MessageObjectStringType.Text,
            Text = text
        };
    }

    /// <summary>
    /// 通过文件 ID 创建图片对象。
    /// </summary>
    public static MessageObjectString BuildImageById(string fileId)
    {
        return new MessageObjectString
        {
            Type = MessageObjectStringType.Image,
            FileId = fileId
        };
    }

    /// <summary>
    /// 通过 URL 创建图片对象。
    /// </summary>
    public static MessageObjectString BuildImageByUrl(string fileUrl)
    {
        return new MessageObjectString
        {
            Type = MessageObjectStringType.Image,
            FileUrl = fileUrl
        };
    }

    /// <summary>
    /// 通过文件 ID 创建文件对象。
    /// </summary>
    public static MessageObjectString BuildFileById(string fileId)
    {
        return new MessageObjectString
        {
            Type = MessageObjectStringType.File,
            FileId = fileId
        };
    }

    /// <summary>
    /// 通过 URL 创建文件对象。
    /// </summary>
    public static MessageObjectString BuildFileByUrl(string fileUrl)
    {
        return new MessageObjectString
        {
            Type = MessageObjectStringType.File,
            FileUrl = fileUrl
        };
    }

    /// <summary>
    /// 创建音频对象。
    /// </summary>
    public static MessageObjectString BuildAudio(string? fileId = null, string? fileUrl = null)
    {
        if (string.IsNullOrEmpty(fileId) && string.IsNullOrEmpty(fileUrl))
        {
            throw new ArgumentException("file_id 或 file_url 必须指定");
        }
        return new MessageObjectString
        {
            Type = MessageObjectStringType.Audio,
            FileId = fileId,
            FileUrl = fileUrl
        };
    }
}

#endregion

#region Conversation Message Model

/// <summary>
/// 会话消息模型。
/// 对应 Java SDK 中的 Message.java（会话消息）。
/// </summary>
public record ConversationMessageDetail
{
    /// <summary>
    /// 获取消息 ID。
    /// </summary>
    [JsonProperty("id")]
    public string? Id { get; init; }

    /// <summary>
    /// 获取会话 ID。
    /// </summary>
    [JsonProperty("conversation_id")]
    public string? ConversationId { get; init; }

    /// <summary>
    /// 获取角色。
    /// </summary>
    [JsonProperty("role")]
    public ConversationMessageRole? Role { get; init; }

    /// <summary>
    /// 获取消息类型。
    /// </summary>
    [JsonProperty("type")]
    public ConversationMessageType? Type { get; init; }

    /// <summary>
    /// 获取内容。
    /// </summary>
    [JsonProperty("content")]
    public string? Content { get; init; }

    /// <summary>
    /// 获取内容类型。
    /// </summary>
    [JsonProperty("content_type")]
    public ConversationMessageContentType? ContentType { get; init; }

    /// <summary>
    /// 获取元数据。
    /// </summary>
    [JsonProperty("meta_data")]
    public IReadOnlyDictionary<string, string>? MetaData { get; init; }

    /// <summary>
    /// 获取分段 ID。
    /// </summary>
    [JsonProperty("section_id")]
    public string? SectionId { get; init; }

    /// <summary>
    /// 获取 Bot ID。
    /// </summary>
    [JsonProperty("bot_id")]
    public string? BotId { get; init; }

    /// <summary>
    /// 获取聊天 ID。
    /// </summary>
    [JsonProperty("chat_id")]
    public string? ChatId { get; init; }

    /// <summary>
    /// 获取创建时间戳。
    /// </summary>
    [JsonProperty("created_at")]
    public long? CreatedAt { get; init; }

    /// <summary>
    /// 获取更新时间戳。
    /// </summary>
    [JsonProperty("updated_at")]
    public long? UpdatedAt { get; init; }

    /// <summary>
    /// 获取推理内容。
    /// </summary>
    [JsonProperty("reasoning_content")]
    public string? ReasoningContent { get; init; }

    /// <summary>
    /// 获取音频字节数据（如果内容类型为音频）。
    /// </summary>
    [JsonIgnore]
    public byte[]? Audio =>
        ContentType == ConversationMessageContentType.Audio && Content != null
            ? Convert.FromBase64String(Content)
            : null;
}

#endregion

#region Requests

/// <summary>
/// 创建会话消息的请求。
/// 对应 Java SDK 中的 CreateMessageReq.java。
/// </summary>
public record CreateConversationMessageRequest
{
    /// <summary>
    /// 获取会话 ID。
    /// </summary>
    [JsonIgnore]
    public required string ConversationId { get; init; }

    /// <summary>
    /// 获取角色。
    /// </summary>
    [JsonProperty("role")]
    public required ConversationMessageRole Role { get; init; }

    /// <summary>
    /// 获取内容。
    /// </summary>
    [JsonProperty("content")]
    public string? Content { get; init; }

    /// <summary>
    /// 获取内容类型。
    /// </summary>
    [JsonProperty("content_type")]
    public required ConversationMessageContentType ContentType { get; init; }

    /// <summary>
    /// 获取元数据。
    /// </summary>
    [JsonProperty("meta_data")]
    public IReadOnlyDictionary<string, string>? MetaData { get; init; }

    /// <summary>
    /// 从消息对象列表设置对象内容。
    /// </summary>
    public void SetObjectContent(IReadOnlyList<MessageObjectString> objects)
    {
        // 此部分在客户端代码中处理
    }
}

/// <summary>
/// 列出会话消息的请求。
/// 对应 Java SDK 中的 ListMessageReq.java。
/// </summary>
public record ListConversationMessagesRequest
{
    /// <summary>
    /// 获取会话 ID。
    /// </summary>
    [JsonIgnore]
    public required string ConversationId { get; init; }

    /// <summary>
    /// 获取排序方式（asc 或 desc）。
    /// </summary>
    [JsonProperty("order")]
    public string Order { get; init; } = "desc";

    /// <summary>
    /// 获取聊天 ID 过滤条件。
    /// </summary>
    [JsonProperty("chat_id")]
    public string? ChatId { get; init; }

    /// <summary>
    /// 获取此 ID 之前的消息。
    /// </summary>
    [JsonProperty("before_id")]
    public string? BeforeId { get; init; }

    /// <summary>
    /// 获取此 ID 之后的消息。
    /// </summary>
    [JsonProperty("after_id")]
    public string? AfterId { get; init; }

    /// <summary>
    /// 获取每页数量限制（1-50，默认 20）。
    /// </summary>
    [JsonProperty("limit")]
    public int? Limit { get; init; } = 20;

    /// <summary>
    /// 获取 Bot ID 过滤条件。
    /// </summary>
    [JsonProperty("bot_id")]
    public string? BotId { get; init; }
}

/// <summary>
/// 获取会话消息的请求。
/// 对应 Java SDK 中的 RetrieveMessageReq.java。
/// </summary>
public record RetrieveConversationMessageRequest
{
    /// <summary>
    /// 获取会话 ID。
    /// </summary>
    [JsonIgnore]
    public required string ConversationId { get; init; }

    /// <summary>
    /// 获取消息 ID。
    /// </summary>
    [JsonIgnore]
    public required string MessageId { get; init; }
}

/// <summary>
/// 更新会话消息的请求。
/// 对应 Java SDK 中的 UpdateMessageReq.java。
/// </summary>
public record UpdateConversationMessageRequest
{
    /// <summary>
    /// 获取会话 ID。
    /// </summary>
    [JsonIgnore]
    public required string ConversationId { get; init; }

    /// <summary>
    /// 获取消息 ID。
    /// </summary>
    [JsonIgnore]
    public required string MessageId { get; init; }

    /// <summary>
    /// 获取内容。
    /// </summary>
    [JsonProperty("content")]
    public string? Content { get; init; }

    /// <summary>
    /// 获取内容类型。
    /// </summary>
    [JsonProperty("content_type")]
    public ConversationMessageContentType? ContentType { get; init; }

    /// <summary>
    /// 获取元数据。
    /// </summary>
    [JsonProperty("meta_data")]
    public IReadOnlyDictionary<string, string>? MetaData { get; init; }
}

/// <summary>
/// 删除会话消息的请求。
/// 对应 Java SDK 中的 DeleteMessageReq.java。
/// </summary>
public record DeleteConversationMessageRequest
{
    /// <summary>
    /// 获取会话 ID。
    /// </summary>
    [JsonIgnore]
    public required string ConversationId { get; init; }

    /// <summary>
    /// 获取消息 ID。
    /// </summary>
    [JsonIgnore]
    public required string MessageId { get; init; }
}

#endregion

#region Responses

/// <summary>
/// 列出会话消息的响应。
/// 对应 Java SDK 中的 ListMessageResp.java。
/// </summary>
public record ListConversationMessagesResponse
{
    /// <summary>
    /// 获取消息列表。
    /// </summary>
    [JsonProperty("data")]
    public IReadOnlyList<ConversationMessageDetail>? Messages { get; init; }

    /// <summary>
    /// 获取是否还有更多消息。
    /// </summary>
    [JsonProperty("has_more")]
    public bool HasMore { get; init; }

    /// <summary>
    /// 获取第一条消息 ID。
    /// </summary>
    [JsonProperty("first_id")]
    public string? FirstId { get; init; }

    /// <summary>
    /// 获取最后一条消息 ID。
    /// </summary>
    [JsonProperty("last_id")]
    public string? LastId { get; init; }
}

#endregion
