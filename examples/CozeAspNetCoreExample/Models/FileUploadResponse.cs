namespace CozeAspNetCoreExample.Models;

/// <summary>
/// 文件上传响应模型
/// </summary>
public class FileUploadResponse
{
    /// <summary>
    /// 文件ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 文件名
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 文件访问URL
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long Size { get; set; }
}
