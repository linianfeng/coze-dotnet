using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using Coze.Sdk.Authentication;
using Coze.Sdk.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Coze.Sdk.WebSocket;

/// <summary>
/// WebSocket 客户端基类实现。
/// 对应 Java SDK 中的 BaseWebsocketsClient.java。
/// </summary>
public abstract class BaseWebSocketClient : IAsyncDisposable
{
    protected readonly ClientWebSocket _webSocket;
    protected readonly ILogger? _logger;
    protected readonly CancellationTokenSource _cancellationTokenSource;
    protected readonly string _baseUrl;
    protected readonly Auth _auth;
    protected bool _disposed;
    protected bool _isConnected;

    private const int ReceiveBufferSize = 8192;
    private readonly SemaphoreSlim _sendLock = new(1, 1);

    protected BaseWebSocketClient(string baseUrl, Auth auth, ILogger? logger = null)
    {
        _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
        _auth = auth ?? throw new ArgumentNullException(nameof(auth));
        _logger = logger;
        _webSocket = new ClientWebSocket();
        _cancellationTokenSource = new CancellationTokenSource();
    }

    /// <summary>
    /// 获取 WebSocket 是否已连接。
    /// </summary>
    public bool IsConnected => _isConnected && _webSocket.State == WebSocketState.Open;

    /// <summary>
    /// 连接到 WebSocket 端点。
    /// </summary>
    public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        if (_webSocket.State == WebSocketState.Open)
        {
            return;
        }

        // 添加认证头
        _webSocket.Options.SetRequestHeader("Authorization", $"{_auth.TokenType} {_auth.GetToken()}");

        await _webSocket.ConnectAsync(uri, cancellationToken);
        _isConnected = true;

        // 开始接收消息
        _ = ReceiveMessagesAsync(_cancellationTokenSource.Token);
    }

    /// <summary>
    /// 向服务器发送事件。
    /// </summary>
    protected async Task SendEventAsync<T>(T eventData, CancellationToken cancellationToken = default)
    {
        if (!IsConnected)
        {
            throw new InvalidOperationException("WebSocket is not connected");
        }

        var json = JsonHelper.SerializeObject(eventData);
        var bytes = Encoding.UTF8.GetBytes(json);

        await _sendLock.WaitAsync(cancellationToken);
        try
        {
            await _webSocket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                cancellationToken);
        }
        finally
        {
            _sendLock.Release();
        }
    }

    /// <summary>
    /// 从 WebSocket 接收消息。
    /// </summary>
    private async Task ReceiveMessagesAsync(CancellationToken cancellationToken)
    {
        var buffer = new byte[ReceiveBufferSize];
        var messageBuilder = new StringBuilder();

        try
        {
            while (!cancellationToken.IsCancellationRequested && _webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    cancellationToken);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await HandleCloseAsync(result.CloseStatus, result.CloseStatusDescription);
                    break;
                }

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

                    if (result.EndOfMessage)
                    {
                        var message = messageBuilder.ToString();
                        messageBuilder.Clear();

                        try
                        {
                            await HandleMessageAsync(message);
                        }
                        catch (Exception ex)
                        {
                            _logger?.LogError(ex, "Error handling message: {Message}", message);
                            await OnClientExceptionAsync(ex);
                        }
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // 正常取消
        }
        catch (WebSocketException ex)
        {
            _logger?.LogError(ex, "WebSocket error");
            await OnFailureAsync(ex);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error receiving messages");
            await OnClientExceptionAsync(ex);
        }
    }

    /// <summary>
    /// 处理接收到的消息。
    /// </summary>
    protected abstract Task HandleMessageAsync(string message);

    /// <summary>
    /// 处理 WebSocket 关闭。
    /// </summary>
    protected virtual async Task HandleCloseAsync(WebSocketCloseStatus? closeStatus, string? closeStatusDescription)
    {
        _isConnected = false;
        await OnClosedAsync((int?)closeStatus ?? 1000, closeStatusDescription ?? "Normal closure");
    }

    /// <summary>
    /// 当连接正在关闭时调用。
    /// </summary>
    protected virtual Task OnClosingAsync(int code, string reason) => Task.CompletedTask;

    /// <summary>
    /// 当连接已关闭时调用。
    /// </summary>
    protected virtual Task OnClosedAsync(int code, string reason) => Task.CompletedTask;

    /// <summary>
    /// 当发生故障时调用。
    /// </summary>
    protected virtual Task OnFailureAsync(Exception exception) => Task.CompletedTask;

    /// <summary>
    /// 当发生客户端异常时调用。
    /// </summary>
    protected virtual Task OnClientExceptionAsync(Exception exception) => Task.CompletedTask;

    /// <summary>
    /// 从 JSON 消息中解析事件类型。
    /// </summary>
    protected static string? ParseEventType(string message)
    {
        try
        {
            var json = JObject.Parse(message);
            return json["event_type"]?.ToString();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 关闭 WebSocket 连接。
    /// </summary>
    public async Task CloseAsync(CancellationToken cancellationToken = default)
    {
        if (_webSocket.State == WebSocketState.Open)
        {
            await OnClosingAsync(1000, "Normal closure");
            await _webSocket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Normal closure",
                cancellationToken);
        }

        _isConnected = false;
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            _cancellationTokenSource.Cancel();

            try
            {
                await CloseAsync();
            }
            catch
            {
                // 忽略关闭错误
            }

            _webSocket.Dispose();
            _cancellationTokenSource.Dispose();
            _sendLock.Dispose();
            _disposed = true;
        }
    }
}
