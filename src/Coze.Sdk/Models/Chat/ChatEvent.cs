using Coze.Sdk.Exceptions;
using Newtonsoft.Json;

namespace Coze.Sdk.Models.Chat;

/// <summary>
/// 表示流式响应中的 Chat 事件。
/// 对应 Java SDK 中的 ChatEvent.java。
/// </summary>
public record ChatEvent
{
    /// <summary>
    /// 获取事件类型。
    /// </summary>
    [JsonProperty("event")]
    public ChatEventType? EventType { get; init; }

    /// <summary>
    /// 获取 Chat 数据（用于 Chat 状态事件）。
    /// </summary>
    [JsonProperty("chat")]
    public Chat? Chat { get; init; }

    /// <summary>
    /// 获取消息数据（用于消息事件）。
    /// </summary>
    [JsonProperty("message")]
    public Message? Message { get; init; }

    /// <summary>
    /// 获取用于追踪的日志 ID。
    /// </summary>
    [JsonIgnore]
    public string? LogId { get; init; }

    /// <summary>
    /// 获取一个值，指示事件流是否已完成。
    /// </summary>
    [JsonIgnore]
    public bool IsDone => EventType == ChatEventType.Done || EventType == ChatEventType.Error;

    /// <summary>
    /// 从 SSE 数据解析 Chat 事件。
    /// </summary>
    /// <param name="eventType">事件类型字符串。</param>
    /// <param name="data">事件数据 JSON。</param>
    /// <param name="logId">可选的用于追踪的日志 ID。</param>
    /// <returns>ChatEvent 实例。</returns>
    /// <exception cref="CozeApiException">遇到错误事件时抛出。</exception>
    public static ChatEvent ParseEvent(string eventType, string data, string? logId = null)
    {
        var type = ParseEventType(eventType);

        if (type == ChatEventType.Done)
        {
            return new ChatEvent { EventType = type, LogId = logId };
        }

        if (type == ChatEventType.Error)
        {
            throw new CozeApiException(0, 0, data, logId, data);
        }

        if (type == ChatEventType.ConversationMessageDelta ||
            type == ChatEventType.ConversationMessageCompleted ||
            type == ChatEventType.ConversationAudioDelta)
        {
            return new ChatEvent
            {
                EventType = type,
                Message = Message.FromJson(data),
                LogId = logId
            };
        }

        if (type == ChatEventType.ConversationChatCreated ||
            type == ChatEventType.ConversationChatInProgress ||
            type == ChatEventType.ConversationChatCompleted ||
            type == ChatEventType.ConversationChatFailed ||
            type == ChatEventType.ConversationChatRequiresAction)
        {
            return new ChatEvent
            {
                EventType = type,
                Chat = Chat.FromJson(data),
                LogId = logId
            };
        }

        return new ChatEvent { EventType = type, LogId = logId };
    }

    private static ChatEventType ParseEventType(string eventType)
    {
        return eventType switch
        {
            "conversation.chat.created" => ChatEventType.ConversationChatCreated,
            "conversation.chat.in_progress" => ChatEventType.ConversationChatInProgress,
            "conversation.message.delta" => ChatEventType.ConversationMessageDelta,
            "conversation.message.completed" => ChatEventType.ConversationMessageCompleted,
            "conversation.chat.completed" => ChatEventType.ConversationChatCompleted,
            "conversation.chat.failed" => ChatEventType.ConversationChatFailed,
            "conversation.chat.requires_action" => ChatEventType.ConversationChatRequiresAction,
            "conversation.audio.delta" => ChatEventType.ConversationAudioDelta,
            "error" => ChatEventType.Error,
            "done" => ChatEventType.Done,
            _ => ChatEventType.Done
        };
    }
}
