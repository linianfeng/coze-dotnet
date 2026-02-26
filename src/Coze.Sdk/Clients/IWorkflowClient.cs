using System.Runtime.CompilerServices;
using Coze.Sdk.Models.Chat;
using Coze.Sdk.Models.Workflows;

namespace Coze.Sdk.Clients;

/// <summary>
/// 工作流操作接口。
/// 对应 Java SDK 中的 WorkflowService.java。
/// </summary>
public interface IWorkflowClient
{
    /// <summary>
    /// 运行工作流（非流式响应）。
    /// </summary>
    /// <param name="request">工作流请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工作流响应。</returns>
    Task<WorkflowResponse> RunAsync(WorkflowRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 运行工作流（流式响应）。
    /// </summary>
    /// <param name="request">工作流请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工作流事件的异步枚举。</returns>
    IAsyncEnumerable<WorkflowEvent> StreamAsync(WorkflowRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 恢复工作流（非流式响应）。
    /// </summary>
    /// <param name="request">恢复请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工作流响应。</returns>
    Task<WorkflowResponse> ResumeAsync(ResumeWorkflowRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 恢复工作流（流式响应）。
    /// </summary>
    /// <param name="request">恢复请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工作流事件的异步枚举。</returns>
    IAsyncEnumerable<WorkflowEvent> StreamResumeAsync(ResumeWorkflowRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取工作流运行历史。
    /// </summary>
    /// <param name="workflowId">工作流 ID。</param>
    /// <param name="executeId">执行 ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>工作流运行历史列表。</returns>
    Task<IReadOnlyList<WorkflowRunHistory>> GetRunHistoryAsync(string workflowId, string executeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 运行工作流聊天（流式响应）。
    /// </summary>
    /// <param name="request">工作流聊天请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>聊天事件的异步枚举。</returns>
    IAsyncEnumerable<ChatEvent> ChatStreamAsync(WorkflowChatRequest request, CancellationToken cancellationToken = default);
}
