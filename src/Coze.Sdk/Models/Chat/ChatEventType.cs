using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Coze.Sdk.Models.Chat;

/// <summary>
/// Chat 流式事件类型。
/// 对应 Java SDK 中的 ChatEventType.java。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum ChatEventType
{
    /// <summary>
    /// 创建会话事件，表示会话开始。
    /// </summary>
    [EnumMember(Value = "conversation.chat.created")]
    ConversationChatCreated,

    /// <summary>
    /// 服务器正在处理会话。
    /// </summary>
    [EnumMember(Value = "conversation.chat.in_progress")]
    ConversationChatInProgress,

    /// <summary>
    /// 增量消息，通常是 type=answer 时的增量消息。
    /// </summary>
    [EnumMember(Value = "conversation.message.delta")]
    ConversationMessageDelta,

    /// <summary>
    /// 消息已完成回复。
    /// </summary>
    [EnumMember(Value = "conversation.message.completed")]
    ConversationMessageCompleted,

    /// <summary>
    /// 会话已完成。
    /// </summary>
    [EnumMember(Value = "conversation.chat.completed")]
    ConversationChatCompleted,

    /// <summary>
    /// 此事件用于标记失败的会话。
    /// </summary>
    [EnumMember(Value = "conversation.chat.failed")]
    ConversationChatFailed,

    /// <summary>
    /// 会话被中断，需要用户上报工具的执行结果。
    /// </summary>
    [EnumMember(Value = "conversation.chat.requires_action")]
    ConversationChatRequiresAction,

    /// <summary>
    /// 音频增量事件。
    /// </summary>
    [EnumMember(Value = "conversation.audio.delta")]
    ConversationAudioDelta,

    /// <summary>
    /// 流式响应过程中的错误事件。
    /// </summary>
    [EnumMember(Value = "error")]
    Error,

    /// <summary>
    /// 本次会话的流式响应正常结束。
    /// </summary>
    [EnumMember(Value = "done")]
    Done
}
