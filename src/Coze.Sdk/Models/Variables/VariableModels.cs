using Newtonsoft.Json;

namespace Coze.Sdk.Models.Variables;

/// <summary>
/// 变量值模型。
/// 对应 Java SDK 中的 VariableValue.java。
/// </summary>
public record VariableValue
{
    /// <summary>
    /// 获取变量名称/关键词。
    /// </summary>
    [JsonProperty("keyword")]
    public string? Keyword { get; init; }

    /// <summary>
    /// 获取变量值。
    /// </summary>
    [JsonProperty("value")]
    public string? Value { get; init; }

    /// <summary>
    /// 获取创建时间戳。
    /// </summary>
    [JsonProperty("create_time")]
    public long? CreateTime { get; init; }

    /// <summary>
    /// 获取更新时间戳。
    /// </summary>
    [JsonProperty("update_time")]
    public long? UpdateTime { get; init; }
}

/// <summary>
/// 更新变量的请求。
/// 对应 Java SDK 中的 UpdateVariableReq.java。
/// </summary>
public record UpdateVariablesRequest
{
    /// <summary>
    /// 获取应用 ID。
    /// </summary>
    [JsonProperty("app_id")]
    public string? AppId { get; init; }

    /// <summary>
    /// 获取 Bot ID。
    /// </summary>
    [JsonProperty("bot_id")]
    public string? BotId { get; init; }

    /// <summary>
    /// 获取连接器 ID（默认：1024）。
    /// </summary>
    [JsonProperty("connector_id")]
    public string ConnectorId { get; init; } = "1024";

    /// <summary>
    /// 获取连接器 UID。
    /// </summary>
    [JsonProperty("connector_uid")]
    public required string ConnectorUid { get; init; }

    /// <summary>
    /// 获取要更新的变量数据。
    /// </summary>
    [JsonProperty("data")]
    public required IReadOnlyList<VariableValue> Data { get; init; }
}

/// <summary>
/// 更新变量的响应。
/// 对应 Java SDK 中的 UpdateVariableResp.java。
/// </summary>
public record UpdateVariablesResponse
{
    // 空响应，继承自 BaseResp
}

/// <summary>
/// 获取变量的请求。
/// 对应 Java SDK 中的 RetrieveVariableReq.java。
/// </summary>
public record RetrieveVariablesRequest
{
    /// <summary>
    /// 获取应用 ID。
    /// </summary>
    [JsonProperty("app_id")]
    public string? AppId { get; init; }

    /// <summary>
    /// 获取 Bot ID。
    /// </summary>
    [JsonProperty("bot_id")]
    public string? BotId { get; init; }

    /// <summary>
    /// 获取连接器 ID。
    /// </summary>
    [JsonProperty("connector_id")]
    public string? ConnectorId { get; init; }

    /// <summary>
    /// 获取连接器 UID。
    /// </summary>
    [JsonProperty("connector_uid")]
    public string? ConnectorUid { get; init; }

    /// <summary>
    /// 获取过滤关键词。
    /// </summary>
    [JsonProperty("keywords")]
    public string? Keywords { get; init; }
}

/// <summary>
/// 获取变量的响应。
/// 对应 Java SDK 中的 RetrieveVariableResp.java。
/// </summary>
public record RetrieveVariablesResponse
{
    /// <summary>
    /// 获取变量项列表。
    /// </summary>
    [JsonProperty("items")]
    public IReadOnlyList<VariableValue>? Items { get; init; }
}
