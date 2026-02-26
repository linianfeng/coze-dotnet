using Coze.Sdk.Exceptions;
using Microsoft.Extensions.Logging;

namespace Coze.Sdk.Authentication;

/// <summary>
/// 设备授权请求的响应。
/// </summary>
public record DeviceAuthResponse
{
    /// <summary>
    /// 获取设备码。
    /// </summary>
    [Newtonsoft.Json.JsonProperty("device_code")]
    public string? DeviceCode { get; init; }

    /// <summary>
    /// 获取用户码。
    /// </summary>
    [Newtonsoft.Json.JsonProperty("user_code")]
    public string? UserCode { get; init; }

    /// <summary>
    /// 获取验证 URI。
    /// </summary>
    [Newtonsoft.Json.JsonProperty("verification_uri")]
    public string? VerificationUri { get; init; }

    /// <summary>
    /// 获取验证 URL（包含 user_code）。
    /// </summary>
    [Newtonsoft.Json.JsonProperty("verification_url")]
    public string? VerificationUrl { get; init; }

    /// <summary>
    /// 获取过期时间（秒）。
    /// </summary>
    [Newtonsoft.Json.JsonProperty("expires_in")]
    public int? ExpiresIn { get; init; }

    /// <summary>
    /// 获取轮询间隔（秒）。
    /// </summary>
    [Newtonsoft.Json.JsonProperty("interval")]
    public int? Interval { get; init; }

    internal DeviceAuthResponse WithVerificationUrl()
    {
        return this with
        {
            VerificationUrl = $"{VerificationUri}?user_code={UserCode}"
        };
    }
}

/// <summary>
/// 使用设备码流程的设备 OAuth 客户端。
/// </summary>
public class DeviceOAuthClient : OAuthClient
{
    private readonly ILogger? _logger;

    /// <summary>
    /// 初始化 <see cref="DeviceOAuthClient"/> 类的新实例。
    /// </summary>
    public DeviceOAuthClient(OAuthOptions options, ILogger? logger = null) : base(options)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取设备授权流程的设备码。
    /// </summary>
    public async Task<DeviceAuthResponse> GetDeviceCodeAsync(CancellationToken cancellationToken = default)
    {
        var request = CreateDeviceCodeRequest();
        var (content, response) = await SendRequestAsync(request, cancellationToken);
        return HandleDeviceCodeResponse(response, content);
    }

    /// <summary>
    /// 获取指定工作空间的设备码。
    /// </summary>
    public async Task<DeviceAuthResponse> GetDeviceCodeAsync(string workspaceId, CancellationToken cancellationToken = default)
    {
        var request = CreateDeviceCodeRequest(workspaceId);
        var (content, response) = await SendRequestAsync(request, cancellationToken);
        return HandleDeviceCodeResponse(response, content);
    }

    /// <summary>
    /// 使用设备码获取访问 Token。
    /// </summary>
    public Task<OAuthToken> GetAccessTokenAsync(string deviceCode, CancellationToken cancellationToken = default)
    {
        return GetTokenFromDeviceCodeAsync(deviceCode, cancellationToken);
    }

    /// <summary>
    /// 轮询访问 Token 直到用户授权设备。
    /// </summary>
    public async Task<OAuthToken> PollAccessTokenAsync(string deviceCode, CancellationToken cancellationToken = default)
    {
        var interval = 5;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                return await GetTokenFromDeviceCodeAsync(deviceCode, cancellationToken);
            }
            catch (CozeAuthException ex) when (ex.Message.Contains("authorization_pending", StringComparison.OrdinalIgnoreCase))
            {
                _logger?.LogInformation("Authorization pending, sleeping {Interval} seconds", interval);
            }
            catch (CozeAuthException ex) when (ex.Message.Contains("slow_down", StringComparison.OrdinalIgnoreCase))
            {
                if (interval < 30) interval += 5;
                _logger?.LogInformation("Slow down, sleeping {Interval} seconds", interval);
            }
            catch (CozeAuthException)
            {
                throw;
            }

            await Task.Delay(TimeSpan.FromSeconds(interval), cancellationToken);
        }

        throw new OperationCanceledException("Device authorization polling was cancelled", cancellationToken);
    }

    // 注意：RefreshTokenAsync 继承自 OAuthClient 基类
}
