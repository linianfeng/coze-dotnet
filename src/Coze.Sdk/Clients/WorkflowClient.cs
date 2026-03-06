using System.Runtime.CompilerServices;
using Coze.Sdk.Http;
using Coze.Sdk.Models.Chat;
using Coze.Sdk.Models.Workflows;
using Coze.Sdk.Utils;

namespace Coze.Sdk.Clients;

/// <summary>
/// 工作流操作的实现类。
/// 对应 Java SDK 中的 WorkflowRunService.java。
/// </summary>
internal class WorkflowClient : IWorkflowClient
{
    private readonly CozeHttpClient _httpClient;

    /// <summary>
    /// 初始化 WorkflowClient 的新实例。
    /// </summary>
    /// <param name="httpClient">HTTP 客户端</param>
    public WorkflowClient(CozeHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// 以非流式方式运行工作流。
    /// </summary>
    /// <param name="request">工作流运行请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>工作流响应结果</returns>
    public async Task<WorkflowResponse> RunAsync(
        WorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        var nonStreamingRequest = request;
        if (request.Stream != true)
        {
            nonStreamingRequest = request with { Stream = false };
        }

        // 转换参数中的 JsonElement 为原始值，解决 ASP.NET Core System.Text.Json 兼容性问题
        nonStreamingRequest = nonStreamingRequest with
        {
            Parameters = JsonHelper.ConvertParameters(nonStreamingRequest.Parameters)
        };

        var httpRequest = _httpClient.CreateRequest(ApiEndpoints.WorkflowRun, HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, nonStreamingRequest);

        // 工作流 API 响应格式特殊：data 字段是字符串而非对象
        // 需要使用 ExecuteRawAsync 获取原始 JSON，然后直接解析为 WorkflowResponse
        var rawResponse = await _httpClient.ExecuteRawAsync(httpRequest, cancellationToken);
        var response = JsonHelper.DeserializeObject<WorkflowResponse>(rawResponse);

        if (response == null)
        {
            throw new Exceptions.CozeApiException(0, 0, "Failed to parse workflow response");
        }

        return response;
    }

    /// <summary>
    /// 以流式方式运行工作流。
    /// </summary>
    /// <param name="request">工作流运行请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>工作流事件流</returns>
    public async IAsyncEnumerable<WorkflowEvent> StreamAsync(
        WorkflowRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var streamingRequest = request.WithStream();

        // 转换参数中的 JsonElement 为原始值，解决 ASP.NET Core System.Text.Json 兼容性问题
        streamingRequest = streamingRequest with
        {
            Parameters = JsonHelper.ConvertParameters(streamingRequest.Parameters)
        };

        Console.WriteLine($"[DEBUG] StreamAsync: Sending request to {ApiEndpoints.WorkflowRun}");
        Console.WriteLine($"[DEBUG] StreamAsync: Stream={streamingRequest.Stream}");

        var httpRequest = _httpClient.CreateRequest(ApiEndpoints.WorkflowRun, HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, streamingRequest);

        var (stream, logId) = await _httpClient.ExecuteStreamWithLogIdAsync(httpRequest, cancellationToken);

        Console.WriteLine($"[DEBUG] StreamAsync: Got stream, LogId={logId}");
        Console.WriteLine($"[DEBUG] StreamAsync: Stream CanRead={stream.CanRead}, Position: {stream.Position}");
        try
        {
            Console.WriteLine($"[DEBUG] StreamAsync: Stream Length: {stream.Length}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DEBUG] StreamAsync: Error getting stream length: {ex.Message}");
        }

        var eventCount = 0;
        await foreach (var (id, eventType, data) in SseReader.ReadWorkflowEventsAsync(stream, cancellationToken))
        {
            eventCount++;
            Console.WriteLine($"[DEBUG] StreamAsync: Event #{eventCount} - Id={id}, Type={eventType}, Data={data?.Substring(0, Math.Min(100, data?.Length ?? 0))}...");
            yield return WorkflowEvent.ParseEvent(id, eventType, data, logId);
        }

        Console.WriteLine($"[DEBUG] StreamAsync: Total events received: {eventCount}");
    }

    /// <summary>
    /// 以非流式方式恢复工作流运行。
    /// </summary>
    /// <param name="request">恢复工作流请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>工作流响应结果</returns>
    public async Task<WorkflowResponse> ResumeAsync(
        ResumeWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        var nonStreamingRequest = request;
        if (request.Stream != true)
        {
            nonStreamingRequest = request with { Stream = false };
        }

        // 转换 ResumeData 中的 JsonElement 为原始值
        if (nonStreamingRequest.ResumeData is System.Text.Json.JsonElement jsonElement)
        {
            nonStreamingRequest = nonStreamingRequest with
            {
                ResumeData = JsonHelper.ConvertJsonElementToObject(jsonElement)
            };
        }

        var endpoint = $"/v1/workflow/runs/{request.WorkflowRunId}/resume";
        var httpRequest = _httpClient.CreateRequest(endpoint, HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, nonStreamingRequest);

        // 工作流 API 响应格式特殊：data 字段是字符串而非对象
        var rawResponse = await _httpClient.ExecuteRawAsync(httpRequest, cancellationToken);
        var response = JsonHelper.DeserializeObject<WorkflowResponse>(rawResponse);

        if (response == null)
        {
            throw new Exceptions.CozeApiException(0, 0, "Failed to parse workflow response");
        }

        return response;
    }

    /// <summary>
    /// 以流式方式恢复工作流运行。
    /// </summary>
    /// <param name="request">恢复工作流请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>工作流事件流</returns>
    public async IAsyncEnumerable<WorkflowEvent> StreamResumeAsync(
        ResumeWorkflowRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var streamingRequest = request.WithStream();

        // 转换 ResumeData 中的 JsonElement 为原始值
        if (streamingRequest.ResumeData is System.Text.Json.JsonElement jsonElement)
        {
            streamingRequest = streamingRequest with
            {
                ResumeData = JsonHelper.ConvertJsonElementToObject(jsonElement)
            };
        }

        var endpoint = $"/v1/workflow/runs/{request.WorkflowRunId}/resume";
        var httpRequest = _httpClient.CreateRequest(endpoint, HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, streamingRequest);

        var (stream, logId) = await _httpClient.ExecuteStreamWithLogIdAsync(httpRequest, cancellationToken);

        await foreach (var (id, eventType, data) in SseReader.ReadWorkflowEventsAsync(stream, cancellationToken))
        {
            yield return WorkflowEvent.ParseEvent(id, eventType, data, logId);
        }
    }

    /// <summary>
    /// 获取工作流运行历史记录。
    /// </summary>
    /// <param name="workflowId">工作流 ID</param>
    /// <param name="executeId">执行 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>工作流运行历史记录列表</returns>
    public async Task<IReadOnlyList<WorkflowRunHistory>> GetRunHistoryAsync(
        string workflowId,
        string executeId,
        CancellationToken cancellationToken = default)
    {
        var endpoint = $"/v1/workflows/{workflowId}/run_histories/{executeId}";
        var httpRequest = _httpClient.CreateGetRequest(endpoint);

        return await _httpClient.ExecuteAsync<IReadOnlyList<WorkflowRunHistory>>(httpRequest, cancellationToken);
    }

    /// <summary>
    /// 以流式方式进行工作流聊天。
    /// </summary>
    /// <param name="request">工作流聊天请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>聊天事件流</returns>
    public async IAsyncEnumerable<ChatEvent> ChatStreamAsync(
        WorkflowChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // 转换参数中的 JsonElement 为原始值，解决 ASP.NET Core System.Text.Json 兼容性问题
        var normalizedRequest = request with
        {
            Parameters = JsonHelper.ConvertParameters(request.Parameters)
        };

        var httpRequest = _httpClient.CreateRequest("/v1/workflows/chat", HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, normalizedRequest);

        var (stream, logId) = await _httpClient.ExecuteStreamWithLogIdAsync(httpRequest, cancellationToken);

        await foreach (var (eventType, data) in SseReader.ReadChatEventsAsync(stream, cancellationToken))
        {
            yield return ChatEvent.ParseEvent(eventType, data, logId);
        }
    }
}
