namespace Coze.Sdk.Exceptions;

/// <summary>
/// 认证异常的错误码。
/// </summary>
public enum AuthErrorCode
{
    /// <summary>
    /// 无效的客户端凭据。
    /// </summary>
    InvalidClient = 1,

    /// <summary>
    /// 无效的授权类型。
    /// </summary>
    InvalidGrant = 2,

    /// <summary>
    /// 未授权的客户端。
    /// </summary>
    UnauthorizedClient = 3,

    /// <summary>
    /// 访问被拒绝。
    /// </summary>
    AccessDenied = 4,

    /// <summary>
    /// 不支持的响应类型。
    /// </summary>
    UnsupportedResponseType = 5,

    /// <summary>
    /// 无效的作用域。
    /// </summary>
    InvalidScope = 6,

    /// <summary>
    /// 服务器错误。
    /// </summary>
    ServerError = 7,

    /// <summary>
    /// 服务暂时不可用。
    /// </summary>
    TemporarilyUnavailable = 8,

    /// <summary>
    /// 未知错误。
    /// </summary>
    Unknown = 999
}

/// <summary>
/// 当认证错误发生时抛出的异常。
/// </summary>
public class CozeAuthException : CozeException
{
    /// <summary>
    /// 获取认证错误码。
    /// </summary>
    public AuthErrorCode ErrorCode { get; }

    /// <summary>
    /// 获取 HTTP 状态码（如果可用）。
    /// </summary>
    public int? StatusCode { get; }

    /// <summary>
    /// 初始化 <see cref="CozeAuthException"/> 类的新实例。
    /// </summary>
    /// <param name="errorCode">认证错误码。</param>
    /// <param name="message">错误消息。</param>
    /// <param name="logId">可选的用于追踪的日志 ID。</param>
    /// <param name="statusCode">可选的 HTTP 状态码。</param>
    public CozeAuthException(
        AuthErrorCode errorCode,
        string message,
        string? logId = null,
        int? statusCode = null)
        : base(message, logId)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }

    /// <summary>
    /// 初始化 <see cref="CozeAuthException"/> 类的新实例。
    /// </summary>
    /// <param name="errorCode">认证错误码。</param>
    /// <param name="message">错误消息。</param>
    /// <param name="innerException">内部异常。</param>
    /// <param name="logId">可选的用于追踪的日志 ID。</param>
    /// <param name="statusCode">可选的 HTTP 状态码。</param>
    public CozeAuthException(
        AuthErrorCode errorCode,
        string message,
        Exception innerException,
        string? logId = null,
        int? statusCode = null)
        : base(message, innerException, logId)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}
