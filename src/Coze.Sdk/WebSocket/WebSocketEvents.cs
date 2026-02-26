using Newtonsoft.Json;

namespace Coze.Sdk.WebSocket;

#region 上行事件（客户端 -> 服务器）

/// <summary>
/// 聊天更新事件。
/// 对应 Java SDK 中的 ChatUpdateEvent.java。
/// </summary>
internal record ChatUpdateEvent
{
    [JsonProperty("event_type")]
    public string EventType => WebSocketEventTypes.ChatUpdate;

    [JsonProperty("data")]
    public ChatUpdateEventData? Data { get; init; }
}

/// <summary>
/// 会话消息创建事件。
/// 对应 Java SDK 中的 ConversationMessageCreateEvent.java。
/// </summary>
internal record ConversationMessageCreateEvent
{
    [JsonProperty("event_type")]
    public string EventType => WebSocketEventTypes.ConversationMessageCreate;

    [JsonProperty("data")]
    public MessageData? Data { get; init; }
}

/// <summary>
/// 会话聊天提交工具输出事件。
/// </summary>
internal record ConversationChatSubmitToolOutputsEvent
{
    [JsonProperty("event_type")]
    public string EventType => WebSocketEventTypes.ConversationChatSubmitToolOutputs;

    [JsonProperty("data")]
    public SubmitToolOutputsEventData? Data { get; init; }
}

/// <summary>
/// 会话聊天取消事件。
/// </summary>
internal record ConversationChatCancelEvent
{
    [JsonProperty("event_type")]
    public string EventType => WebSocketEventTypes.ConversationChatCancel;
}

/// <summary>
/// 会话清除事件。
/// </summary>
internal record ConversationClearEvent
{
    [JsonProperty("event_type")]
    public string EventType => WebSocketEventTypes.ConversationClear;
}

/// <summary>
/// 输入音频缓冲区追加事件。
/// </summary>
internal record InputAudioBufferAppendEvent
{
    [JsonProperty("event_type")]
    public string EventType => WebSocketEventTypes.InputAudioBufferAppend;

    [JsonProperty("data")]
    public string? Data { get; init; }
}

/// <summary>
/// 输入音频缓冲区清除事件。
/// </summary>
internal record InputAudioBufferClearEvent
{
    [JsonProperty("event_type")]
    public string EventType => WebSocketEventTypes.InputAudioBufferClear;
}

/// <summary>
/// 输入音频缓冲区完成事件。
/// </summary>
internal record InputAudioBufferCompleteEvent
{
    [JsonProperty("event_type")]
    public string EventType => WebSocketEventTypes.InputAudioBufferComplete;
}

/// <summary>
/// 语音合成更新事件。
/// </summary>
internal record SpeechUpdateEvent
{
    [JsonProperty("event_type")]
    public string EventType => WebSocketEventTypes.SpeechUpdate;

    [JsonProperty("data")]
    public SpeechUpdateEventData? Data { get; init; }
}

/// <summary>
/// 转录更新事件。
/// </summary>
internal record TranscriptionsUpdateEvent
{
    [JsonProperty("event_type")]
    public string EventType => WebSocketEventTypes.TranscriptionsUpdate;

    [JsonProperty("data")]
    public TranscriptionsUpdateEventData? Data { get; init; }
}

#endregion
