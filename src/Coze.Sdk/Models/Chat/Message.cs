using System.Text;
using Coze.Sdk.Utils;
using Newtonsoft.Json;

namespace Coze.Sdk.Models.Chat;

/// <summary>
/// 表示会话中的一条消息。
/// 对应 Java SDK 中的 Message.java。
/// </summary>
public record Message
{
    /// <summary>
    /// 获取消息发送者的角色。
    /// </summary>
    [JsonProperty("role")]
    public MessageRole? Role { get; init; }

    /// <summary>
    /// 获取消息类型。
    /// </summary>
    [JsonProperty("type")]
    public MessageType? Type { get; init; }

    /// <summary>
    /// 获取消息内容。
    /// </summary>
    [JsonProperty("content")]
    public string? Content { get; init; }

    /// <summary>
    /// 获取消息的内容类型。
    /// </summary>
    [JsonProperty("content_type")]
    public MessageContentType? ContentType { get; init; }

    /// <summary>
    /// 获取消息的附加元数据。
    /// </summary>
    [JsonProperty("meta_data")]
    public IReadOnlyDictionary<string, string>? MetaData { get; init; }

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
    /// 获取用于区分上下文片段的段落 ID。
    /// </summary>
    [JsonProperty("section_id")]
    public string? SectionId { get; init; }

    /// <summary>
    /// 获取 Bot ID。
    /// </summary>
    [JsonProperty("bot_id")]
    public string? BotId { get; init; }

    /// <summary>
    /// 获取 Chat ID。
    /// </summary>
    [JsonProperty("chat_id")]
    public string? ChatId { get; init; }

    /// <summary>
    /// 获取创建时间戳（秒）。
    /// </summary>
    [JsonProperty("created_at")]
    public long? CreatedAt { get; init; }

    /// <summary>
    /// 获取更新时间戳（秒）。
    /// </summary>
    [JsonProperty("updated_at")]
    public long? UpdatedAt { get; init; }

    /// <summary>
    /// 获取消息的推理内容。
    /// </summary>
    [JsonProperty("reasoning_content")]
    public string? ReasoningContent { get; init; }

    /// <summary>
    /// 如果内容类型为音频，获取音频数据。
    /// </summary>
    [JsonIgnore]
    public byte[]? Audio =>
        ContentType == MessageContentType.Audio && Content != null
            ? Convert.FromBase64String(Content)
            : null;

    /// <summary>
    /// 创建用户问题文本消息。
    /// </summary>
    /// <param name="content">消息内容。</param>
    /// <param name="metaData">可选的元数据。</param>
    /// <returns>新的 Message 实例。</returns>
    public static Message BuildUserQuestionText(string content, IReadOnlyDictionary<string, string>? metaData = null)
    {
        return new Message
        {
            Role = MessageRole.User,
            Type = MessageType.Question,
            Content = content,
            ContentType = MessageContentType.Text,
            MetaData = metaData
        };
    }

    /// <summary>
    /// 创建助手回答消息。
    /// </summary>
    /// <param name="content">消息内容。</param>
    /// <param name="metaData">可选的元数据。</param>
    /// <returns>新的 Message 实例。</returns>
    public static Message BuildAssistantAnswer(string content, IReadOnlyDictionary<string, string>? metaData = null)
    {
        return new Message
        {
            Role = MessageRole.Assistant,
            Type = MessageType.Answer,
            Content = content,
            ContentType = MessageContentType.Text,
            MetaData = metaData
        };
    }

    /// <summary>
    /// 从 JSON 字符串创建 Message 实例。
    /// </summary>
    /// <param name="json">JSON 字符串。</param>
    /// <returns>新的 Message 实例。</returns>
    public static Message? FromJson(string json)
    {
        return JsonHelper.DeserializeObject<Message>(json);
    }
}
