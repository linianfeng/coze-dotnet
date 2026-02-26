using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Coze.Sdk.Models.Chat;

/// <summary>
/// Chat 的状态。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum ChatStatus
{
    /// <summary>
    /// Chat 已创建。
    /// </summary>
    [EnumMember(Value = "created")]
    Created,

    /// <summary>
    /// Bot 正在处理中。
    /// </summary>
    [EnumMember(Value = "in_progress")]
    InProgress,

    /// <summary>
    /// Bot 已完成处理，会话已结束。
    /// </summary>
    [EnumMember(Value = "completed")]
    Completed,

    /// <summary>
    /// 会话已失败。
    /// </summary>
    [EnumMember(Value = "failed")]
    Failed,

    /// <summary>
    /// 会话被中断，需要进一步处理。
    /// </summary>
    [EnumMember(Value = "requires_action")]
    RequiresAction,

    /// <summary>
    /// 会话已取消。
    /// </summary>
    [EnumMember(Value = "canceled")]
    Canceled
}
