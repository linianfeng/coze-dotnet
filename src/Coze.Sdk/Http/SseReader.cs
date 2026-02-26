using System.Runtime.CompilerServices;
using System.Text;

namespace Coze.Sdk.Http;

/// <summary>
/// SSE 格式类型。
/// </summary>
internal enum SseFormat
{
    /// <summary>
    /// Chat 格式：event:\ndata: 格式。
    /// </summary>
    Chat,

    /// <summary>
    /// Workflow 格式：id:\nevent:\ndata: 格式。
    /// </summary>
    Workflow
}

/// <summary>
/// 服务器发送事件（SSE）读取器。
/// 对应 Java SDK 中的 AbstractEventCallback.java。
/// </summary>
internal static class SseReader
{
    /// <summary>
    /// 从流中读取 SSE 事件。
    /// </summary>
    /// <param name="stream">输入流。</param>
    /// <param name="format">SSE 格式。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>事件数据字典的异步枚举。</returns>
    public static async IAsyncEnumerable<Dictionary<string, string>> ReadEventsAsync(
        Stream stream,
        SseFormat format,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8);

        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (line == null)
            {
                yield break;
            }

            // 跳过空行
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var eventData = new Dictionary<string, string>();

            if (format == SseFormat.Chat)
            {
                // Chat 格式：event: xxx \n data: xxx
                if (line.StartsWith("event:", StringComparison.OrdinalIgnoreCase))
                {
                    eventData["event"] = line.Substring(6).Trim();

                    // 读取数据行
                    var dataLine = await reader.ReadLineAsync(cancellationToken);
                    if (dataLine != null && dataLine.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                    {
                        eventData["data"] = dataLine.Substring(5).Trim();
                    }

                    yield return eventData;
                }
                else if (line.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                {
                    // 某些响应可能只有 data 而没有 event
                    eventData["event"] = "done";
                    eventData["data"] = line.Substring(5).Trim();
                    yield return eventData;
                }
            }
            else if (format == SseFormat.Workflow)
            {
                // Workflow 格式：id: xxx \n event: xxx \n data: xxx
                if (line.StartsWith("id:", StringComparison.OrdinalIgnoreCase))
                {
                    eventData["id"] = line.Substring(3).Trim();

                    // 读取事件行
                    var eventLine = await reader.ReadLineAsync(cancellationToken);
                    if (eventLine != null && eventLine.StartsWith("event:", StringComparison.OrdinalIgnoreCase))
                    {
                        eventData["event"] = eventLine.Substring(6).Trim();
                    }

                    // 读取数据行
                    var dataLine = await reader.ReadLineAsync(cancellationToken);
                    if (dataLine != null && dataLine.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                    {
                        eventData["data"] = dataLine.Substring(5).Trim();
                    }

                    if (eventData.Count > 0)
                    {
                        yield return eventData;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 从流中读取聊天 SSE 事件。
    /// </summary>
    /// <param name="stream">输入流。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>（事件类型，数据）元组的异步枚举。</returns>
    public static async IAsyncEnumerable<(string EventType, string Data)> ReadChatEventsAsync(
        Stream stream,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var eventData in ReadEventsAsync(stream, SseFormat.Chat, cancellationToken))
        {
            var eventType = eventData.GetValueOrDefault("event", "unknown");
            var data = eventData.GetValueOrDefault("data", string.Empty);
            yield return (eventType, data);
        }
    }

    /// <summary>
    /// 从流中读取工作流 SSE 事件。
    /// </summary>
    /// <param name="stream">输入流。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>（ID，事件类型，数据）元组的异步枚举。</returns>
    public static async IAsyncEnumerable<(string Id, string EventType, string Data)> ReadWorkflowEventsAsync(
        Stream stream,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var eventData in ReadEventsAsync(stream, SseFormat.Workflow, cancellationToken))
        {
            var id = eventData.GetValueOrDefault("id", string.Empty);
            var eventType = eventData.GetValueOrDefault("event", "unknown");
            var data = eventData.GetValueOrDefault("data", string.Empty);
            yield return (id, eventType, data);
        }
    }
}
