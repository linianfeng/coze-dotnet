using Microsoft.AspNetCore.Mvc;

namespace CozeAspNetCoreExample.Controllers;

/// <summary>
/// 提供健康检查端点
/// </summary>
[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 返回服务的健康状态
    /// </summary>
    /// <returns>健康状态信息</returns>
    [HttpGet]
    public IActionResult Get()
    {
        _logger.LogInformation("Health check endpoint accessed");
        return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
    }
}