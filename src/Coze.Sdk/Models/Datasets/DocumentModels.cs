using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Coze.Sdk.Models.Datasets;

/// <summary>
/// 文档状态。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum DocumentStatus
{
    /// <summary>
    /// 未知状态。
    /// </summary>
    [EnumMember(Value = "0")]
    Unknown = 0,

    /// <summary>
    /// 初始化中。
    /// </summary>
    [EnumMember(Value = "1")]
    Initializing = 1,

    /// <summary>
    /// 处理中。
    /// </summary>
    [EnumMember(Value = "2")]
    Processing = 2,

    /// <summary>
    /// 已完成。
    /// </summary>
    [EnumMember(Value = "3")]
    Completed = 3,

    /// <summary>
    /// 失败。
    /// </summary>
    [EnumMember(Value = "4")]
    Failed = 4
}

/// <summary>
/// 文档来源类型。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum DocumentSourceType
{
    /// <summary>
    /// 文件上传。
    /// </summary>
    [EnumMember(Value = "file_upload")]
    FileUpload,

    /// <summary>
    /// 网页。
    /// </summary>
    [EnumMember(Value = "web_page")]
    WebPage,

    /// <summary>
    /// 记事本。
    /// </summary>
    [EnumMember(Value = "notepad")]
    Notepad
}

/// <summary>
/// 文档分块策略。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum DocumentChunkStrategy
{
    /// <summary>
    /// 自动分块。
    /// </summary>
    [EnumMember(Value = "auto")]
    Auto,

    /// <summary>
    /// 手动分块。
    /// </summary>
    [EnumMember(Value = "manual")]
    Manual
}

/// <summary>
/// 文档更新类型。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum DocumentUpdateType
{
    /// <summary>
    /// 自动更新。
    /// </summary>
    [EnumMember(Value = "0")]
    Auto = 0,

    /// <summary>
    /// 手动更新。
    /// </summary>
    [EnumMember(Value = "1")]
    Manual = 1
}

/// <summary>
/// 文档模型。
/// </summary>
public record Document
{
    /// <summary>
    /// 获取文档 ID。
    /// </summary>
    [JsonProperty("document_id")]
    public string? DocumentId { get; init; }

    /// <summary>
    /// 获取数据集 ID。
    /// </summary>
    [JsonProperty("dataset_id")]
    public string? DatasetId { get; init; }

    /// <summary>
    /// 获取文档名称。
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; init; }

    /// <summary>
    /// 获取来源信息。
    /// </summary>
    [JsonProperty("source_info")]
    public DocumentSourceInfo? SourceInfo { get; init; }

    /// <summary>
    /// 获取状态。
    /// </summary>
    [JsonProperty("status")]
    public DocumentStatus? Status { get; init; }

    /// <summary>
    /// 获取分块策略。
    /// </summary>
    [JsonProperty("chunk_strategy")]
    public DocumentChunkStrategy? ChunkStrategy { get; init; }

    /// <summary>
    /// 获取更新类型。
    /// </summary>
    [JsonProperty("update_type")]
    public DocumentUpdateType? UpdateType { get; init; }

    /// <summary>
    /// 获取字符数。
    /// </summary>
    [JsonProperty("char_count")]
    public int? CharCount { get; init; }

    /// <summary>
    /// 获取分段数。
    /// </summary>
    [JsonProperty("segment_count")]
    public int? SegmentCount { get; init; }

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
/// 文档来源信息。
/// </summary>
public record DocumentSourceInfo
{
    /// <summary>
    /// 获取来源类型。
    /// </summary>
    [JsonProperty("source_type")]
    public DocumentSourceType? SourceType { get; init; }

    /// <summary>
    /// 获取文件 ID。
    /// </summary>
    [JsonProperty("file_id")]
    public string? FileId { get; init; }

    /// <summary>
    /// 获取网页 URL。
    /// </summary>
    [JsonProperty("web_url")]
    public string? WebUrl { get; init; }

    /// <summary>
    /// 获取内容（用于记事本类型）。
    /// </summary>
    [JsonProperty("content")]
    public string? Content { get; init; }
}

/// <summary>
/// 创建文档的请求。
/// </summary>
public record CreateDocumentRequest
{
    /// <summary>
    /// 获取数据集 ID。
    /// </summary>
    [JsonProperty("dataset_id")]
    public required string DatasetId { get; init; }

    /// <summary>
    /// 获取文档名称。
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; init; }

    /// <summary>
    /// 获取来源信息。
    /// </summary>
    [JsonProperty("source_info")]
    public DocumentSourceInfo? SourceInfo { get; init; }

    /// <summary>
    /// 获取分块策略。
    /// </summary>
    [JsonProperty("chunk_strategy")]
    public DocumentChunkStrategy? ChunkStrategy { get; init; }
}

/// <summary>
/// 创建文档的响应。
/// </summary>
public record CreateDocumentResponse
{
    /// <summary>
    /// 获取文档。
    /// </summary>
    [JsonProperty("data")]
    public Document? Document { get; init; }
}

/// <summary>
/// 列出文档的请求。
/// </summary>
public record ListDocumentsRequest
{
    /// <summary>
    /// 获取数据集 ID。
    /// </summary>
    [JsonProperty("dataset_id")]
    public required string DatasetId { get; init; }

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
/// 列出文档的响应。
/// </summary>
public record ListDocumentsResponse
{
    /// <summary>
    /// 获取文档列表。
    /// </summary>
    [JsonProperty("data")]
    public IReadOnlyList<Document>? Documents { get; init; }

    /// <summary>
    /// 获取总数。
    /// </summary>
    [JsonProperty("total")]
    public int? Total { get; init; }
}

/// <summary>
/// 更新文档的请求。
/// </summary>
public record UpdateDocumentRequest
{
    /// <summary>
    /// 获取数据集 ID。
    /// </summary>
    [JsonProperty("dataset_id")]
    public required string DatasetId { get; init; }

    /// <summary>
    /// 获取文档 ID。
    /// </summary>
    [JsonProperty("document_id")]
    public required string DocumentId { get; init; }

    /// <summary>
    /// 获取文档名称。
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; init; }
}

/// <summary>
/// 更新文档的响应。
/// </summary>
public record UpdateDocumentResponse
{
    /// <summary>
    /// 获取文档。
    /// </summary>
    [JsonProperty("data")]
    public Document? Document { get; init; }
}

/// <summary>
/// 删除文档的请求。
/// </summary>
public record DeleteDocumentRequest
{
    /// <summary>
    /// 获取数据集 ID。
    /// </summary>
    [JsonProperty("dataset_id")]
    public required string DatasetId { get; init; }

    /// <summary>
    /// 获取文档 ID。
    /// </summary>
    [JsonProperty("document_id")]
    public required string DocumentId { get; init; }
}
