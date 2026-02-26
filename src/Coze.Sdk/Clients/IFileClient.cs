using Coze.Sdk.Models.Files;

namespace Coze.Sdk.Clients;

/// <summary>
/// 文件操作接口。
/// 对应 Java SDK 中的 FileService.java。
/// </summary>
public interface IFileClient
{
    /// <summary>
    /// 上传文件。
    /// </summary>
    Task<UploadFileResponse> UploadAsync(UploadFileRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 从路径上传文件。
    /// </summary>
    Task<UploadFileResponse> UploadFromPathAsync(string filePath, string? fileName = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 从流上传文件。
    /// </summary>
    Task<UploadFileResponse> UploadFromStreamAsync(Stream stream, string fileName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取文件信息。
    /// </summary>
    Task<CozeFile> RetrieveAsync(string fileId, CancellationToken cancellationToken = default);
}
