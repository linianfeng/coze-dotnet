using Microsoft.AspNetCore.Mvc;
using CozeAspNetCoreExample.Models;

namespace CozeAspNetCoreExample.Controllers;

/// <summary>
/// 文件上传控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly ILogger<FileController> _logger;
    private readonly IWebHostEnvironment _environment;
    private static readonly string[] AllowedExtensions = { ".pdf", ".doc", ".docx", ".txt", ".md", ".jpg", ".jpeg", ".png", ".gif" };
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB

    public FileController(ILogger<FileController> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="file">文件</param>
    /// <returns>文件上传响应</returns>
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { Error = "请选择要上传的文件" });
        }

        // 检查文件大小
        if (file.Length > MaxFileSize)
        {
            return BadRequest(new { Error = "文件大小不能超过 10MB" });
        }

        // 检查文件扩展名
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
        {
            return BadRequest(new { Error = "不支持的文件类型" });
        }

        try
        {
            // 创建上传目录
            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // 生成唯一文件名
            var fileId = Guid.NewGuid().ToString("N");
            var fileName = $"{fileId}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // 保存文件
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation("文件上传成功: {FileName}, 大小: {Size} 字节", file.FileName, file.Length);

            return Ok(new FileUploadResponse
            {
                Id = fileId,
                Name = file.FileName,
                Url = $"/api/file/download/{fileName}",
                Size = file.Length
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "文件上传失败");
            return StatusCode(500, new { Error = "文件上传失败" });
        }
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <returns>文件内容</returns>
    [HttpGet("download/{fileName}")]
    public IActionResult Download(string fileName)
    {
        var uploadsFolder = Path.Combine(_environment.ContentRootPath, "uploads");
        var filePath = Path.Combine(uploadsFolder, fileName);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        // 安全检查：确保文件在 uploads 目录内
        var fullPath = Path.GetFullPath(filePath);
        var fullUploadsFolder = Path.GetFullPath(uploadsFolder);
        if (!fullPath.StartsWith(fullUploadsFolder))
        {
            return BadRequest();
        }

        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        var contentType = GetContentType(fileName);
        return File(fileBytes, contentType, fileName);
    }

    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".txt" => "text/plain",
            ".md" => "text/markdown",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            _ => "application/octet-stream"
        };
    }
}
