using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Coze.Sdk.Models.Chat;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Coze.Sdk.Models.Common; 
namespace Coze.Sdk.Models.Workflows;

/// <summary>
/// Workflow 事件类型。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum WorkflowEventType
{
    /// <summary>
    /// 消息事件。
    /// </summary>
    [EnumMember(Value = "Message")]
    Message,

    /// <summary>
    /// 错误事件。
    /// </summary>
    [EnumMember(Value = "Error")]
    Error,

    /// <summary>
    /// 完成事件。
    /// </summary>
    [EnumMember(Value = "Done")]
    Done,

    /// <summary>
    /// 中断事件。
    /// </summary>
    [EnumMember(Value = "Interrupt")]
    Interrupt
}

/// <summary>
/// 运行 Workflow 的请求。
/// </summary>
public record WorkflowRequest
{
    /// <summary>
    /// 获取 Workflow ID。
    /// </summary>
    [JsonProperty("workflow_id")]
    public required string WorkflowId { get; init; }

    /// <summary>
    /// 获取 Workflow 的参数。
    /// </summary>
    [JsonProperty("parameters")]
    public IReadOnlyDictionary<string, object?>? Parameters { get; init; }

    /// <summary>
    /// 获取是否以流式模式运行。
    /// </summary>
    [JsonProperty("stream")]
    public bool? Stream { get; init; }

    /// <summary>
    /// 获取用于 Workflow 聊天的 Bot ID。
    /// </summary>
    [JsonProperty("bot_id")]
    public string? BotId { get; init; }

    /// <summary>
    /// 获取用户 ID。
    /// </summary>
    [JsonProperty("user_id")]
    public string? UserId { get; init; }

    internal WorkflowRequest WithStream()
    {
        return this with { Stream = true };
    }
}

/// <summary>
/// 运行 Workflow 的响应。
/// </summary>
public record WorkflowResponse : ApiResponse<string>
{
    /// <summary>
    /// 获取异步执行的执行 ID。
    /// 仅当工作流异步执行时（is_async=true）返回。
    /// </summary>
    [JsonProperty("execute_id")]
    public string? ExecuteId { get; init; }

   

    /// <summary>
    /// 获取调试 URL。
    /// </summary>
    [JsonProperty("debug_url")]
    public string? DebugUrl { get; init; }

 

    /// <summary>
    /// 获取 Token 使用量信息。
    /// </summary>
    [JsonProperty("usage")]
    public ChatUsage? Usage { get; init; }
}

/// <summary>
/// Workflow 的 Token 使用量。
/// </summary>
public record WorkflowTokenUsage
{
    /// <summary>
    /// 获取输入 Token 数量。
    /// </summary>
    [JsonProperty("input")]
    public int? Input { get; init; }

    /// <summary>
    /// 获取输出 Token 数量。
    /// </summary>
    [JsonProperty("output")]
    public int? Output { get; init; }

    /// <summary>
    /// 获取总 Token 数量。
    /// </summary>
    [JsonProperty("total")]
    public int? Total { get; init; }
}

/// <summary>
/// 流式传输的 Workflow 事件。
/// </summary>
public record WorkflowEvent
{
    /// <summary>
    /// 获取事件 ID。
    /// </summary>
    [JsonIgnore]
    public string? Id { get; init; }

    /// <summary>
    /// 获取事件类型。
    /// </summary>
    [JsonIgnore]
    public WorkflowEventType EventType { get; init; }

    /// <summary>
    /// 获取消息内容。
    /// </summary>
    [JsonIgnore]
    public string? Message { get; init; }

    /// <summary>
    /// 获取日志 ID。
    /// </summary>
    [JsonIgnore]
    public string? LogId { get; init; }

    /// <summary>
    /// 获取一个值，指示流是否已完成。
    /// </summary>
    [JsonIgnore]
    public bool IsDone => EventType == WorkflowEventType.Done || EventType == WorkflowEventType.Error;

    /// <summary>
    /// 从 SSE 数据解析 Workflow 事件。
    /// </summary>
    public static WorkflowEvent ParseEvent(string id, string eventType, string data, string? logId = null)
    {
        var type = eventType?.ToLowerInvariant() switch
        {
            "message" => WorkflowEventType.Message,
            "error" => WorkflowEventType.Error,
            "done" => WorkflowEventType.Done,
            _ => WorkflowEventType.Done
        };

        return new WorkflowEvent
        {
            Id = id,
            EventType = type,
            Message = data,
            LogId = logId
        };
    }
}

/// <summary>
/// 恢复 Workflow 的请求。
/// </summary>
public record ResumeWorkflowRequest
{
    /// <summary>
    /// 获取 Workflow 运行 ID。
    /// </summary>
    [JsonProperty("workflow_run_id")]
    public required string WorkflowRunId { get; init; }

    /// <summary>
    /// 获取事件 ID。
    /// </summary>
    [JsonProperty("event_id")]
    public required string EventId { get; init; }

    /// <summary>
    /// 获取恢复数据。
    /// </summary>
    [JsonProperty("resume_data")]
    public object? ResumeData { get; init; }

    /// <summary>
    /// 获取是否以流式模式运行。
    /// </summary>
    [JsonProperty("stream")]
    public bool? Stream { get; init; }

    internal ResumeWorkflowRequest WithStream()
    {
        return this with { Stream = true };
    }
}

/// <summary>
/// Workflow 执行状态。
/// 对应 Java SDK 中的 WorkflowExecuteStatus.java。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum WorkflowExecuteStatus
{
    /// <summary>
    /// 执行成功。
    /// </summary>
    [EnumMember(Value = "success")]
    Success,

    /// <summary>
    /// 执行中。
    /// </summary>
    [EnumMember(Value = "running")]
    Running,

    /// <summary>
    /// 执行失败。
    /// </summary>
    [EnumMember(Value = "fail")]
    Fail
}

/// <summary>
/// Workflow 运行模式。
/// 对应 Java SDK 中的 WorkflowRunMode.java。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum WorkflowRunMode
{
    /// <summary>
    /// 同步操作。
    /// </summary>
    [EnumMember(Value = "0")]
    Sync = 0,

    /// <summary>
    /// 流式操作。
    /// </summary>
    [EnumMember(Value = "1")]
    Streaming = 1,

    /// <summary>
    /// 异步操作。
    /// </summary>
    [EnumMember(Value = "2")]
    Async = 2
}

/// <summary>
/// Workflow 运行历史记录。
/// 对应 Java SDK 中的 WorkflowRunHistory.java。
/// </summary>
public record WorkflowRunHistory
{
    /// <summary>
    /// 获取执行 ID。
    /// </summary>
    [JsonProperty("execute_id")]
    public string? ExecuteId { get; init; }

    /// <summary>
    /// 获取执行状态。
    /// </summary>
    [JsonProperty("execute_status")]
    public WorkflowExecuteStatus? ExecuteStatus { get; init; }

    /// <summary>
    /// 获取 Bot ID。
    /// </summary>
    [JsonProperty("bot_id")]
    public string? BotId { get; init; }

    /// <summary>
    /// 获取连接器 ID。
    /// </summary>
    [JsonProperty("connector_id")]
    public string? ConnectorId { get; init; }

    /// <summary>
    /// 获取连接器 UID。
    /// </summary>
    [JsonProperty("connector_uid")]
    public string? ConnectorUid { get; init; }

    /// <summary>
    /// 获取运行模式。
    /// </summary>
    [JsonProperty("run_mode")]
    public WorkflowRunMode? RunMode { get; init; }

    /// <summary>
    /// 获取日志 ID。
    /// </summary>
    [JsonProperty("logid")]
    public string? LogId { get; init; }

    /// <summary>
    /// 获取创建时间戳。
    /// </summary>
    [JsonProperty("create_time")]
    public int? CreateTime { get; init; }

    /// <summary>
    /// 获取更新时间戳。
    /// </summary>
    [JsonProperty("update_time")]
    public int? UpdateTime { get; init; }

    /// <summary>
    /// 获取输出。
    /// </summary>
    [JsonProperty("output")]
    public string? Output { get; init; }

    /// <summary>
    /// 获取错误码。
    /// </summary>
    [JsonProperty("error_code")]
    public string? ErrorCode { get; init; }

    /// <summary>
    /// 获取错误消息。
    /// </summary>
    [JsonProperty("error_message")]
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// 获取调试 URL。
    /// </summary>
    [JsonProperty("debug_url")]
    public string? DebugUrl { get; init; }

    /// <summary>
    /// 获取 Token 使用量。
    /// </summary>
    [JsonProperty("usage")]
    public ChatUsage? Usage { get; init; }
}

/// <summary>
/// Workflow 聊天请求。
/// 对应 Java SDK 中的 WorkflowChatReq.java。
/// </summary>
public record WorkflowChatRequest
{
    /// <summary>
    /// 获取 Workflow ID。
    /// </summary>
    [JsonProperty("workflow_id")]
    public required string WorkflowId { get; init; }

    /// <summary>
    /// 获取附加消息。
    /// </summary>
    [JsonProperty("additional_messages")]
    public IReadOnlyList<Message>? AdditionalMessages { get; init; }

    /// <summary>
    /// 获取参数。
    /// </summary>
    [JsonProperty("parameters")]
    public IReadOnlyDictionary<string, object?>? Parameters { get; init; }

    /// <summary>
    /// 获取应用 ID。
    /// </summary>
    [JsonProperty("app_id")]
    public string? AppId { get; init; }

    /// <summary>
    /// 获取 Bot ID。
    /// </summary>
    [JsonProperty("bot_id")]
    public string? BotId { get; init; }

    /// <summary>
    /// 获取会话 ID。
    /// </summary>
    [JsonProperty("conversation_id")]
    public string? ConversationId { get; init; }

    /// <summary>
    /// 获取扩展数据。
    /// </summary>
    [JsonProperty("ext")]
    public IReadOnlyDictionary<string, string>? Ext { get; init; }
}
