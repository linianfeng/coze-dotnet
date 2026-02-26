using Newtonsoft.Json;

namespace Coze.Sdk.Models.Chat;

/// <summary>
/// 表示 Chat 执行过程中的必需操作。
/// </summary>
public record ChatRequiredAction
{
    /// <summary>
    /// 获取必需操作的类型。
    /// </summary>
    [JsonProperty("type")]
    public string? Type { get; init; }

    /// <summary>
    /// 获取提交工具输出的信息。
    /// </summary>
    [JsonProperty("submit_tool_outputs")]
    public ChatSubmitToolOutputs? SubmitToolOutputs { get; init; }
}

/// <summary>
/// 表示提交工具输出的信息。
/// </summary>
public record ChatSubmitToolOutputs
{
    /// <summary>
    /// 获取工具调用列表。
    /// </summary>
    [JsonProperty("tool_calls")]
    public IReadOnlyList<ChatToolCall>? ToolCalls { get; init; }
}

/// <summary>
/// 表示 Chat 中的工具调用。
/// </summary>
public record ChatToolCall
{
    /// <summary>
    /// 获取工具调用 ID。
    /// </summary>
    [JsonProperty("id")]
    public string? Id { get; init; }

    /// <summary>
    /// 获取工具类型。
    /// </summary>
    [JsonProperty("type")]
    public string? Type { get; init; }

    /// <summary>
    /// 获取函数信息。
    /// </summary>
    [JsonProperty("function")]
    public ChatToolCallFunction? Function { get; init; }
}

/// <summary>
/// 表示工具调用函数。
/// </summary>
public record ChatToolCallFunction
{
    /// <summary>
    /// 获取函数名称。
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; init; }

    /// <summary>
    /// 获取函数参数（JSON 字符串格式）。
    /// </summary>
    [JsonProperty("arguments")]
    public string? Arguments { get; init; }
}
