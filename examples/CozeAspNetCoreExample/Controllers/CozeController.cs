using Microsoft.AspNetCore.Mvc;
using CozeAspNetCoreExample.Services;

namespace CozeAspNetCoreExample.Controllers;

/// <summary>
/// 提供与 Coze 机器人交互的 API 端点
/// </summary>
[ApiController]
[Route("[controller]")]
public class CozeController : ControllerBase
{
    private readonly ICozeService _cozeService;
    private readonly ILogger<CozeController> _logger;

    public CozeController(ICozeService cozeService, ILogger<CozeController> logger)
    {
        _cozeService = cozeService;
        _logger = logger;
    }

    /// <summary>
    /// 与指定的机器人进行聊天
    /// </summary>
    /// <param name="botId">机器人的ID</param>
    /// <param name="message">发送给机器人的消息</param>
    /// <param name="userId">用户的ID（可选）</param>
    /// <returns>包含机器人回复的响应</returns>
    /// <response code="200">成功返回机器人的回复</response>
    /// <response code="400">如果缺少必需的参数</response>
    /// <response code="500">如果处理请求时发生错误</response>
    [HttpGet("chat")]
    public async Task<IActionResult> ChatAsync([FromQuery] string botId, [FromQuery] string message, [FromQuery] string? userId = null)
    {
        if (string.IsNullOrEmpty(botId))
        {
            return BadRequest("BotId is required.");
        }

        if (string.IsNullOrEmpty(message))
        {
            return BadRequest("Message is required.");
        }

        try
        {
            _logger.LogInformation("Sending message to bot {BotId}: {Message}", botId, message);

            var response = await _cozeService.ChatWithBotAsync(botId, message, userId);

            _logger.LogInformation("Received response from bot {BotId}", botId);

            return Ok(new { BotId = botId, Message = message, Response = response });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while chatting with bot {BotId}", botId);
            return StatusCode(500, new { Error = "An error occurred while processing your request", Details = ex.Message });
        }
    }
}