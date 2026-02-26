using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Coze.Sdk.Models.Datasets;

/// <summary>
/// 数据集状态。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum DatasetStatus
{
    /// <summary>
    /// 未知状态。
    /// </summary>
    [EnumMember(Value = "0")]
    Unknown = 0,

    /// <summary>
    /// 已启用。
    /// </summary>
    [EnumMember(Value = "1")]
    Enabled = 1,

    /// <summary>
    /// 已禁用。
    /// </summary>
    [EnumMember(Value = "3")]
    Disabled = 3
}

/// <summary>
/// 文档格式类型。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum DocumentFormatType
{
    /// <summary>
    /// 文本格式。
    /// </summary>
    [EnumMember(Value = "0")]
    Text = 0,

    /// <summary>
    /// 表格格式。
    /// </summary>
    [EnumMember(Value = "1")]
    Table = 1,

    /// <summary>
    /// 图片格式。
    /// </summary>
    [EnumMember(Value = "2")]
    Image = 2
}

/// <summary>
/// 数据集模型。
/// 对应 Java SDK 中的 Dataset.java。
/// </summary>
public record Dataset
{
    /// <summary>
    /// 获取数据集 ID。
    /// </summary>
    [JsonProperty("dataset_id")]
    public string? DatasetId { get; init; }

    /// <summary>
    /// 获取数据集名称。
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; init; }

    /// <summary>
    /// 获取描述。
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; init; }

    /// <summary>
    /// 获取空间 ID。
    /// </summary>
    [JsonProperty("space_id")]
    public string? SpaceId { get; init; }

    /// <summary>
    /// 获取状态。
    /// </summary>
    [JsonProperty("status")]
    public DatasetStatus? Status { get; init; }

    /// <summary>
    /// 获取格式类型。
    /// </summary>
    [JsonProperty("format_type")]
    public DocumentFormatType? FormatType { get; init; }

    /// <summary>
    /// 获取是否可编辑。
    /// </summary>
    [JsonProperty("can_edit")]
    public bool? CanEdit { get; init; }

    /// <summary>
    /// 获取图标 URL。
    /// </summary>
    [JsonProperty("icon_url")]
    public string? IconUrl { get; init; }

    /// <summary>
    /// 获取文档数量。
    /// </summary>
    [JsonProperty("doc_count")]
    public int? DocCount { get; init; }

    /// <summary>
    /// 获取文件列表。
    /// </summary>
    [JsonProperty("file_list")]
    public IReadOnlyList<string>? FileList { get; init; }

    /// <summary>
    /// 获取命中次数。
    /// </summary>
    [JsonProperty("hit_count")]
    public int? HitCount { get; init; }

    /// <summary>
    /// 获取 Bot 使用次数。
    /// </summary>
    [JsonProperty("bot_used_count")]
    public int? BotUsedCount { get; init; }

    /// <summary>
    /// 获取切片数量。
    /// </summary>
    [JsonProperty("slice_count")]
    public int? SliceCount { get; init; }

    /// <summary>
    /// 获取总文件大小。
    /// </summary>
    [JsonProperty("all_file_size")]
    public string? AllFileSize { get; init; }

    /// <summary>
    /// 获取创建者 ID。
    /// </summary>
    [JsonProperty("creator_id")]
    public string? CreatorId { get; init; }

    /// <summary>
    /// 获取创建者名称。
    /// </summary>
    [JsonProperty("creator_name")]
    public string? CreatorName { get; init; }

    /// <summary>
    /// 获取创建时间戳。
    /// </summary>
    [JsonProperty("create_time")]
    public long? CreateTime { get; init; }

    /// <summary>
    /// 获取更新时间戳。
    /// </summary>
    [JsonProperty("update_time")]
    public long? UpdateTime { get; init; }
}

/// <summary>
/// 创建数据集的请求。
/// </summary>
public record CreateDatasetRequest
{
    /// <summary>
    /// 获取数据集名称。
    /// </summary>
    [JsonProperty("name")]
    public required string Name { get; init; }

    /// <summary>
    /// 获取空间 ID。
    /// </summary>
    [JsonProperty("space_id")]
    public required string SpaceId { get; init; }

    /// <summary>
    /// 获取描述。
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; init; }

    /// <summary>
    /// 获取格式类型。
    /// </summary>
    [JsonProperty("format_type")]
    public DocumentFormatType? FormatType { get; init; }
}

/// <summary>
/// 创建数据集的响应。
/// </summary>
public record CreateDatasetResponse
{
    /// <summary>
    /// 获取数据集。
    /// </summary>
    [JsonProperty("data")]
    public Dataset? Dataset { get; init; }
}

/// <summary>
/// 列出数据集的请求。
/// </summary>
public record ListDatasetsRequest
{
    /// <summary>
    /// 获取空间 ID。
    /// </summary>
    [JsonProperty("space_id")]
    public required string SpaceId { get; init; }

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

    /// <summary>
    /// 获取按名称过滤。
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; init; }
}

/// <summary>
/// 列出数据集的响应（API 返回的 data 部分）。
/// </summary>
internal record ListDatasetsDataResponse
{
    [JsonProperty("total_count")]
    public int? TotalCount { get; init; }

    [JsonProperty("dataset_list")]
    public IReadOnlyList<Dataset>? DatasetList { get; init; }
}

/// <summary>
/// 列出数据集的响应。
/// </summary>
public record ListDatasetsResponse
{
    /// <summary>
    /// 获取数据集列表。
    /// </summary>
    [JsonIgnore]
    public IReadOnlyList<Dataset>? Datasets { get; init; }

    /// <summary>
    /// 获取总数。
    /// </summary>
    [JsonIgnore]
    public int? Total { get; init; }
}

/// <summary>
/// 更新数据集的请求。
/// </summary>
public record UpdateDatasetRequest
{
    /// <summary>
    /// 获取数据集 ID。
    /// </summary>
    [JsonProperty("dataset_id")]
    public required string DatasetId { get; init; }

    /// <summary>
    /// 获取名称。
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; init; }

    /// <summary>
    /// 获取描述。
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; init; }
}

/// <summary>
/// 更新数据集的响应（无 data 字段）。
/// </summary>
public record UpdateDatasetResponse;

/// <summary>
/// 删除数据集的请求。
/// </summary>
public record DeleteDatasetRequest
{
    /// <summary>
    /// 获取数据集 ID。
    /// </summary>
    [JsonProperty("dataset_id")]
    public required string DatasetId { get; init; }
}
