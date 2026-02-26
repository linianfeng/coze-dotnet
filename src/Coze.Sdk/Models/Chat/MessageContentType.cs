using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Coze.Sdk.Models.Chat;

/// <summary>
/// 消息的内容类型。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum MessageContentType
{
    /// <summary>
    /// 未知类型。
    /// </summary>
    [EnumMember(Value = "unknown")]
    Unknown,

    /// <summary>
    /// 文本内容。
    /// </summary>
    [EnumMember(Value = "text")]
    Text,

    /// <summary>
    /// 对象字符串内容（多模态）。
    /// </summary>
    [EnumMember(Value = "object_string")]
    ObjectString,

    /// <summary>
    /// 音频内容。
    /// </summary>
    [EnumMember(Value = "audio")]
    Audio,

    /// <summary>
    /// 卡片内容。
    /// </summary>
    [EnumMember(Value = "card")]
    Card
}
