using System.Runtime.CompilerServices;
using Coze.Sdk.Http;
using Coze.Sdk.Models.Chat;
using Coze.Sdk.Models.Workflows;
using Coze.Sdk.Utils;

namespace Coze.Sdk.Clients;

/// <summary>
/// 工作流操作的实现。
/// 对应 Java SDK 中的 WorkflowRunService.java。
/// </summary>
internal class WorkflowClient : IWorkflowClient
{
    private readonly CozeHttpClient _httpClient;

    public WorkflowClient(CozeHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WorkflowResponse> RunAsync(
        WorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        var nonStreamingRequest = request;
        if (request.Stream != true)
        {
            nonStreamingRequest = request with { Stream = false };
        }

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

    public async IAsyncEnumerable<WorkflowEvent> StreamAsync(
        WorkflowRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var streamingRequest = request.WithStream();

        var httpRequest = _httpClient.CreateRequest(ApiEndpoints.WorkflowRun, HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, streamingRequest);

        var (stream, logId) = await _httpClient.ExecuteStreamWithLogIdAsync(httpRequest, cancellationToken);

        await foreach (var (id, eventType, data) in SseReader.ReadWorkflowEventsAsync(stream, cancellationToken))
        {
            yield return WorkflowEvent.ParseEvent(id, eventType, data, logId);
        }
    }

    public async Task<WorkflowResponse> ResumeAsync(
        ResumeWorkflowRequest request,
        CancellationToken cancellationToken = default)
    {
        var nonStreamingRequest = request;
        if (request.Stream != true)
        {
            nonStreamingRequest = request with { Stream = false };
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

    public async IAsyncEnumerable<WorkflowEvent> StreamResumeAsync(
        ResumeWorkflowRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var streamingRequest = request.WithStream();

        var endpoint = $"/v1/workflow/runs/{request.WorkflowRunId}/resume";
        var httpRequest = _httpClient.CreateRequest(endpoint, HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, streamingRequest);

        var (stream, logId) = await _httpClient.ExecuteStreamWithLogIdAsync(httpRequest, cancellationToken);

        await foreach (var (id, eventType, data) in SseReader.ReadWorkflowEventsAsync(stream, cancellationToken))
        {
            yield return WorkflowEvent.ParseEvent(id, eventType, data, logId);
        }
    }

    public async Task<IReadOnlyList<WorkflowRunHistory>> GetRunHistoryAsync(
        string workflowId,
        string executeId,
        CancellationToken cancellationToken = default)
    {
        var endpoint = $"/v1/workflows/{workflowId}/run_histories/{executeId}";
        var httpRequest = _httpClient.CreateGetRequest(endpoint);

        return await _httpClient.ExecuteAsync<IReadOnlyList<WorkflowRunHistory>>(httpRequest, cancellationToken);
    }

    public async IAsyncEnumerable<ChatEvent> ChatStreamAsync(
        WorkflowChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest("/v1/workflows/chat", HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, request);

        var (stream, logId) = await _httpClient.ExecuteStreamWithLogIdAsync(httpRequest, cancellationToken);

        await foreach (var (eventType, data) in SseReader.ReadChatEventsAsync(stream, cancellationToken))
        {
            yield return ChatEvent.ParseEvent(eventType, data, logId);
        }
    }
}
