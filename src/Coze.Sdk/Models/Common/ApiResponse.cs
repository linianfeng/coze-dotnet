using Newtonsoft.Json;

namespace Coze.Sdk.Models.Common;

/// <summary>
/// API 响应的基础包装类。
/// 对应 Java SDK 中的 BaseResponse.java。
/// </summary>
/// <typeparam name="T">数据负载的类型。</typeparam>
public record ApiResponse<T>
{
    /// <summary>
    /// 获取响应码。0 表示成功。
    /// </summary>
    [JsonProperty("code")]
    public int Code { get; init; }

    /// <summary>
    /// 获取响应消息。
    /// </summary>
    [JsonProperty("msg")]
    public string? Message { get; init; }

    /// <summary>
    /// 获取响应数据负载。
    /// </summary>
    [JsonProperty("data")]
    public T? Data { get; init; }

    /// <summary>
    /// 获取错误详情（如果有）。
    /// </summary>
    [JsonProperty("detail")]
    public ErrorDetail? Detail { get; init; }

    /// <summary>
    /// 获取一个值，指示响应是否表示成功。
    /// </summary>
    [JsonIgnore]
    public bool IsSuccess => Code == 0;

    /// <summary>
    /// 获取用于请求追踪的日志 ID。
    /// </summary>
    [JsonIgnore]
    public string? LogId => Detail?.LogId;
}
