using Newtonsoft.Json;

namespace Coze.Sdk.Models.Bots;

/// <summary>
/// 列出 Bot 的请求。
/// </summary>
public record ListBotsRequest
{
    /// <summary>
    /// 获取空间 ID。
    /// </summary>
    [JsonProperty("space_id")]
    public required string SpaceId { get; init; }

    /// <summary>
    /// 获取页码（从 1 开始）。
    /// </summary>
    [JsonProperty("page_num")]
    public int? PageNumber { get; init; } = 1;

    /// <summary>
    /// 获取每页数量。
    /// </summary>
    [JsonProperty("page_size")]
    public int? PageSize { get; init; } = 20;
}

/// <summary>
/// 获取 Bot 详情的请求。
/// </summary>
public record RetrieveBotRequest
{
    /// <summary>
    /// 获取 Bot ID。
    /// </summary>
    [JsonProperty("bot_id")]
    public required string BotId { get; init; }
}

/// <summary>
/// 创建 Bot 的请求。
/// </summary>
public record CreateBotRequest
{
    /// <summary>
    /// 获取空间 ID。
    /// </summary>
    [JsonProperty("space_id")]
    public required string SpaceId { get; init; }

    /// <summary>
    /// 获取 Bot 名称。
    /// </summary>
    [JsonProperty("name")]
    public required string Name { get; init; }

    /// <summary>
    /// 获取描述。
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; init; }

    /// <summary>
    /// 获取图标文件 ID。
    /// </summary>
    [JsonProperty("icon_file_id")]
    public string? IconFileId { get; init; }
}

/// <summary>
/// 创建 Bot 的响应。
/// </summary>
public record CreateBotResponse
{
    /// <summary>
    /// 获取 Bot ID。
    /// </summary>
    [JsonProperty("bot_id")]
    public string? BotId { get; init; }
}

/// <summary>
/// 更新 Bot 的请求。
/// </summary>
public record UpdateBotRequest
{
    /// <summary>
    /// 获取 Bot ID。
    /// </summary>
    [JsonProperty("bot_id")]
    public required string BotId { get; init; }

    /// <summary>
    /// 获取 Bot 名称。
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; init; }

    /// <summary>
    /// 获取描述。
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; init; }
}

/// <summary>
/// 更新 Bot 的响应。
/// </summary>
public record UpdateBotResponse
{
    /// <summary>
    /// 获取日志 ID。
    /// </summary>
    [JsonIgnore]
    public string? LogId { get; init; }
}

/// <summary>
/// 发布 Bot 的请求。
/// </summary>
public record PublishBotRequest
{
    /// <summary>
    /// 获取 Bot ID。
    /// </summary>
    [JsonProperty("bot_id")]
    public required string BotId { get; init; }
}

/// <summary>
/// 发布 Bot 的响应。
/// </summary>
public record PublishBotResponse
{
    /// <summary>
    /// 获取 Bot ID。
    /// </summary>
    [JsonProperty("bot_id")]
    public string? BotId { get; init; }
}

/// <summary>
/// 列出 Bot 的响应。
/// </summary>
public record ListBotsResponse
{
    /// <summary>
    /// 获取 Bot 列表。
    /// </summary>
    [JsonProperty("space_bots")]
    public IReadOnlyList<SimpleBot>? Bots { get; init; }

    /// <summary>
    /// 获取总数。
    /// </summary>
    [JsonProperty("total")]
    public int? Total { get; init; }
}

/// <summary>
/// 获取 Bot 详情的响应。
/// </summary>
public record RetrieveBotResponse
{
    /// <summary>
    /// 获取 Bot 信息。
    /// </summary>
    [JsonIgnore]
    public Bot? Bot { get; init; }

    /// <summary>
    /// 获取日志 ID。
    /// </summary>
    [JsonIgnore]
    public string? LogId { get; init; }
}
