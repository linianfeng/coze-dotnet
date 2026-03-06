using Microsoft.AspNetCore.Mvc;
using CozeAspNetCoreExample.Services;
using CozeAspNetCoreExample.Models;
using System.Text;
using System.Text.Json;

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

    // 使用 camelCase 的 JSON 序列化选项
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public CozeController(ICozeService cozeService, ILogger<CozeController> logger)
    {
        _cozeService = cozeService;
        _logger = logger;
    }

    /// <summary>
    /// 与指定的机器人进行聊天
    /// </summary>
    /// <param name="request">聊天请求参数</param>
    /// <returns>包含机器人回复的响应</returns>
    /// <response code="200">成功返回机器人的回复</response>
    /// <response code="400">如果缺少必需的参数</response>
    /// <response code="500">如果处理请求时发生错误</response>
    [HttpGet("chat")]
    public async Task<IActionResult> ChatAsync([FromQuery] Models.ChatRequest request)
    {
        // 模型验证由 ASP.NET Core 自动处理
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            _logger.LogInformation("Sending message to bot {BotId}: {Message}", request.BotId, request.Message);

            var response = await _cozeService.ChatWithBotAsync(request.BotId, request.Message, request.UserId);

            _logger.LogInformation("Received response from bot {BotId}", request.BotId);

            return Ok(new { BotId = request.BotId, Message = request.Message, Response = response });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while chatting with bot {BotId}", request.BotId);
            // 不在生产环境暴露详细错误信息
            return StatusCode(500, new { Error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// 与指定的机器人进行流式聊天
    /// </summary>
    /// <param name="request">聊天请求参数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>Server-Sent Events 流</returns>
    /// <response code="200">成功返回流式响应</response>
    /// <response code="400">如果缺少必需的参数</response>
    [HttpGet("chat/stream")]
    public async Task ChatStreamAsync([FromQuery] Models.ChatRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            await Response.WriteAsync("Invalid request parameters");
            return;
        }

        Response.Headers.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers.Connection = "keep-alive";

        try
        {
            _logger.LogInformation("Starting stream chat with bot {BotId}", request.BotId);

            await foreach (var evt in _cozeService.ChatWithBotStreamAsync(request.BotId, request.Message, request.UserId, cancellationToken))
            {
                // 使用 camelCase 序列化
                var json = JsonSerializer.Serialize(evt, JsonOptions);
                var sseMessage = $"data: {json}\n\n";
                await Response.WriteAsync(sseMessage, Encoding.UTF8, cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }

            // 发送结束标记
            await Response.WriteAsync("data: [DONE]\n\n", Encoding.UTF8, cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);

            _logger.LogInformation("Stream chat completed for bot {BotId}", request.BotId);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Stream chat cancelled for bot {BotId}", request.BotId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during stream chat with bot {BotId}", request.BotId);
            var errorEvent = new StreamChatEvent { EventType = "error", Error = "An error occurred" };
            var errorJson = JsonSerializer.Serialize(errorEvent, JsonOptions);
            await Response.WriteAsync($"data: {errorJson}\n\n", Encoding.UTF8, cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
    }
}
