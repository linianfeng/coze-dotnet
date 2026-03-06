using System.ComponentModel.DataAnnotations;

namespace CozeAspNetCoreExample.Models;

/// <summary>
/// 聊天请求模型
/// </summary>
public class ChatRequest
{
    /// <summary>
    /// 机器人的ID
    /// </summary>
    [Required(ErrorMessage = "BotId is required")]
    public string BotId { get; set; } = string.Empty;

    /// <summary>
    /// 发送给机器人的消息
    /// </summary>
    [Required(ErrorMessage = "Message is required")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 用户的ID（可选）
    /// </summary>
    public string? UserId { get; set; }
}
