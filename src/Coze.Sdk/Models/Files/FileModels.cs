using Newtonsoft.Json;

namespace Coze.Sdk.Models.Files;

/// <summary>
/// 文件信息模型。
/// 对应 Java SDK 中的 FileInfo.java。
/// </summary>
public record CozeFile
{
    /// <summary>
    /// 获取文件 ID。
    /// </summary>
    [JsonProperty("id")]
    public string? Id { get; init; }

    /// <summary>
    /// 获取文件名称。
    /// </summary>
    [JsonProperty("file_name")]
    public string? FileName { get; init; }

    /// <summary>
    /// 获取文件大小（字节）。
    /// </summary>
    [JsonProperty("bytes")]
    public int? Bytes { get; init; }

    /// <summary>
    /// 获取创建时间戳。
    /// </summary>
    [JsonProperty("created_at")]
    public int? CreatedAt { get; init; }
}

/// <summary>
/// 上传文件的请求。
/// </summary>
public record UploadFileRequest
{
    /// <summary>
    /// 获取文件路径（内部使用）。
    /// </summary>
    internal string? FilePath { get; init; }

    /// <summary>
    /// 获取文件名称。
    /// </summary>
    internal string? FileName { get; init; }

    /// <summary>
    /// 获取文件流。
    /// </summary>
    internal Stream? FileStream { get; init; }

    /// <summary>
    /// 从文件路径创建上传请求。
    /// </summary>
    public static UploadFileRequest FromPath(string filePath, string? fileName = null)
    {
        return new UploadFileRequest
        {
            FilePath = filePath,
            FileName = fileName ?? Path.GetFileName(filePath)
        };
    }

    /// <summary>
    /// 从流创建上传请求。
    /// </summary>
    public static UploadFileRequest FromStream(Stream stream, string fileName)
    {
        return new UploadFileRequest
        {
            FileStream = stream,
            FileName = fileName
        };
    }
}

/// <summary>
/// 上传文件的响应。
/// </summary>
public record UploadFileResponse
{
    /// <summary>
    /// 获取文件信息。
    /// </summary>
    [JsonProperty("data")]
    public CozeFile? File { get; init; }

    /// <summary>
    /// 获取日志 ID。
    /// </summary>
    [JsonIgnore]
    public string? LogId { get; init; }
}

/// <summary>
/// 获取文件的请求。
/// </summary>
public record RetrieveFileRequest
{
    /// <summary>
    /// 获取文件 ID。
    /// </summary>
    [JsonProperty("file_id")]
    public required string FileId { get; init; }
}

/// <summary>
/// 获取文件的响应。
/// </summary>
public record RetrieveFileResponse
{
    /// <summary>
    /// 获取文件信息。
    /// </summary>
    [JsonProperty("data")]
    public CozeFile? File { get; init; }

    /// <summary>
    /// 获取日志 ID。
    /// </summary>
    [JsonIgnore]
    public string? LogId { get; init; }
}
