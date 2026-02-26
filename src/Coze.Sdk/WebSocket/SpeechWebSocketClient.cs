using System.Net.WebSockets;
using System.Text;
using Coze.Sdk.Authentication;
using Coze.Sdk.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Coze.Sdk.WebSocket;

/// <summary>
/// 语音合成 WebSocket 回调处理器。
/// 对应 Java SDK 中的 WebsocketsAudioSpeechCallbackHandler.java。
/// </summary>
public abstract class SpeechWebSocketCallbackHandler
{
    /// <summary>
    /// 当语音合成创建时调用。
    /// </summary>
    public virtual Task OnSpeechCreatedAsync(SpeechWebSocketClient client, SpeechCreatedEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当语音合成更新时调用。
    /// </summary>
    public virtual Task OnSpeechUpdatedAsync(SpeechWebSocketClient client, SpeechUpdatedEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当语音合成音频更新时调用。
    /// </summary>
    public virtual Task OnSpeechAudioUpdateAsync(SpeechWebSocketClient client, SpeechAudioUpdateEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当语音合成音频完成时调用。
    /// </summary>
    public virtual Task OnSpeechAudioCompletedAsync(SpeechWebSocketClient client, SpeechAudioCompletedEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当输入文本缓冲区完成时调用。
    /// </summary>
    public virtual Task OnInputTextBufferCompletedAsync(SpeechWebSocketClient client, InputTextBufferCompletedEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当发生错误时调用。
    /// </summary>
    public virtual Task OnErrorAsync(SpeechWebSocketClient client, ErrorEvent evt) => Task.CompletedTask;

    /// <summary>
    /// 当连接正在关闭时调用。
    /// </summary>
    public virtual Task OnClosingAsync(SpeechWebSocketClient client, int code, string reason) => Task.CompletedTask;

    /// <summary>
    /// 当连接已关闭时调用。
    /// </summary>
    public virtual Task OnClosedAsync(SpeechWebSocketClient client, int code, string reason) => Task.CompletedTask;

    /// <summary>
    /// 当连接失败时调用。
    /// </summary>
    public virtual Task OnFailureAsync(SpeechWebSocketClient client, Exception exception) => Task.CompletedTask;

    /// <summary>
    /// 当发生客户端异常时调用。
    /// </summary>
    public virtual Task OnClientExceptionAsync(SpeechWebSocketClient client, Exception exception) => Task.CompletedTask;
}

/// <summary>
/// 语音合成 WebSocket 客户端选项。
/// </summary>
public record SpeechWebSocketOptions
{
    /// <summary>
    /// 获取输入文本。
    /// </summary>
    public string? InputText { get; init; }

    /// <summary>
    /// 获取语音 ID。
    /// </summary>
    public string? VoiceId { get; init; }

    /// <summary>
    /// 获取输出格式。
    /// </summary>
    public string? OutputFormat { get; init; }

    /// <summary>
    /// 获取语速。
    /// </summary>
    public double? Speed { get; init; }
}

/// <summary>
/// 语音合成 WebSocket 客户端，用于文本转语音。
/// 对应 Java SDK 中的 WebsocketsAudioSpeechClient.java。
/// </summary>
public class SpeechWebSocketClient : BaseWebSocketClient
{
    private const string SpeechPath = "/v1/audio/speech";
    private readonly SpeechWebSocketCallbackHandler _handler;

    internal SpeechWebSocketClient(
        string baseUrl,
        Auth auth,
        SpeechWebSocketCallbackHandler handler,
        ILogger? logger = null)
        : base(baseUrl, auth, logger)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    /// <summary>
    /// 连接到语音合成 WebSocket。
    /// </summary>
    public async Task ConnectAsync(CancellationToken cancellationToken = default)
    {
        var wsUrl = _baseUrl.Replace("https://", "wss://").Replace("http://", "ws://");
        var uri = new Uri($"{wsUrl}{SpeechPath}");
        await ConnectAsync(uri, cancellationToken);
    }

    /// <summary>
    /// 向输入缓冲区追加文本。
    /// </summary>
    public async Task InputTextBufferAppendAsync(string text, CancellationToken cancellationToken = default)
    {
        var evt = new InputTextBufferAppendEvent { Data = text };
        await SendEventAsync(evt, cancellationToken);
    }

    /// <summary>
    /// 完成输入文本缓冲区。
    /// </summary>
    public async Task InputTextBufferCompleteAsync(CancellationToken cancellationToken = default)
    {
        await SendEventAsync(new InputTextBufferCompleteEvent(), cancellationToken);
    }

    /// <summary>
    /// 更新语音合成配置。
    /// </summary>
    public async Task SpeechUpdateAsync(SpeechUpdateEventData data, CancellationToken cancellationToken = default)
    {
        var evt = new SpeechUpdateEvent { Data = data };
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
                case WebSocketEventTypes.SpeechCreated:
                    await _handler.OnSpeechCreatedAsync(this, DeserializeEvent<SpeechCreatedEvent>(message));
                    break;

                case WebSocketEventTypes.SpeechUpdated:
                    await _handler.OnSpeechUpdatedAsync(this, DeserializeEvent<SpeechUpdatedEvent>(message));
                    break;

                case WebSocketEventTypes.SpeechAudioUpdate:
                    await _handler.OnSpeechAudioUpdateAsync(this, DeserializeEvent<SpeechAudioUpdateEvent>(message));
                    break;

                case WebSocketEventTypes.SpeechAudioCompleted:
                    await _handler.OnSpeechAudioCompletedAsync(this, DeserializeEvent<SpeechAudioCompletedEvent>(message));
                    break;

                case WebSocketEventTypes.InputTextBufferCompleted:
                    await _handler.OnInputTextBufferCompletedAsync(this, DeserializeEvent<InputTextBufferCompletedEvent>(message));
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

/// <summary>
/// 输入文本缓冲区追加事件。
/// </summary>
internal record InputTextBufferAppendEvent
{
    [JsonProperty("event_type")]
    public string EventType => WebSocketEventTypes.InputTextBufferAppend;

    [JsonProperty("data")]
    public string? Data { get; init; }
}

/// <summary>
/// 输入文本缓冲区完成事件。
/// </summary>
internal record InputTextBufferCompleteEvent
{
    [JsonProperty("event_type")]
    public string EventType => WebSocketEventTypes.InputTextBufferComplete;
}

/// <summary>
/// 输入文本缓冲区已完成事件（来自服务器）。
/// </summary>
public record InputTextBufferCompletedEvent
{
    [JsonProperty("event_type")]
    public string EventType => WebSocketEventTypes.InputTextBufferCompleted;

    [JsonProperty("id")]
    public string? Id { get; init; }

    [JsonProperty("detail")]
    public EventDetail? Detail { get; init; }
}
