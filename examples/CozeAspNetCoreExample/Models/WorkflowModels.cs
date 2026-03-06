using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CozeAspNetCoreExample.Models;

/// <summary>
/// 运行工作流的请求
/// </summary>
public class WorkflowRunRequest
{
    /// <summary>
    /// 工作流 ID
    /// </summary>
    [Required(ErrorMessage = "WorkflowId is required")]
    public string WorkflowId { get; set; } = string.Empty;

    /// <summary>
    /// 工作流参数（JSON 对象）
    /// </summary>
    public Dictionary<string, object?>? Parameters { get; set; }

    /// <summary>
    /// 用户 ID（可选）
    /// </summary>
    public string? UserId { get; set; }
}

/// <summary>
/// 恢复工作流的请求
/// </summary>
public class WorkflowResumeRequest
{
    /// <summary>
    /// 工作流运行 ID
    /// </summary>
    [Required(ErrorMessage = "WorkflowRunId is required")]
    public string WorkflowRunId { get; set; } = string.Empty;

    /// <summary>
    /// 事件 ID
    /// </summary>
    [Required(ErrorMessage = "EventId is required")]
    public string EventId { get; set; } = string.Empty;

    /// <summary>
    /// 恢复数据
    /// </summary>
    public object? ResumeData { get; set; }
}

/// <summary>
/// 工作流响应
/// </summary>
public record WorkflowRunResponse
{
    /// <summary>
    /// 执行 ID
    /// </summary>
    [JsonPropertyName("executeId")]
    public string? ExecuteId { get; init; }

    /// <summary>
    /// 输出内容
    /// </summary>
    [JsonPropertyName("output")]
    public string? Output { get; init; }

    /// <summary>
    /// 调试 URL
    /// </summary>
    [JsonPropertyName("debugUrl")]
    public string? DebugUrl { get; init; }
}
