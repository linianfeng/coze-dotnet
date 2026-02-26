using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using Coze.Sdk.Authentication;
using Coze.Sdk.Models.Chat;
using Coze.Sdk.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Coze.Sdk.WebSocket;

/// <summary>
/// 聊天 WebSocket 客户端选项。
/// 对应 Java SDK 中的 WebsocketsChatCreateReq.java。
/// </summary>
public record ChatWebSocketOptions
{
    /// <summary>
    /// 获取机器人 ID。
    /// </summary>
    public required string BotId { get; init; }

    /// <summary>
    /// 获取连接超时时间（秒）。
    /// </summary>
    public int? ConnectTimeout { get; init; }

    /// <summary>
    /// 获取读取超时时间（秒）。
    /// </summary>
    public int? ReadTimeout { get; init; }

    /// <summary>
    /// 获取写入超时时间（秒）。
    /// </summary>
    public int? WriteTimeout { get; init; }
}

/// <summary>
/// 聊天 WebSocket 回调处理器。
/// 对应 Java SDK 中的 WebsocketsChatCallbackHandler.java。
/// </summary>
public abstract class ChatWebSocketCallbackHandler
{
    /// <summary>
    /// 当聊天创建时调用。
    /// </summary>
    public virtual Task OnChatCreatedAsync(ChatWebSocketClient client, ChatCreatedEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当聊天更新时调用。
    /// </summary>
    public virtual Task OnChatUpdatedAsync(ChatWebSocketClient client, ChatUpdatedEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当会话聊天创建时调用。
    /// </summary>
    public virtual Task OnConversationChatCreatedAsync(ChatWebSocketClient client, ConversationChatCreatedEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当会话聊天进行中时调用。
    /// </summary>
    public virtual Task OnConversationChatInProgressAsync(ChatWebSocketClient client, ConversationChatInProgressEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当收到会话消息增量时调用。
    /// </summary>
    public virtual Task OnConversationMessageDeltaAsync(ChatWebSocketClient client, ConversationMessageDeltaEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当收到会话音频增量时调用。
    /// </summary>
    public virtual Task OnConversationAudioDeltaAsync(ChatWebSocketClient client, ConversationAudioDeltaEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当会话消息完成时调用。
    /// </summary>
    public virtual Task OnConversationMessageCompletedAsync(ChatWebSocketClient client, ConversationMessageCompletedEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当会话音频完成时调用。
    /// </summary>
    public virtual Task OnConversationAudioCompletedAsync(ChatWebSocketClient client, ConversationAudioCompletedEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当会话聊天完成时调用。
    /// </summary>
    public virtual Task OnConversationChatCompletedAsync(ChatWebSocketClient client, ConversationChatCompletedEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当会话聊天失败时调用。
    /// </summary>
    public virtual Task OnConversationChatFailedAsync(ChatWebSocketClient client, ConversationChatFailedEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当输入音频缓冲区完成时调用。
    /// </summary>
    public virtual Task OnInputAudioBufferCompletedAsync(ChatWebSocketClient client, InputAudioBufferCompletedEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当输入音频缓冲区清除时调用。
    /// </summary>
    public virtual Task OnInputAudioBufferClearedAsync(ChatWebSocketClient client, InputAudioBufferClearedEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当会话清除时调用。
    /// </summary>
    public virtual Task OnConversationClearedAsync(ChatWebSocketClient client, ConversationClearedEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当会话聊天取消时调用。
    /// </summary>
    public virtual Task OnConversationChatCanceledAsync(ChatWebSocketClient client, ConversationChatCanceledEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当会话音频转录更新时调用。
    /// </summary>
    public virtual Task OnConversationAudioTranscriptUpdateAsync(ChatWebSocketClient client, ConversationAudioTranscriptUpdateEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当会话音频转录完成时调用。
    /// </summary>
    public virtual Task OnConversationAudioTranscriptCompletedAsync(ChatWebSocketClient client, ConversationAudioTranscriptCompletedEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当会话聊天需要操作时调用。
    /// </summary>
    public virtual Task OnConversationChatRequiresActionAsync(ChatWebSocketClient client, ConversationChatRequiresActionEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当输入音频缓冲区语音开始时调用。
    /// </summary>
    public virtual Task OnInputAudioBufferSpeechStartedAsync(ChatWebSocketClient client, InputAudioBufferSpeechStartedEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当输入音频缓冲区语音结束时调用。
    /// </summary>
    public virtual Task OnInputAudioBufferSpeechStoppedAsync(ChatWebSocketClient client, InputAudioBufferSpeechStoppedEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当会话音频句子开始时调用。
    /// </summary>
    public virtual Task OnConversationAudioSentenceStartAsync(ChatWebSocketClient client, ConversationAudioSentenceStartEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当发生错误时调用。
    /// </summary>
    public virtual Task OnErrorAsync(ChatWebSocketClient client, ErrorEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当连接正在关闭时调用。
    /// </summary>
    public virtual Task OnClosingAsync(ChatWebSocketClient client, int code, string reason) => Task.CompletedTask;

    /// <summary>
    /// 当连接已关闭时调用。
    /// </summary>
    public virtual Task OnClosedAsync(ChatWebSocketClient client, int code, string reason) => Task.CompletedTask;

    /// <summary>
    /// 当连接失败时调用。
    /// </summary>
    public virtual Task OnFailureAsync(ChatWebSocketClient client, Exception exception) => Task.CompletedTask;

    /// <summary>
    /// 当发生客户端异常时调用。
    /// </summary>
    public virtual Task OnClientExceptionAsync(ChatWebSocketClient client, Exception exception) => Task.CompletedTask;
}

/// <summary>
/// 聊天 WebSocket 客户端。
/// 对应 Java SDK 中的 WebsocketsChatClient.java。
/// </summary>
public class ChatWebSocketClient : BaseWebSocketClient
{
    private const string ChatPath = "/v1/chat";
    private readonly ChatWebSocketCallbackHandler _handler;
    private readonly string _botId;

    internal ChatWebSocketClient(
        string baseUrl,
        Auth auth,
        string botId,
        ChatWebSocketCallbackHandler handler,
        ILogger? logger = null)
        : base(baseUrl, auth, logger)
    {
        _botId = botId ?? throw new ArgumentNullException(nameof(botId));
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    /// <summary>
    /// 连接到聊天 WebSocket。
    /// </summary>
    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        var wsUrl = _baseUrl.Replace("https://", "wss://").Replace("http://", "ws://");
        var uri = new Uri($"{wsUrl}{ChatPath}?bot_id={_botId}");
        await ConnectAsync(uri, cancellationToken);
    }

    /// <summary>
    /// 发送聊天更新事件。
    /// </summary>
    public async Task ChatUpdateAsync(ChatUpdateEventData data, CancellationToken cancellationToken = default)
    {
        var evt = new ChatUpdateEvent { Data = data };
        await SendEventAsync(evt, cancellationToken);
    }

    /// <summary>
    /// 取消会话聊天。
    /// </summary>
    public async Task ConversationChatCancelAsync(CancellationToken cancellationToken = default)
    {
        await SendEventAsync(new ConversationChatCancelEvent(), cancellationToken);
    }

    /// <summary>
    /// 提交工具输出。
    /// </summary>
    public async Task ConversationChatSubmitToolOutputsAsync(
        SubmitToolOutputsEventData data,
        CancellationToken cancellationToken = default)
    {
        var evt = new ConversationChatSubmitToolOutputsEvent { Data = data };
        await SendEventAsync(evt, cancellationToken);
    }

    /// <summary>
    /// 清除会话。
    /// </summary>
    public async Task ConversationClearAsync(CancellationToken cancellationToken = default)
    {
        await SendEventAsync(new ConversationClearEvent(), cancellationToken);
    }

    /// <summary>
    /// 创建会话消息。
    /// </summary>
    public async Task ConversationMessageCreateAsync(
        MessageData message,
        CancellationToken cancellationToken = default)
    {
        var evt = new ConversationMessageCreateEvent { Data = message };
        await SendEventAsync(evt, cancellationToken);
    }

    /// <summary>
    /// 向输入缓冲区追加音频（字符串格式）。
    /// </summary>
    public async Task InputAudioBufferAppendAsync(string data, CancellationToken cancellationToken = default)
    {
        var bytes = Encoding.UTF8.GetBytes(data);
        await InputAudioBufferAppendAsync(bytes, cancellationToken);
    }

    /// <summary>
    /// 向输入缓冲区追加音频（字节数组格式）。
    /// </summary>
    public async Task InputAudioBufferAppendAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        var base64 = Convert.ToBase64String(data);
        var evt = new InputAudioBufferAppendEvent { Data = base64 };
        await SendEventAsync(evt, cancellationToken);
    }

    /// <summary>
    /// 清除输入音频缓冲区。
    /// </summary>
    public async Task InputAudioBufferClearAsync(CancellationToken cancellationToken = default)
    {
        await SendEventAsync(new InputAudioBufferClearEvent(), cancellationToken);
    }

    /// <summary>
    /// 完成输入音频缓冲区。
    /// </summary>
    public async Task InputAudioBufferCompleteAsync(CancellationToken cancellationToken = default)
    {
        await SendEventAsync(new InputAudioBufferCompleteEvent(), cancellationToken);
    }

    /// <summary>
    /// 处理接收到的消息。
    /// </summary>
    protected override async Task HandleMessageAsync(string message)
    {
        var eventType = ParseEventType(message);
        if (eventType == null)
        {
            _logger?.LogError("Missing event_type field in event: {Message}", message);
            await _handler.OnClientExceptionAsync(this, new InvalidOperationException("Missing event_type field"));
            return;
        }

        try
        {
            await HandleEventTypeAsync(eventType, message);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling event type: {EventType}", eventType);
            await _handler.OnClientExceptionAsync(this, ex);
        }
    }

    private async Task HandleEventTypeAsync(string eventType, string message)
    {
        switch (eventType)
        {
            case WebSocketEventTypes.ChatCreated:
                await _handler.OnChatCreatedAsync(this, DeserializeEvent<ChatCreatedEvent>(message));
                break;

            case WebSocketEventTypes.ChatUpdated:
                await _handler.OnChatUpdatedAsync(this, DeserializeEvent<ChatUpdatedEvent>(message));
                break;

            case WebSocketEventTypes.ConversationChatCreated:
                await _handler.OnConversationChatCreatedAsync(this, DeserializeEvent<ConversationChatCreatedEvent>(message));
                break;

            case WebSocketEventTypes.ConversationChatInProgress:
                await _handler.OnConversationChatInProgressAsync(this, DeserializeEvent<ConversationChatInProgressEvent>(message));
                break;

            case WebSocketEventTypes.ConversationMessageDelta:
                await _handler.OnConversationMessageDeltaAsync(this, DeserializeEvent<ConversationMessageDeltaEvent>(message));
                break;

            case WebSocketEventTypes.ConversationAudioDelta:
                await _handler.OnConversationAudioDeltaAsync(this, DeserializeEvent<ConversationAudioDeltaEvent>(message));
                break;

            case WebSocketEventTypes.ConversationMessageCompleted:
                await _handler.OnConversationMessageCompletedAsync(this, DeserializeEvent<ConversationMessageCompletedEvent>(message));
                break;

            case WebSocketEventTypes.ConversationAudioCompleted:
                await _handler.OnConversationAudioCompletedAsync(this, DeserializeEvent<ConversationAudioCompletedEvent>(message));
                break;

            case WebSocketEventTypes.ConversationChatCompleted:
                await _handler.OnConversationChatCompletedAsync(this, DeserializeEvent<ConversationChatCompletedEvent>(message));
                break;

            case WebSocketEventTypes.ConversationChatFailed:
                await _handler.OnConversationChatFailedAsync(this, DeserializeEvent<ConversationChatFailedEvent>(message));
                break;

            case WebSocketEventTypes.InputAudioBufferCompleted:
                await _handler.OnInputAudioBufferCompletedAsync(this, DeserializeEvent<InputAudioBufferCompletedEvent>(message));
                break;

            case WebSocketEventTypes.InputAudioBufferCleared:
                await _handler.OnInputAudioBufferClearedAsync(this, DeserializeEvent<InputAudioBufferClearedEvent>(message));
                break;

            case WebSocketEventTypes.ConversationCleared:
                await _handler.OnConversationClearedAsync(this, DeserializeEvent<ConversationClearedEvent>(message));
                break;

            case WebSocketEventTypes.ConversationChatCanceled:
                await _handler.OnConversationChatCanceledAsync(this, DeserializeEvent<ConversationChatCanceledEvent>(message));
                break;

            case WebSocketEventTypes.ConversationAudioTranscriptUpdate:
                await _handler.OnConversationAudioTranscriptUpdateAsync(this, DeserializeEvent<ConversationAudioTranscriptUpdateEvent>(message));
                break;

            case WebSocketEventTypes.ConversationAudioTranscriptCompleted:
                await _handler.OnConversationAudioTranscriptCompletedAsync(this, DeserializeEvent<ConversationAudioTranscriptCompletedEvent>(message));
                break;

            case WebSocketEventTypes.ConversationChatRequiresAction:
                await _handler.OnConversationChatRequiresActionAsync(this, DeserializeEvent<ConversationChatRequiresActionEvent>(message));
                break;

            case WebSocketEventTypes.InputAudioBufferSpeechStarted:
                await _handler.OnInputAudioBufferSpeechStartedAsync(this, DeserializeEvent<InputAudioBufferSpeechStartedEvent>(message));
                break;

            case WebSocketEventTypes.InputAudioBufferSpeechStopped:
                await _handler.OnInputAudioBufferSpeechStoppedAsync(this, DeserializeEvent<InputAudioBufferSpeechStoppedEvent>(message));
                break;

            case WebSocketEventTypes.ConversationAudioSentenceStart:
                await _handler.OnConversationAudioSentenceStartAsync(this, DeserializeEvent<ConversationAudioSentenceStartEvent>(message));
                break;

            case WebSocketEventTypes.Error:
                await _handler.OnErrorAsync(this, DeserializeEvent<ErrorEvent>(message));
                break;

            default:
                _logger?.LogWarning("Unknown event type: {EventType}, message: {Message}", eventType, message);
                break;
        }
    }

    private static T DeserializeEvent<T>(string message) where T : class
    {
        return JsonHelper.DeserializeObject<T>(message)
            ?? throw new InvalidOperationException($"Failed to deserialize {typeof(T).Name}");
    }

    protected override async Task OnClosingAsync(int code, string reason)
    {
        await _handler.OnClosingAsync(this, code, reason);
    }

    protected override async Task OnClosedAsync(int code, string reason)
    {
        await _handler.OnClosedAsync(this, code, reason);
    }

    protected override async Task OnFailureAsync(Exception exception)
    {
        await _handler.OnFailureAsync(this, exception);
    }

    protected override async Task OnClientExceptionAsync(Exception exception)
    {
        await _handler.OnClientExceptionAsync(this, exception);
    }
}
