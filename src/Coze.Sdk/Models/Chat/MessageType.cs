using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Coze.Sdk.Models.Chat;

/// <summary>
/// 消息类型。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum MessageType
{
    /// <summary>
    /// 未知类型。
    /// </summary>
    [EnumMember(Value = "")]
    Unknown,

    /// <summary>
    /// 问题消息（用户输入内容）。
    /// </summary>
    [EnumMember(Value = "question")]
    Question,

    /// <summary>
    /// 回答消息（Bot 返回给用户的内容）。
    /// </summary>
    [EnumMember(Value = "answer")]
    Answer,

    /// <summary>
    /// 后续追问消息。
    /// </summary>
    [EnumMember(Value = "follow_up")]
    FollowUp,

    /// <summary>
    /// 函数调用的中间结果。
    /// </summary>
    [EnumMember(Value = "function_call")]
    FunctionCall,

    /// <summary>
    /// 工具调用后的输出结果。
    /// </summary>
    [EnumMember(Value = "tool_output")]
    ToolOutput,

    /// <summary>
    /// 工具调用后的响应结果。
    /// </summary>
    [EnumMember(Value = "tool_response")]
    ToolResponse,

    /// <summary>
    /// 多回答场景下的详细包。
    /// </summary>
    [EnumMember(Value = "verbose")]
    Verbose
}
