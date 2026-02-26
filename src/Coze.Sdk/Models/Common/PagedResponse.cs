using Newtonsoft.Json;

namespace Coze.Sdk.Models.Common;

/// <summary>
/// 列表 API 的分页响应包装类。
/// 对应 Java SDK 中的 PageResp.java。
/// </summary>
/// <typeparam name="T">响应中项目的类型。</typeparam>
public record PagedResponse<T>
{
    /// <summary>
    /// 获取项目总数。
    /// </summary>
    [JsonProperty("total")]
    public int? Total { get; init; }

    /// <summary>
    /// 获取当前页的项目列表。
    /// </summary>
    [JsonProperty("items")]
    public IReadOnlyList<T>? Items { get; init; } = Array.Empty<T>();

    /// <summary>
    /// 获取一个值，指示是否还有更多项目。
    /// </summary>
    [JsonProperty("has_more")]
    public bool HasMore { get; init; }

    /// <summary>
    /// 获取当前页第一个项目的 ID。
    /// </summary>
    [JsonProperty("first_id")]
    public string? FirstId { get; init; }

    /// <summary>
    /// 获取当前页最后一个项目的 ID。
    /// </summary>
    [JsonProperty("last_id")]
    public string? LastId { get; init; }

    /// <summary>
    /// 获取用于请求追踪的日志 ID。
    /// </summary>
    [JsonIgnore]
    public string? LogId { get; init; }

    /// <summary>
    /// 获取空的分页响应。
    /// </summary>
    public static PagedResponse<T> Empty => new();
}
