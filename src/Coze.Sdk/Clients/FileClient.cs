using Coze.Sdk.Http;
using Coze.Sdk.Models.Files;

namespace Coze.Sdk.Clients;

/// <summary>
/// 文件操作的实现。
/// 对应 Java SDK 中的 FileService.java。
/// </summary>
internal class FileClient : IFileClient
{
    private readonly CozeHttpClient _httpClient;

    public FileClient(CozeHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UploadFileResponse> UploadAsync(UploadFileRequest request, CancellationToken cancellationToken = default)
    {
        if (request.FileStream != null)
        {
            return await UploadFromStreamAsync(request.FileStream, request.FileName ?? "file", cancellationToken);
        }

        if (!string.IsNullOrEmpty(request.FilePath))
        {
            return await UploadFromPathAsync(request.FilePath, request.FileName, cancellationToken);
        }

        throw new ArgumentException("必须提供 FilePath 或 FileStream");
    }

    public async Task<UploadFileResponse> UploadFromPathAsync(string filePath, string? fileName = null, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"文件未找到: {filePath}");
        }

        fileName ??= Path.GetFileName(filePath);

        using var fileStream = File.OpenRead(filePath);
        return await UploadFromStreamAsync(fileStream, fileName, cancellationToken);
    }

    public async Task<UploadFileResponse> UploadFromStreamAsync(Stream stream, string fileName, CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest(ApiEndpoints.FileUpload, HttpMethodType.Post);

        var fileInfo = await _httpClient.ExecuteMultipartAsync<CozeFile>(httpRequest, stream, fileName, cancellationToken);

        return new UploadFileResponse
        {
            File = fileInfo
        };
    }

    public async Task<CozeFile> RetrieveAsync(string fileId, CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateGetRequest(ApiEndpoints.FileRetrieve);
        httpRequest.AddQueryParameter("file_id", fileId);

        return await _httpClient.ExecuteAsync<CozeFile>(httpRequest, cancellationToken);
    }
}
