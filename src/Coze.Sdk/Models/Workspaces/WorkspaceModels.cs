using Newtonsoft.Json;

namespace Coze.Sdk.Models.Workspaces;

/// <summary>
/// 工作空间模型。
/// 对应 Java SDK 中的 Workspace.java。
/// </summary>
public record Workspace
{
    /// <summary>
    /// 获取工作空间 ID。
    /// </summary>
    [JsonProperty("id")]
    public string? Id { get; init; }

    /// <summary>
    /// 获取工作空间名称。
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; init; }

    /// <summary>
    /// 获取工作空间类型。
    /// </summary>
    [JsonProperty("workspace_type")]
    public string? WorkspaceType { get; init; }

    /// <summary>
    /// 获取图标 URL。
    /// </summary>
    [JsonProperty("icon_url")]
    public string? IconUrl { get; init; }

    /// <summary>
    /// 获取是否为个人工作空间。
    /// </summary>
    [JsonProperty("is_personal")]
    public bool? IsPersonal { get; init; }

    /// <summary>
    /// 获取在工作空间中的角色。
    /// </summary>
    [JsonProperty("role_type")]
    public string? RoleType { get; init; }

    /// <summary>
    /// 获取描述。
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; init; }

    /// <summary>
    /// 获取企业 ID。
    /// </summary>
    [JsonProperty("enterprise_id")]
    public string? EnterpriseId { get; init; }

    /// <summary>
    /// 获取加入状态。
    /// </summary>
    [JsonProperty("joined_status")]
    public string? JoinedStatus { get; init; }

    /// <summary>
    /// 获取所有者 UID。
    /// </summary>
    [JsonProperty("owner_uid")]
    public string? OwnerUid { get; init; }

    /// <summary>
    /// 获取管理员 UID 列表。
    /// </summary>
    [JsonProperty("admin_uids")]
    public IReadOnlyList<string>? AdminUids { get; init; }

    /// <summary>
    /// 获取创建时间戳。
    /// </summary>
    [JsonProperty("created_at")]
    public long? CreatedAt { get; init; }

    /// <summary>
    /// 获取更新时间戳。
    /// </summary>
    [JsonProperty("updated_at")]
    public long? UpdatedAt { get; init; }
}

/// <summary>
/// 列出工作空间的请求。
/// </summary>
public record ListWorkspacesRequest
{
    /// <summary>
    /// 获取页码。
    /// </summary>
    [JsonProperty("page_num")]
    public int? PageNumber { get; init; } = 1;

    /// <summary>
    /// 获取每页数量。
    /// </summary>
    [JsonProperty("page_size")]
    public int? PageSize { get; init; } = 20;
}

/// <summary>
/// 列出工作空间的响应。
/// 对应 API 返回的 data 对象结构。
/// </summary>
public record ListWorkspacesResponse
{
    /// <summary>
    /// 获取工作空间列表。
    /// </summary>
    [JsonProperty("workspaces")]
    public IReadOnlyList<Workspace>? Workspaces { get; init; }

    /// <summary>
    /// 获取是否还有更多工作空间。
    /// </summary>
    [JsonProperty("has_more")]
    public bool HasMore { get; init; }

    /// <summary>
    /// 获取总数。
    /// </summary>
    [JsonProperty("total_count")]
    public int? Total { get; init; }
}
