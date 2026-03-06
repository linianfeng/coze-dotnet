using Microsoft.AspNetCore.Mvc;
using CozeAspNetCoreExample.Services;
using CozeAspNetCoreExample.Models;
using System.Text;
using System.Text.Json;

namespace CozeAspNetCoreExample.Controllers;

/// <summary>
/// 提供与 Coze 工作流交互的 API 端点
/// </summary>
[ApiController]
[Route("[controller]")]
public class WorkflowController : ControllerBase
{
    private readonly ICozeService _cozeService;
    private readonly ILogger<WorkflowController> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public WorkflowController(ICozeService cozeService, ILogger<WorkflowController> logger)
    {
        _cozeService = cozeService;
        _logger = logger;
    }

    /// <summary>
    /// 运行工作流（非流式）
    /// </summary>
    /// <param name="request">工作流请求参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>工作流执行结果</returns>
    [HttpPost("run")]
    public async Task<IActionResult> RunAsync([FromBody] WorkflowRunRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Running workflow {WorkflowId}", request.WorkflowId);

            var response = await _cozeService.RunWorkflowAsync(
                request.WorkflowId,
                request.Parameters,
                request.UserId,
                cancellationToken);

            _logger.LogInformation("Workflow {WorkflowId} completed", request.WorkflowId);

            return Ok(new WorkflowRunResponse
            {
                ExecuteId = response.ExecuteId,
                Output = response.Data,
                DebugUrl = response.DebugUrl
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while running workflow {WorkflowId}", request.WorkflowId);
            return StatusCode(500, new { Error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// 运行工作流（流式响应）
    /// </summary>
    /// <param name="workflowId">工作流 ID</param>
    /// <param name="parameters">工作流参数（JSON 字符串）</param>
    /// <param name="userId">用户 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    [HttpGet("run/stream")]
    public async Task RunStreamAsync(
        [FromQuery] string workflowId,
        [FromQuery] string? parameters = null,
        [FromQuery] string? userId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(workflowId))
        {
            Response.StatusCode = 400;
            await Response.WriteAsync("WorkflowId is required");
            return;
        }

        Response.Headers.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers.Connection = "keep-alive";

        try
        {
            _logger.LogInformation("Starting stream workflow {WorkflowId}", workflowId);

            // 解析参数 JSON
            Dictionary<string, object?>? paramDict = null;
            if (!string.IsNullOrWhiteSpace(parameters))
            {
                try
                {
                    paramDict = JsonSerializer.Deserialize<Dictionary<string, object?>>(parameters);
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Failed to parse parameters JSON");
                }
            }

            await foreach (var evt in _cozeService.RunWorkflowStreamAsync(workflowId, paramDict, userId, cancellationToken))
            {
                var json = JsonSerializer.Serialize(evt, JsonOptions);
                var sseMessage = $"data: {json}\n\n";
                await Response.WriteAsync(sseMessage, Encoding.UTF8, cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);

                if (evt.EventType is "done" or "error")
                {
                    break;
                }
            }

            await Response.WriteAsync("data: [DONE]\n\n", Encoding.UTF8, cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);

            _logger.LogInformation("Stream workflow completed for {WorkflowId}", workflowId);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Stream workflow cancelled for {WorkflowId}", workflowId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during stream workflow for {WorkflowId}", workflowId);
            var errorEvent = new StreamWorkflowEvent { EventType = "error", Error = "An error occurred" };
            var errorJson = JsonSerializer.Serialize(errorEvent, JsonOptions);
            await Response.WriteAsync($"data: {errorJson}\n\n", Encoding.UTF8, cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }

    /// <summary>
    /// 恢复工作流（流式响应）
    /// </summary>
    /// <param name="request">恢复请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    [HttpPost("resume/stream")]
    public async Task ResumeStreamAsync([FromBody] WorkflowResumeRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.WorkflowRunId) || string.IsNullOrWhiteSpace(request.EventId))
        {
            Response.StatusCode = 400;
            await Response.WriteAsync("WorkflowRunId and EventId are required");
            return;
        }

        Response.Headers.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers.Connection = "keep-alive";

        try
        {
            _logger.LogInformation("Resuming workflow {WorkflowRunId}", request.WorkflowRunId);

            await foreach (var evt in _cozeService.ResumeWorkflowStreamAsync(
                request.WorkflowRunId,
                request.EventId,
                request.ResumeData,
                cancellationToken))
            {
                var json = JsonSerializer.Serialize(evt, JsonOptions);
                var sseMessage = $"data: {json}\n\n";
                await Response.WriteAsync(sseMessage, Encoding.UTF8, cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);

                if (evt.EventType is "done" or "error")
                {
                    break;
                }
            }

            await Response.WriteAsync("data: [DONE]\n\n", Encoding.UTF8, cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);

            _logger.LogInformation("Resume workflow completed for {WorkflowRunId}", request.WorkflowRunId);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Resume workflow cancelled for {WorkflowRunId}", request.WorkflowRunId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during resume workflow for {WorkflowRunId}", request.WorkflowRunId);
            var errorEvent = new StreamWorkflowEvent { EventType = "error", Error = "An error occurred" };
            var errorJson = JsonSerializer.Serialize(errorEvent, JsonOptions);
            await Response.WriteAsync($"data: {errorJson}\n\n", Encoding.UTF8, cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }

    /// <summary>
    /// 获取工作流运行历史
    /// </summary>
    /// <param name="workflowId">工作流 ID</param>
    /// <param name="executeId">执行 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>工作流运行历史列表</returns>
    [HttpGet("history/{workflowId}/{executeId}")]
    public async Task<IActionResult> GetHistoryAsync(string workflowId, string executeId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting workflow history for {WorkflowId}/{ExecuteId}", workflowId, executeId);

            var history = await _cozeService.GetWorkflowHistoryAsync(workflowId, executeId, cancellationToken);

            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting workflow history for {WorkflowId}/{ExecuteId}", workflowId, executeId);
            return StatusCode(500, new { Error = "An error occurred while retrieving workflow history" });
        }
    }
}
