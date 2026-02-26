namespace Coze.Sdk.WebSocket;

/// <summary>
/// WebSocket 事件类型常量。
/// 对应 Java SDK 中的 EventType.java。
/// </summary>
public static class WebSocketEventTypes
{
    // 通用
    public const string ClientError = "client_error";
    public const string Closed = "closed";

    // 错误
    public const string Error = "error";

    // v1/audio/speech - 请求
    public const string InputTextBufferAppend = "input_text_buffer.append";
    public const string InputTextBufferComplete = "input_text_buffer.complete";
    public const string SpeechUpdate = "speech.update";

    // v1/audio/speech - 响应
    public const string SpeechUpdated = "speech.updated";
    public const string SpeechCreated = "speech.created";
    public const string InputTextBufferCompleted = "input_text_buffer.completed";
    public const string SpeechAudioUpdate = "speech.audio.update";
    public const string SpeechAudioCompleted = "speech.audio.completed";

    // v1/audio/transcriptions - 请求
    public const string InputAudioBufferAppend = "input_audio_buffer.append";
    public const string InputAudioBufferComplete = "input_audio_buffer.complete";
    public const string TranscriptionsUpdate = "transcriptions.update";

    // v1/audio/transcriptions - 响应
    public const string TranscriptionsCreated = "transcriptions.created";
    public const string TranscriptionsUpdated = "transcriptions.updated";
    public const string InputAudioBufferCompleted = "input_audio_buffer.completed";
    public const string TranscriptionsMessageUpdate = "transcriptions.message.update";
    public const string TranscriptionsMessageCompleted = "transcriptions.message.completed";

    // v1/chat - 请求
    public const string ChatUpdate = "chat.update";
    public const string ConversationChatSubmitToolOutputs = "conversation.chat.submit_tool_outputs";
    public const string InputAudioBufferClear = "input_audio_buffer.clear";
    public const string ConversationMessageCreate = "conversation.message.create";
    public const string ConversationClear = "conversation.clear";
    public const string ConversationChatCancel = "conversation.chat.cancel";

    // v1/chat - 响应
    public const string ChatCreated = "chat.created";
    public const string ChatUpdated = "chat.updated";
    public const string ConversationChatCreated = "conversation.chat.created";
    public const string ConversationChatInProgress = "conversation.chat.in_progress";
    public const string ConversationMessageDelta = "conversation.message.delta";
    public const string ConversationAudioDelta = "conversation.audio.delta";
    public const string ConversationMessageCompleted = "conversation.message.completed";
    public const string ConversationAudioCompleted = "conversation.audio.completed";
    public const string ConversationChatCompleted = "conversation.chat.completed";
    public const string ConversationChatFailed = "conversation.chat.failed";
    public const string InputAudioBufferCleared = "input_audio_buffer.cleared";
    public const string ConversationCleared = "conversation.cleared";
    public const string ConversationChatCanceled = "conversation.chat.canceled";
    public const string ConversationAudioTranscriptUpdate = "conversation.audio_transcript.update";
    public const string ConversationAudioTranscriptCompleted = "conversation.audio_transcript.completed";
    public const string ConversationChatRequiresAction = "conversation.chat.requires_action";
    public const string InputAudioBufferSpeechStarted = "input_audio_buffer.speech_started";
    public const string InputAudioBufferSpeechStopped = "input_audio_buffer.speech_stopped";
    public const string ConversationAudioSentenceStart = "conversation.audio.sentence_start";
}
