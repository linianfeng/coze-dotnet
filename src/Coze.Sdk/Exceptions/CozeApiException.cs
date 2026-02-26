namespace Coze.Sdk.Exceptions;

/// <summary>
/// 当 API 错误发生时抛出的异常。
/// </summary>
public class CozeApiException : CozeException
{
    /// <summary>
    /// 获取 HTTP 状态码。
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// 获取 API 响应中的错误码。
    /// </summary>
    public int ErrorCode { get; }

    /// <summary>
    /// 获取原始响应内容。
    /// </summary>
    public string? RawResponse { get; }

    /// <summary>
    /// 初始化 <see cref="CozeApiException"/> 类的新实例。
    /// </summary>
    /// <param name="statusCode">HTTP 状态码。</param>
    /// <param name="errorCode">API 错误码。</param>
    /// <param name="message">错误消息。</param>
    /// <param name="logId">可选的用于追踪的日志 ID。</param>
    /// <param name="rawResponse">可选的原始响应内容。</param>
    public CozeApiException(
        int statusCode,
        int errorCode,
        string message,
        string? logId = null,
        string? rawResponse = null)
        : base(message, logId)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        RawResponse = rawResponse;
    }

    /// <summary>
    /// 初始化 <see cref="CozeApiException"/> 类的新实例。
    /// </summary>
    /// <param name="statusCode">HTTP 状态码。</param>
    /// <param name="errorCode">API 错误码。</param>
    /// <param name="message">错误消息。</param>
    /// <param name="innerException">内部异常。</param>
    /// <param name="logId">可选的用于追踪的日志 ID。</param>
    /// <param name="rawResponse">可选的原始响应内容。</param>
    public CozeApiException(
        int statusCode,
        int errorCode,
        string message,
        Exception innerException,
        string? logId = null,
        string? rawResponse = null)
        : base(message, innerException, logId)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        RawResponse = rawResponse;
    }
}
