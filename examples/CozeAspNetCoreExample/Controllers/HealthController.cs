using Microsoft.AspNetCore.Mvc;

namespace CozeAspNetCoreExample.Controllers;

/// <summary>
/// 提供健康检查端点
/// </summary>
[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// 返回服务的健康状态
    /// </summary>
    /// <returns>健康状态信息</returns>
    [HttpGet]
    public IActionResult Get()
    {
        // 健康检查是高频调用端点，不记录日志以避免日志膨胀
        return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
    }
}