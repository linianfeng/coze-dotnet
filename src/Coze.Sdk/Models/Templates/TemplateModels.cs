using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Coze.Sdk.Models.Templates;

/// <summary>
/// 模板实体类型。
/// 对应 Java SDK 中的 TemplateEntityType.java。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum TemplateEntityType
{
    /// <summary>
    /// Agent 类型。
    /// </summary>
    [EnumMember(Value = "agent")]
    Agent
}

/// <summary>
/// 复制模板的请求。
/// 对应 Java SDK 中的 DuplicateTemplateReq.java。
/// </summary>
public record DuplicateTemplateRequest
{
    /// <summary>
    /// 获取工作空间 ID。
    /// </summary>
    [JsonProperty("workspace_id")]
    public required string WorkspaceId { get; init; }

    /// <summary>
    /// 获取复制实体的可选名称。
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; init; }
}

/// <summary>
/// 复制模板的响应。
/// 对应 Java SDK 中的 DuplicateTemplateResp.java。
/// </summary>
public record DuplicateTemplateResponse
{
    /// <summary>
    /// 获取复制后的实体 ID。
    /// </summary>
    [JsonProperty("entity_id")]
    public string? EntityId { get; init; }

    /// <summary>
    /// 获取实体类型。
    /// </summary>
    [JsonProperty("entity_type")]
    public TemplateEntityType? EntityType { get; init; }
}
