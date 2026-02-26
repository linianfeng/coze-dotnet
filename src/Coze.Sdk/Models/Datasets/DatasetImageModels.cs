using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Coze.Sdk.Models.Datasets;

/// <summary>
/// 数据集中的图片状态。
/// 对应 Java SDK 中的 ImageStatus.java。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum ImageStatus
{
    /// <summary>
    /// 处理中。
    /// </summary>
    [EnumMember(Value = "0")]
    InProcessing = 0,

    /// <summary>
    /// 已完成。
    /// </summary>
    [EnumMember(Value = "1")]
    Completed = 1,

    /// <summary>
    /// 处理失败。
    /// </summary>
    [EnumMember(Value = "9")]
    ProcessingFailed = 9
}

/// <summary>
/// 数据集中的图片模型。
/// 对应 Java SDK 中的 Image.java。
/// </summary>
public record DatasetImage
{
    /// <summary>
    /// 获取文档/图片 ID。
    /// </summary>
    [JsonProperty("document_id")]
    public string? DocumentId { get; init; }

    /// <summary>
    /// 获取图片 URL。
    /// </summary>
    [JsonProperty("url")]
    public string? Url { get; init; }

    /// <summary>
    /// 获取图片名称。
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; init; }

    /// <summary>
    /// 获取图片大小（字节）。
    /// </summary>
    [JsonProperty("size")]
    public int? Size { get; init; }

    /// <summary>
    /// 获取文件格式（扩展名，如 jpg、png）。
    /// </summary>
    [JsonProperty("type")]
    public string? Type { get; init; }

    /// <summary>
    /// 获取图片状态。
    /// </summary>
    [JsonProperty("status")]
    public ImageStatus? Status { get; init; }

    /// <summary>
    /// 获取图片说明/描述。
    /// </summary>
    [JsonProperty("caption")]
    public string? Caption { get; init; }

    /// <summary>
    /// 获取创建者 ID。
    /// </summary>
    [JsonProperty("creator_id")]
    public string? CreatorId { get; init; }

    /// <summary>
    /// 获取来源类型。
    /// </summary>
    [JsonProperty("source_type")]
    public DocumentSourceType? SourceType { get; init; }

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
}

/// <summary>
/// 更新数据集中图片的请求。
/// 对应 Java SDK 中的 UpdateImageReq.java。
/// </summary>
public record UpdateDatasetImageRequest
{
    /// <summary>
    /// 获取图片说明/描述。
    /// </summary>
    [JsonProperty("caption")]
    public string? Caption { get; init; }
}

/// <summary>
/// 更新数据集中图片的响应。
/// </summary>
public record UpdateDatasetImageResponse
{
    // 空响应
}

/// <summary>
/// 列出数据集中图片的请求。
/// </summary>
public record ListDatasetImagesRequest
{
    /// <summary>
    /// 获取数据集 ID。
    /// </summary>
    public required string DatasetId { get; init; }

    /// <summary>
    /// 获取关键词过滤条件。
    /// </summary>
    public string? Keyword { get; init; }

    /// <summary>
    /// 获取是否按有说明过滤。
    /// </summary>
    public bool? HasCaption { get; init; }

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
/// 列出数据集中图片的响应。
/// 对应 Java SDK 中的 ListImageResp.java。
/// </summary>
public record ListDatasetImagesResponse
{
    /// <summary>
    /// 获取图片列表。
    /// </summary>
    [JsonProperty("photo_infos")]
    public IReadOnlyList<DatasetImage>? Images { get; init; }

    /// <summary>
    /// 获取总数。
    /// </summary>
    [JsonProperty("total_count")]
    public int? TotalCount { get; init; }
}
