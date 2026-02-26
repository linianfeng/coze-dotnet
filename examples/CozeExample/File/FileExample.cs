using Coze.Sdk;
using Coze.Sdk.Models.Files;

namespace CozeExample.File;

/// <summary>
/// Demonstrates file operations: upload and retrieve.
/// </summary>
public static class FileExample
{
    public static async Task RunAsync(CozeClient client, string? filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
        {
            Console.WriteLine("FILE_PATH not set or file not found, skipping.");
            return;
        }

        // Upload file
        var uploadResult = await client.Files.UploadAsync(UploadFileRequest.FromPath(filePath));
        var fileId = uploadResult.File?.Id;
        Console.WriteLine($"Uploaded file: {fileId}, size: {uploadResult.File?.Bytes} bytes");

        // Retrieve file info
        var fileInfo = await client.Files.RetrieveAsync(fileId!);
        Console.WriteLine($"Retrieved file: {fileInfo.FileName}");
    }
}
