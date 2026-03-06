using Coze.Sdk;
using Coze.Sdk.Models.Workflows;

namespace CozeAspNetCoreExample.Services;

public interface ICozeService
{
    /// <summary>
    /// 与 Bot 进行聊天（非流式）
    /// </summary>
    Task<string> ChatWithBotAsync(string botId, string message, string? userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 与 Bot 进行聊天（流式响应）
    /// </summary>
    IAsyncEnumerable<StreamChatEvent> ChatWithBotStreamAsync(string botId, string message, string? userId = null, CancellationToken cancellationToken = default);

    #region 工作流方法

    /// <summary>
    /// 运行工作流（非流式）
    /// </summary>
    Task<WorkflowResponse> RunWorkflowAsync(string workflowId, Dictionary<string, object?>? parameters = null, string? userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 运行工作流（流式响应）
    /// </summary>
    IAsyncEnumerable<StreamWorkflowEvent> RunWorkflowStreamAsync(string workflowId, Dictionary<string, object?>? parameters = null, string? userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 恢复工作流（流式响应）
    /// </summary>
    IAsyncEnumerable<StreamWorkflowEvent> ResumeWorkflowStreamAsync(string workflowRunId, string eventId, object? resumeData = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取工作流运行历史
    /// </summary>
    Task<IReadOnlyList<WorkflowRunHistory>> GetWorkflowHistoryAsync(string workflowId, string executeId, CancellationToken cancellationToken = default);

    #endregion
}

/// <summary>
/// 流式聊天事件
/// </summary>
public record StreamChatEvent
{
    /// <summary>
    /// 事件类型
    /// </summary>
    public string EventType { get; init; } = string.Empty;

    /// <summary>
    /// 消息内容
    /// </summary>
    public string? Content { get; init; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? Error { get; init; }
}

/// <summary>
/// 流式工作流事件
/// </summary>
public record StreamWorkflowEvent
{
    /// <summary>
    /// 事件类型：message, error, done, interrupt
    /// </summary>
    public string EventType { get; init; } = string.Empty;

    /// <summary>
    /// 消息内容
    /// </summary>
    public string? Content { get; init; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// 执行 ID（用于中断恢复）
    /// </summary>
    public string? ExecuteId { get; init; }
}
