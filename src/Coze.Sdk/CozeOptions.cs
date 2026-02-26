using Coze.Sdk.Authentication;

namespace Coze.Sdk;

/// <summary>
/// Coze SDK 客户端的配置选项。
/// </summary>
public record CozeOptions
{
    /// <summary>
    /// Coze API 的基础 URL。
    /// 默认值为 https://api.coze.cn
    /// </summary>
    public string BaseUrl { get; init; } = "https://api.coze.cn";

    /// <summary>
    /// API 请求的读取超时时间。
    /// 默认值为 633 秒（10 分 33 秒）。
    /// </summary>
    public TimeSpan ReadTimeout { get; init; } = TimeSpan.FromSeconds(633);

    /// <summary>
    /// API 请求的连接超时时间。
    /// 默认值为 5 秒。
    /// </summary>
    public TimeSpan ConnectTimeout { get; init; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// 用于 API 请求的认证提供程序。
    /// </summary>
    public Auth? Auth { get; init; }

    /// <summary>
    /// 可选的日志记录器工厂。
    /// </summary>
    public Microsoft.Extensions.Logging.ILoggerFactory? LoggerFactory { get; init; }
}
