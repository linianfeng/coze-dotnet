namespace Coze.Sdk.Exceptions;

/// <summary>
/// 所有 Coze SDK 异常的基类。
/// </summary>
public class CozeException : Exception
{
    /// <summary>
    /// 获取用于请求追踪的日志 ID。
    /// </summary>
    public string? LogId { get; }

    /// <summary>
    /// 初始化 <see cref="CozeException"/> 类的新实例。
    /// </summary>
    /// <param name="message">错误消息。</param>
    /// <param name="logId">可选的用于追踪的日志 ID。</param>
    public CozeException(string message, string? logId = null) : base(message)
    {
        LogId = logId;
    }

    /// <summary>
    /// 初始化 <see cref="CozeException"/> 类的新实例。
    /// </summary>
    /// <param name="message">错误消息。</param>
    /// <param name="innerException">内部异常。</param>
    /// <param name="logId">可选的用于追踪的日志 ID。</param>
    public CozeException(string message, Exception innerException, string? logId = null)
        : base(message, innerException)
    {
        LogId = logId;
    }
}
