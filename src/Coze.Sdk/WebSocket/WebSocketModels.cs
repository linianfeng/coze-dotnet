using Newtonsoft.Json;

namespace Coze.Sdk.WebSocket;

#region Audio 配置模型

/// <summary>
/// 输入音频配置。
/// 对应 Java SDK 中的 InputAudio.java。
/// </summary>
public record InputAudio
{
    [JsonProperty("format")]
    public string? Format { get; init; }

    [JsonProperty("codec")]
    public string? Codec { get; init; }

    [JsonProperty("sample_rate")]
    public int? SampleRate { get; init; }

    [JsonProperty("channel")]
    public int? Channel { get; init; }

    [JsonProperty("bit_depth")]
    public int? BitDepth { get; init; }
}

/// <summary>
/// 输出音频配置。
/// 对应 Java SDK 中的 OutputAudio.java。
/// </summary>
public record OutputAudio
{
    [JsonProperty("codec")]
    public string? Codec { get; init; }

    [JsonProperty("pcm_config")]
    public PcmConfig? PcmConfig { get; init; }

    [JsonProperty("opus_config")]
    public OpusConfig? OpusConfig { get; init; }

    [JsonProperty("speech_rate")]
    public int? SpeechRate { get; init; }

    [JsonProperty("voice_id")]
    public string? VoiceId { get; init; }
}

/// <summary>
/// PCM 音频配置。
/// </summary>
public record PcmConfig
{
    [JsonProperty("sample_rate")]
    public int? SampleRate { get; init; }

    [JsonProperty("bits_per_sample")]
    public int? BitsPerSample { get; init; }
}

/// <summary>
/// Opus 音频配置。
/// </summary>
public record OpusConfig
{
    [JsonProperty("bitrate")]
    public int? Bitrate { get; init; }
}

/// <summary>
/// 中断配置。
/// </summary>
public record InterruptConfig
{
    [JsonProperty("type")]
    public string? Type { get; init; } = "client_interrupt";
}

/// <summary>
/// 轮次检测配置。
/// </summary>
public record TurnDetection
{
    [JsonProperty("type")]
    public string? Type { get; init; } = "client_interrupt";

    [JsonProperty("prefix_padding_ms")]
    public int? PrefixPaddingMs { get; init; }

    [JsonProperty("silence_duration_ms")]
    public int? SilenceDurationMs { get; init; }

    [JsonProperty("interrupt_config")]
    public InterruptConfig? InterruptConfig { get; init; }
}

/// <summary>
/// 聊天配置。
/// </summary>
public record ChatConfig
{
    [JsonProperty("user_id")]
    public string? UserId { get; init; }

    [JsonProperty("meta_data")]
    public IReadOnlyDictionary<string, string>? MetaData { get; init; }

    [JsonProperty("custom_variables")]
    public IReadOnlyDictionary<string, string>? CustomVariables { get; init; }

    [JsonProperty("auto_save_history")]
    public bool? AutoSaveHistory { get; init; }

    [JsonProperty("conversation_id")]
    public string? ConversationId { get; init; }
}

/// <summary>
/// ASR（自动语音识别）配置。
/// </summary>
public record AsrConfig
{
    [JsonProperty("language")]
    public string? Language { get; init; }

    [JsonProperty("enable_itn")]
    public bool? EnableItn { get; init; }

    [JsonProperty("enable_punctuation")]
    public bool? EnablePunctuation { get; init; }
}

/// <summary>
/// 限制配置。
/// </summary>
public record LimitConfig
{
    [JsonProperty("max_output_tokens")]
    public int? MaxOutputTokens { get; init; }
}

#endregion

#region 事件数据模型

/// <summary>
/// 聊天更新事件数据。
/// </summary>
public record ChatUpdateEventData
{
    [JsonProperty("input_audio")]
    public InputAudio? InputAudio { get; init; }

    [JsonProperty("output_audio")]
    public OutputAudio? OutputAudio { get; init; }

    [JsonProperty("chat_config")]
    public ChatConfig? ChatConfig { get; init; }

    [JsonProperty("turn_detection")]
    public TurnDetection? TurnDetection { get; init; }

    [JsonProperty("need_play_prologue")]
    public bool? NeedPlayPrologue { get; init; }

    [JsonProperty("prologue_content")]
    public string? PrologueContent { get; init; }

    [JsonProperty("asr_config")]
    public AsrConfig? AsrConfig { get; init; }
}

/// <summary>
/// 语音合成更新事件数据。
/// </summary>
public record SpeechUpdateEventData
{
    [JsonProperty("input_text")]
    public string? InputText { get; init; }

    [JsonProperty("voice_id")]
    public string? VoiceId { get; init; }

    [JsonProperty("output_format")]
    public string? OutputFormat { get; init; }

    [JsonProperty("speed")]
    public double? Speed { get; init; }
}

/// <summary>
/// 转录更新事件数据。
/// </summary>
public record TranscriptionsUpdateEventData
{
    [JsonProperty("language")]
    public string? Language { get; init; }

    [JsonProperty("model")]
    public string? Model { get; init; }
}

/// <summary>
/// 工具输出数据，用于提交工具输出。
/// </summary>
public record ToolOutputData
{
    [JsonProperty("tool_call_id")]
    public string? ToolCallId { get; init; }

    [JsonProperty("output")]
    public string? Output { get; init; }
}

/// <summary>
/// 提交工具输出事件数据。
/// </summary>
public record SubmitToolOutputsEventData
{
    [JsonProperty("tool_outputs")]
    public IReadOnlyList<ToolOutputData>? ToolOutputs { get; init; }
}

#endregion

#region 基础事件模型

/// <summary>
/// 事件中的详细信息。
/// </summary>
public record EventDetail
{
    [JsonProperty("code")]
    public int? Code { get; init; }

    [JsonProperty("msg")]
    public string? Message { get; init; }

    [JsonProperty("log_id")]
    public string? LogId { get; init; }
}

/// <summary>
/// WebSocket 事件的基类。
/// </summary>
public abstract record BaseWebSocketEvent
{
    [JsonProperty("id")]
    public string? Id { get; init; }

    [JsonProperty("event_type")]
    public abstract string EventType { get; }

    [JsonProperty("detail")]
    public EventDetail? Detail { get; init; }
}

#endregion

#region 下行事件（服务器 -> 客户端）

/// <summary>
/// 聊天创建事件。
/// </summary>
public record ChatCreatedEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.ChatCreated;
}

/// <summary>
/// 聊天更新事件。
/// </summary>
public record ChatUpdatedEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.ChatUpdated;
}

/// <summary>
/// 会话聊天创建事件。
/// </summary>
public record ConversationChatCreatedEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.ConversationChatCreated;

    [JsonProperty("data")]
    public ChatData? Data { get; init; }
}

/// <summary>
/// 会话聊天进行中事件。
/// </summary>
public record ConversationChatInProgressEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.ConversationChatInProgress;

    [JsonProperty("data")]
    public ChatData? Data { get; init; }
}

/// <summary>
/// 会话消息增量事件。
/// </summary>
public record ConversationMessageDeltaEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.ConversationMessageDelta;

    [JsonProperty("data")]
    public MessageData? Data { get; init; }
}

/// <summary>
/// 会话音频增量事件。
/// </summary>
public record ConversationAudioDeltaEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.ConversationAudioDelta;

    [JsonProperty("data")]
    public AudioData? Data { get; init; }
}

/// <summary>
/// 会话消息完成事件。
/// </summary>
public record ConversationMessageCompletedEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.ConversationMessageCompleted;

    [JsonProperty("data")]
    public MessageData? Data { get; init; }
}

/// <summary>
/// 会话音频完成事件。
/// </summary>
public record ConversationAudioCompletedEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.ConversationAudioCompleted;

    [JsonProperty("data")]
    public AudioData? Data { get; init; }
}

/// <summary>
/// 会话聊天完成事件。
/// </summary>
public record ConversationChatCompletedEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.ConversationChatCompleted;

    [JsonProperty("data")]
    public ChatData? Data { get; init; }
}

/// <summary>
/// 会话聊天失败事件。
/// </summary>
public record ConversationChatFailedEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.ConversationChatFailed;

    [JsonProperty("data")]
    public ChatData? Data { get; init; }
}

/// <summary>
/// 输入音频缓冲区完成事件。
/// </summary>
public record InputAudioBufferCompletedEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.InputAudioBufferCompleted;
}

/// <summary>
/// 输入音频缓冲区清除事件。
/// </summary>
public record InputAudioBufferClearedEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.InputAudioBufferCleared;
}

/// <summary>
/// 会话清除事件。
/// </summary>
public record ConversationClearedEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.ConversationCleared;
}

/// <summary>
/// 会话聊天取消事件。
/// </summary>
public record ConversationChatCanceledEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.ConversationChatCanceled;
}

/// <summary>
/// 会话音频转录更新事件。
/// </summary>
public record ConversationAudioTranscriptUpdateEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.ConversationAudioTranscriptUpdate;

    [JsonProperty("data")]
    public TranscriptData? Data { get; init; }
}

/// <summary>
/// 会话音频转录完成事件。
/// </summary>
public record ConversationAudioTranscriptCompletedEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.ConversationAudioTranscriptCompleted;

    [JsonProperty("data")]
    public TranscriptData? Data { get; init; }
}

/// <summary>
/// 会话聊天需要操作事件。
/// </summary>
public record ConversationChatRequiresActionEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.ConversationChatRequiresAction;

    [JsonProperty("data")]
    public RequiresActionData? Data { get; init; }
}

/// <summary>
/// 输入音频缓冲区语音开始事件。
/// </summary>
public record InputAudioBufferSpeechStartedEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.InputAudioBufferSpeechStarted;

    [JsonProperty("data")]
    public SpeechDetectionData? Data { get; init; }
}

/// <summary>
/// 输入音频缓冲区语音结束事件。
/// </summary>
public record InputAudioBufferSpeechStoppedEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.InputAudioBufferSpeechStopped;

    [JsonProperty("data")]
    public SpeechDetectionData? Data { get; init; }
}

/// <summary>
/// 会话音频句子开始事件。
/// </summary>
public record ConversationAudioSentenceStartEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.ConversationAudioSentenceStart;

    [JsonProperty("data")]
    public SentenceData? Data { get; init; }
}

/// <summary>
/// 错误事件。
/// </summary>
public record ErrorEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.Error;

    [JsonProperty("data")]
    public ErrorData? Data { get; init; }
}

/// <summary>
/// 语音合成创建事件。
/// </summary>
public record SpeechCreatedEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.SpeechCreated;
}

/// <summary>
/// 语音合成更新事件。
/// </summary>
public record SpeechUpdatedEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.SpeechUpdated;
}

/// <summary>
/// 语音合成音频更新事件。
/// </summary>
public record SpeechAudioUpdateEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.SpeechAudioUpdate;

    [JsonProperty("data")]
    public AudioData? Data { get; init; }
}

/// <summary>
/// 语音合成音频完成事件。
/// </summary>
public record SpeechAudioCompletedEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.SpeechAudioCompleted;
}

/// <summary>
/// 转录创建事件。
/// </summary>
public record TranscriptionsCreatedEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.TranscriptionsCreated;
}

/// <summary>
/// 转录更新事件。
/// </summary>
public record TranscriptionsUpdatedEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.TranscriptionsUpdated;
}

/// <summary>
/// 转录消息更新事件。
/// </summary>
public record TranscriptionsMessageUpdateEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.TranscriptionsMessageUpdate;

    [JsonProperty("data")]
    public TranscriptData? Data { get; init; }
}

/// <summary>
/// 转录消息完成事件。
/// </summary>
public record TranscriptionsMessageCompletedEvent : BaseWebSocketEvent
{
    public override string EventType => WebSocketEventTypes.TranscriptionsMessageCompleted;

    [JsonProperty("data")]
    public TranscriptData? Data { get; init; }
}

#endregion

#region 事件数据模型

/// <summary>
/// 事件中的聊天数据。
/// </summary>
public record ChatData
{
    [JsonProperty("id")]
    public string? Id { get; init; }

    [JsonProperty("conversation_id")]
    public string? ConversationId { get; init; }

    [JsonProperty("bot_id")]
    public string? BotId { get; init; }

    [JsonProperty("status")]
    public string? Status { get; init; }

    [JsonProperty("usage")]
    public ChatUsage? Usage { get; init; }

    [JsonProperty("created_at")]
    public long? CreatedAt { get; init; }

    [JsonProperty("completed_at")]
    public long? CompletedAt { get; init; }
}

/// <summary>
/// 聊天用量信息。
/// </summary>
public record ChatUsage
{
    [JsonProperty("token_count")]
    public int? TokenCount { get; init; }

    [JsonProperty("input_tokens")]
    public int? InputTokens { get; init; }

    [JsonProperty("output_tokens")]
    public int? OutputTokens { get; init; }
}

/// <summary>
/// 事件中的消息数据。
/// </summary>
public record MessageData
{
    [JsonProperty("id")]
    public string? Id { get; init; }

    [JsonProperty("role")]
    public string? Role { get; init; }

    [JsonProperty("type")]
    public string? Type { get; init; }

    [JsonProperty("content")]
    public string? Content { get; init; }

    [JsonProperty("content_type")]
    public string? ContentType { get; init; }

    [JsonProperty("conversation_id")]
    public string? ConversationId { get; init; }

    [JsonProperty("chat_id")]
    public string? ChatId { get; init; }

    [JsonProperty("created_at")]
    public long? CreatedAt { get; init; }
}

/// <summary>
/// 事件中的音频数据。
/// </summary>
public record AudioData
{
    [JsonProperty("audio")]
    public string? Audio { get; init; }

    [JsonProperty("audio_base64")]
    public string? AudioBase64 { get; init; }

    /// <summary>
    /// 获取音频字节数组。
    /// </summary>
    [JsonIgnore]
    public byte[]? AudioBytes => AudioBase64 != null ? Convert.FromBase64String(AudioBase64) : null;
}

/// <summary>
/// 事件中的转录数据。
/// </summary>
public record TranscriptData
{
    [JsonProperty("text")]
    public string? Text { get; init; }
}

/// <summary>
/// 需要操作的数据。
/// </summary>
public record RequiresActionData
{
    [JsonProperty("tool_calls")]
    public IReadOnlyList<ToolCall>? ToolCalls { get; init; }
}

/// <summary>
/// 工具调用信息。
/// </summary>
public record ToolCall
{
    [JsonProperty("id")]
    public string? Id { get; init; }

    [JsonProperty("type")]
    public string? Type { get; init; }

    [JsonProperty("function")]
    public FunctionCall? Function { get; init; }
}

/// <summary>
/// 函数调用信息。
/// </summary>
public record FunctionCall
{
    [JsonProperty("name")]
    public string? Name { get; init; }

    [JsonProperty("arguments")]
    public string? Arguments { get; init; }
}

/// <summary>
/// 语音检测数据。
/// </summary>
public record SpeechDetectionData
{
    [JsonProperty("audio_start_ms")]
    public long? AudioStartMs { get; init; }

    [JsonProperty("audio_end_ms")]
    public long? AudioEndMs { get; init; }
}

/// <summary>
/// 句子数据。
/// </summary>
public record SentenceData
{
    [JsonProperty("sentence_id")]
    public string? SentenceId { get; init; }

    [JsonProperty("text")]
    public string? Text { get; init; }
}

/// <summary>
/// 错误数据。
/// </summary>
public record ErrorData
{
    [JsonProperty("code")]
    public int? Code { get; init; }

    [JsonProperty("msg")]
    public string? Message { get; init; }
}

#endregion
