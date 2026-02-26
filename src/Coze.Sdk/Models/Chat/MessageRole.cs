using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Coze.Sdk.Models.Chat;

/// <summary>
/// 消息发送者的角色。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum MessageRole
{
    /// <summary>
    /// 未知角色。
    /// </summary>
    [EnumMember(Value = "unknown")]
    Unknown,

    /// <summary>
    /// 用户角色（消息由用户发送）。
    /// </summary>
    [EnumMember(Value = "user")]
    User,

    /// <summary>
    /// 助手角色（消息由 Bot 发送）。
    /// </summary>
    [EnumMember(Value = "assistant")]
    Assistant
}
