using System.Net.WebSockets;
using System.Text;
using Coze.Sdk.Authentication;
using Coze.Sdk.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Coze.Sdk.WebSocket;

/// <summary>
/// 转录 WebSocket 回调处理器。
/// 对应 Java SDK 中的 WebsocketsAudioTranscriptionsCallbackHandler.java。
/// </summary>
public abstract class TranscriptionsWebSocketCallbackHandler
{
    /// <summary>
    /// 当转录创建时调用。
    /// </summary>
    public virtual Task OnTranscriptionsCreatedAsync(TranscriptionsWebSocketClient client, TranscriptionsCreatedEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当转录更新时调用。
    /// </summary>
    public virtual Task OnTranscriptionsUpdatedAsync(TranscriptionsWebSocketClient client, TranscriptionsUpdatedEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当转录消息更新时调用。
    /// </summary>
    public virtual Task OnTranscriptionsMessageUpdateAsync(TranscriptionsWebSocketClient client, TranscriptionsMessageUpdateEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当转录消息完成时调用。
    /// </summary>
    public virtual Task OnTranscriptionsMessageCompletedAsync(TranscriptionsWebSocketClient client, TranscriptionsMessageCompletedEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当输入音频缓冲区完成时调用。
    /// </summary>
    public virtual Task OnInputAudioBufferCompletedAsync(TranscriptionsWebSocketClient client, InputAudioBufferCompletedEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当发生错误时调用。
    /// </summary>
    public virtual Task OnErrorAsync(TranscriptionsWebSocketClient client, ErrorEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当连接正在关闭时调用。
    /// </summary>
    public virtual Task OnClosingAsync(TranscriptionsWebSocketClient client, int code, string reason) => Task.CompletedTask;

    /// <summary>
    /// 当连接已关闭时调用。
    /// </summary>
    public virtual Task OnClosedAsync(TranscriptionsWebSocketClient client, int code, string reason) => Task.CompletedTask;

    /// <summary>
    /// 当连接失败时调用。
    /// </summary>
    public virtual Task OnFailureAsync(TranscriptionsWebSocketClient client, Exception exception) => Task.CompletedTask;

    /// <summary>
    /// 当发生客户端异常时调用。
    /// </summary>
    public virtual Task OnClientExceptionAsync(TranscriptionsWebSocketClient client, Exception exception) => Task.CompletedTask;
}

/// <summary>
/// 转录 WebSocket 选项。
/// </summary>
public record TranscriptionsWebSocketOptions
{
    /// <summary>
    /// 获取语言。
    /// </summary>
    public string? Language { get; init; }

    /// <summary>
    /// 获取模型。
    /// </summary>
    public string? Model { get; init; }
}

/// <summary>
/// 转录 WebSocket 客户端，用于语音转文本。
/// 对应 Java SDK 中的 WebsocketsAudioTranscriptionsClient.java。
/// </summary>
public class TranscriptionsWebSocketClient : BaseWebSocketClient
{
    private const string TranscriptionsPath = "/v1/audio/transcriptions";
    private readonly TranscriptionsWebSocketCallbackHandler _handler;

    internal TranscriptionsWebSocketClient(
        string baseUrl,
        Auth auth,
        TranscriptionsWebSocketCallbackHandler handler,
        ILogger? logger = null)
        : base(baseUrl, auth, logger)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    /// <summary>
    /// 连接到转录 WebSocket。
    /// </summary>
    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        var wsUrl = _baseUrl.Replace("https://", "wss://").Replace("http://", "ws://");
        var uri = new Uri($"{wsUrl}{TranscriptionsPath}");
        await ConnectAsync(uri, cancellationToken);
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
    /// 完成输入音频缓冲区。
    /// </summary>
    public async Task InputAudioBufferCompleteAsync(CancellationToken cancellationToken = default)
    {
        await SendEventAsync(new InputAudioBufferCompleteEvent(), cancellationToken);
    }

    /// <summary>
    /// 更新转录配置。
    /// </summary>
    public async Task TranscriptionsUpdateAsync(TranscriptionsUpdateEventData data, CancellationToken cancellationToken = default)
    {
        var evt = new TranscriptionsUpdateEvent { Data = data };
        await SendEventAsync(evt, cancellationToken);
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
            switch (eventType)
            {
                case WebSocketEventTypes.TranscriptionsCreated:
                    await _handler.OnTranscriptionsCreatedAsync(this, DeserializeEvent<TranscriptionsCreatedEvent>(message));
                    break;

                case WebSocketEventTypes.TranscriptionsUpdated:
                    await _handler.OnTranscriptionsUpdatedAsync(this, DeserializeEvent<TranscriptionsUpdatedEvent>(message));
                    break;

                case WebSocketEventTypes.TranscriptionsMessageUpdate:
                    await _handler.OnTranscriptionsMessageUpdateAsync(this, DeserializeEvent<TranscriptionsMessageUpdateEvent>(message));
                    break;

                case WebSocketEventTypes.TranscriptionsMessageCompleted:
                    await _handler.OnTranscriptionsMessageCompletedAsync(this, DeserializeEvent<TranscriptionsMessageCompletedEvent>(message));
                    break;

                case WebSocketEventTypes.InputAudioBufferCompleted:
                    await _handler.OnInputAudioBufferCompletedAsync(this, DeserializeEvent<InputAudioBufferCompletedEvent>(message));
                    break;

                case WebSocketEventTypes.Error:
                    await _handler.OnErrorAsync(this, DeserializeEvent<ErrorEvent>(message));
                    break;

                default:
                    _logger?.LogWarning("Unknown event type: {EventType}, message: {Message}", eventType, message);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error handling event type: {EventType}", eventType);
            await _handler.OnClientExceptionAsync(this, ex);
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
