using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Coze.Sdk.Models.Commerce;

#region Enums

/// <summary>
/// 权益状态。
/// 对应 Java SDK 中的 BenefitStatus.java。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum BenefitStatus
{
    /// <summary>
    /// 有效状态。
    /// </summary>
    [EnumMember(Value = "valid")]
    Valid,

    /// <summary>
    /// 冻结状态。
    /// </summary>
    [EnumMember(Value = "frozen")]
    Frozen
}

/// <summary>
/// 权益类型。
/// 对应 Java SDK 中的 BenefitType.java。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum BenefitType
{
    /// <summary>
    /// 资源点类型。
    /// </summary>
    [EnumMember(Value = "resource_point")]
    ResourcePoint
}

/// <summary>
/// 权益激活模式。
/// 对应 Java SDK 中的 ActiveMode.java。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum ActiveMode
{
    /// <summary>
    /// 绝对时间模式。
    /// </summary>
    [EnumMember(Value = "absolute_time")]
    AbsoluteTime
}

/// <summary>
/// 权益实体类型。
/// 对应 Java SDK 中的 BenefitEntityType.java。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum BenefitEntityType
{
    /// <summary>
    /// 企业所有设备。
    /// </summary>
    [EnumMember(Value = "enterprise_all_devices")]
    EnterpriseAllDevices,

    /// <summary>
    /// 企业所有标识符。
    /// </summary>
    [EnumMember(Value = "enterprise_all_identifiers")]
    EnterpriseAllIdentifiers,

    /// <summary>
    /// 单个设备。
    /// </summary>
    [EnumMember(Value = "single_device")]
    SingleDevice,

    /// <summary>
    /// 单个标识符。
    /// </summary>
    [EnumMember(Value = "single_identifier")]
    SingleIdentifier
}

#endregion

#region Bill Models

/// <summary>
/// 账单任务信息。
/// 对应 Java SDK 中的 BillTaskInfo.java。
/// </summary>
public record BillTaskInfo
{
    /// <summary>
    /// 获取任务 ID。
    /// </summary>
    [JsonProperty("task_id")]
    public string? TaskId { get; init; }

    /// <summary>
    /// 获取文件 URL 列表。
    /// </summary>
    [JsonProperty("file_urls")]
    public IReadOnlyList<string>? FileUrls { get; init; }
}

/// <summary>
/// 创建账单下载任务的请求。
/// 对应 Java SDK 中的 CreateBillDownloadTaskReq.java。
/// </summary>
public record CreateBillDownloadTaskRequest
{
    /// <summary>
    /// 获取开始时间戳。
    /// </summary>
    [JsonProperty("started_at")]
    public required long StartedAt { get; init; }

    /// <summary>
    /// 获取结束时间戳。
    /// </summary>
    [JsonProperty("ended_at")]
    public required long EndedAt { get; init; }
}

/// <summary>
/// 创建账单下载任务的响应。
/// </summary>
public record CreateBillDownloadTaskResponse
{
    /// <summary>
    /// 获取任务信息。
    /// </summary>
    [JsonProperty("data")]
    public BillTaskInfo? TaskInfo { get; init; }
}

/// <summary>
/// 列出账单下载任务的请求。
/// </summary>
public record ListBillDownloadTasksRequest
{
    /// <summary>
    /// 获取过滤的任务 ID 列表。
    /// </summary>
    public IReadOnlyList<string>? TaskIds { get; init; }

    /// <summary>
    /// 获取页码。
    /// </summary>
    public int? PageNumber { get; init; } = 1;

    /// <summary>
    /// 获取每页数量。
    /// </summary>
    public int? PageSize { get; init; } = 20;
}

/// <summary>
/// 列出账单下载任务的响应。
/// 对应 Java SDK 中的 ListBillDownloadTaskResp.java。
/// </summary>
public record ListBillDownloadTasksResponse
{
    /// <summary>
    /// 获取任务信息列表。
    /// </summary>
    [JsonProperty("task_infos")]
    public IReadOnlyList<BillTaskInfo>? TaskInfos { get; init; }

    /// <summary>
    /// 获取总数。
    /// </summary>
    [JsonProperty("total")]
    public int? Total { get; init; }
}

#endregion

#region Benefit Limitation Models

/// <summary>
/// 权益信息。
/// 对应 Java SDK 中的 BenefitInfo.java。
/// </summary>
public record BenefitInfo
{
    /// <summary>
    /// 获取权益 ID。
    /// </summary>
    [JsonProperty("benefit_id")]
    public string? BenefitId { get; init; }

    /// <summary>
    /// 获取权益类型。
    /// </summary>
    [JsonProperty("benefit_type")]
    public BenefitType? BenefitType { get; init; }

    /// <summary>
    /// 获取激活模式。
    /// </summary>
    [JsonProperty("active_mode")]
    public ActiveMode? ActiveMode { get; init; }

    /// <summary>
    /// 获取开始时间戳。
    /// </summary>
    [JsonProperty("started_at")]
    public long? StartedAt { get; init; }

    /// <summary>
    /// 获取结束时间戳。
    /// </summary>
    [JsonProperty("ended_at")]
    public long? EndedAt { get; init; }

    /// <summary>
    /// 获取限额。
    /// </summary>
    [JsonProperty("limit")]
    public int? Limit { get; init; }

    /// <summary>
    /// 获取状态。
    /// </summary>
    [JsonProperty("status")]
    public BenefitStatus? Status { get; init; }
}

/// <summary>
/// 创建权益限制的请求。
/// 对应 Java SDK 中的 CreateBenefitLimitationReq.java。
/// </summary>
public record CreateBenefitLimitationRequest
{
    /// <summary>
    /// 获取实体类型。
    /// </summary>
    [JsonProperty("entity_type")]
    public BenefitEntityType? EntityType { get; init; }

    /// <summary>
    /// 获取实体 ID。
    /// </summary>
    [JsonProperty("entity_id")]
    public string? EntityId { get; init; }

    /// <summary>
    /// 获取权益信息。
    /// </summary>
    [JsonProperty("benefit_info")]
    public BenefitInfo? BenefitInfo { get; init; }
}

/// <summary>
/// 创建权益限制的响应。
/// </summary>
public record CreateBenefitLimitationResponse
{
    /// <summary>
    /// 获取权益信息。
    /// </summary>
    [JsonProperty("data")]
    public BenefitInfo? BenefitInfo { get; init; }
}

/// <summary>
/// 更新权益限制的请求。
/// 对应 Java SDK 中的 UpdateBenefitLimitationReq.java。
/// </summary>
public record UpdateBenefitLimitationRequest
{
    /// <summary>
    /// 获取权益 ID（不序列化，用于 URL 路径）。
    /// </summary>
    [JsonIgnore]
    public required string BenefitId { get; init; }

    /// <summary>
    /// 获取开始时间戳。
    /// </summary>
    [JsonProperty("started_at")]
    public long? StartedAt { get; init; }

    /// <summary>
    /// 获取结束时间戳。
    /// </summary>
    [JsonProperty("ended_at")]
    public long? EndedAt { get; init; }

    /// <summary>
    /// 获取限额。
    /// </summary>
    [JsonProperty("limit")]
    public int? Limit { get; init; }

    /// <summary>
    /// 获取状态。
    /// </summary>
    [JsonProperty("status")]
    public BenefitStatus? Status { get; init; }
}

/// <summary>
/// 更新权益限制的响应。
/// </summary>
public record UpdateBenefitLimitationResponse
{
    /// <summary>
    /// 获取权益信息。
    /// </summary>
    [JsonProperty("data")]
    public BenefitInfo? BenefitInfo { get; init; }
}

/// <summary>
/// 列出权益限制的请求。
/// </summary>
public record ListBenefitLimitationsRequest
{
    /// <summary>
    /// 获取实体类型过滤条件。
    /// </summary>
    public BenefitEntityType? EntityType { get; init; }

    /// <summary>
    /// 获取实体 ID 过滤条件。
    /// </summary>
    public string? EntityId { get; init; }

    /// <summary>
    /// 获取权益类型过滤条件。
    /// </summary>
    public BenefitType? BenefitType { get; init; }

    /// <summary>
    /// 获取状态过滤条件。
    /// </summary>
    public BenefitStatus? Status { get; init; }

    /// <summary>
    /// 获取分页令牌。
    /// </summary>
    public string? PageToken { get; init; }

    /// <summary>
    /// 获取每页数量。
    /// </summary>
    public int? PageSize { get; init; } = 20;
}

/// <summary>
/// 列出权益限制的响应。
/// 对应 Java SDK 中的 ListBenefitLimitationResp.java。
/// </summary>
public record ListBenefitLimitationsResponse
{
    /// <summary>
    /// 获取是否还有更多结果。
    /// </summary>
    [JsonProperty("has_more")]
    public bool HasMore { get; init; }

    /// <summary>
    /// 获取下一页的分页令牌。
    /// </summary>
    [JsonProperty("page_token")]
    public string? PageToken { get; init; }

    /// <summary>
    /// 获取权益信息列表。
    /// </summary>
    [JsonProperty("benefit_infos")]
    public IReadOnlyList<BenefitInfo>? BenefitInfos { get; init; }
}

#endregion
