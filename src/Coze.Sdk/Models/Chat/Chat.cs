using Coze.Sdk.Utils;
using Newtonsoft.Json;

namespace Coze.Sdk.Models.Chat;

/// <summary>
/// 表示 Chat 会话。
/// 对应 Java SDK 中的 Chat.java。
/// </summary>
public record Chat
{
    /// <summary>
    /// 获取 Chat ID。
    /// </summary>
    [JsonProperty("id")]
    public string? Id { get; init; }

    /// <summary>
    /// 获取会话 ID。
    /// </summary>
    [JsonProperty("conversation_id")]
    public string? ConversationId { get; init; }

    /// <summary>
    /// 获取 Bot ID。
    /// </summary>
    [JsonProperty("bot_id")]
    public string? BotId { get; init; }

    /// <summary>
    /// 获取创建时间戳（秒）。
    /// </summary>
    [JsonProperty("created_at")]
    public int? CreatedAt { get; init; }

    /// <summary>
    /// 获取完成时间戳（秒）。
    /// </summary>
    [JsonProperty("completed_at")]
    public int? CompletedAt { get; init; }

    /// <summary>
    /// 获取失败时间戳（秒）。
    /// </summary>
    [JsonProperty("failed_at")]
    public int? FailedAt { get; init; }

    /// <summary>
    /// 获取附加元数据。
    /// </summary>
    [JsonProperty("meta_data")]
    public IReadOnlyDictionary<string, string>? MetaData { get; init; }

    /// <summary>
    /// 获取最后的错误信息。
    /// </summary>
    [JsonProperty("last_error")]
    public ChatError? LastError { get; init; }

    /// <summary>
    /// 获取 Chat 状态。
    /// </summary>
    [JsonProperty("status")]
    public ChatStatus? Status { get; init; }

    /// <summary>
    /// 获取必需操作信息。
    /// </summary>
    [JsonProperty("required_action")]
    public ChatRequiredAction? RequiredAction { get; init; }

    /// <summary>
    /// 获取 Token 使用信息。
    /// </summary>
    [JsonProperty("usage")]
    public ChatUsage? Usage { get; init; }

    /// <summary>
    /// 从 JSON 字符串创建 Chat 实例。
    /// </summary>
    /// <param name="json">JSON 字符串。</param>
    /// <returns>新的 Chat 实例。</returns>
    public static Chat? FromJson(string json)
    {
        return JsonHelper.DeserializeObject<Chat>(json);
    }
}
