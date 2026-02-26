using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Web;
using Coze.Sdk.Exceptions;
using Coze.Sdk.Utils;
using Newtonsoft.Json;

namespace Coze.Sdk.Authentication;

/// <summary>
/// OAuth 客户端配置选项。
/// </summary>
public record OAuthOptions
{
    /// <summary>
    /// 获取或设置客户端 ID。
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置客户端密钥（PKCE 流程可选）。
    /// 对于 JWT OAuth，此字段为私钥。
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// 获取或设置基础 URL。
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.coze.cn";

    /// <summary>
    /// 获取或设置 WWW URL（用于授权页面）。
    /// </summary>
    public string? WwwUrl { get; set; }

    /// <summary>
    /// 获取或设置公钥 ID（JWT OAuth 需要）。
    /// </summary>
    public string? PublicKeyId { get; set; }

    /// <summary>
    /// 获取或设置 HTTP 请求超时时间。
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(50);
}

/// <summary>
/// OAuth 客户端的基类，同时作为 Auth 提供者。
/// 统一了认证接口与 OAuth 功能。
/// </summary>
public abstract class OAuthClient : Auth, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _clientId;
    private readonly string? _clientSecret;
    private readonly string _baseUrl;
    private readonly string _wwwUrl;

    /// <summary>
    /// 获取客户端 ID。
    /// </summary>
    protected string ClientId => _clientId;

    /// <summary>
    /// 获取客户端密钥。
    /// </summary>
    protected string? ClientSecret => _clientSecret;

    /// <summary>
    /// 获取基础 URL。
    /// </summary>
    public string BaseUrl => _baseUrl;

    /// <summary>
    /// 获取 WWW URL。
    /// </summary>
    protected string WwwUrl => _wwwUrl;

    /// <summary>
    /// 获取共享的 HttpClient 实例。
    /// </summary>
    protected HttpClient HttpClient => _httpClient;

    /// <summary>
    /// 初始化 <see cref="OAuthClient"/> 类的新实例。
    /// </summary>
    protected OAuthClient(OAuthOptions options)
    {
        _clientId = options.ClientId ?? throw new ArgumentNullException(nameof(options.ClientId));
        _clientSecret = options.ClientSecret;
        _baseUrl = options.BaseUrl ?? "https://api.coze.cn";
        _wwwUrl = options.WwwUrl ?? _baseUrl.Replace("api.", "www.");

        _httpClient = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        })
        {
            Timeout = options.Timeout
        };
    }

    #region Auth 实现

    /// <inheritdoc/>
    public override string TokenType => "Bearer";

    /// <summary>
    /// 获取访问 Token。
    /// 默认实现抛出异常 - 子类应重写此方法。
    /// </summary>
    public override string GetToken()
    {
        throw new NotImplementedException("Subclasses must implement GetToken()");
    }

    #endregion

    #region OAuth URL 生成

    /// <summary>
    /// 生成 OAuth 授权 URL。
    /// </summary>
    protected string BuildOAuthUrl(
        string? redirectUri,
        string? state,
        string? codeChallenge = null,
        string? codeChallengeMethod = null,
        string? workspaceId = null)
    {
        var queryParams = new Dictionary<string, string?>
        {
            ["response_type"] = "code",
            ["client_id"] = _clientId,
            ["redirect_uri"] = redirectUri,
            ["state"] = state,
            ["code_challenge"] = codeChallenge,
            ["code_challenge_method"] = codeChallengeMethod
        };

        var basePath = workspaceId != null
            ? $"/api/permission/oauth2/workspace_id/{workspaceId}/authorize"
            : "/api/permission/oauth2/authorize";

        var queryString = string.Join("&", queryParams
            .Where(kvp => kvp.Value != null)
            .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value!)}"));

        return $"{_wwwUrl}{basePath}?{queryString}";
    }

    #endregion

    #region Token 操作

    /// <summary>
    /// 使用授权码交换访问 Token。
    /// </summary>
    protected async Task<OAuthToken> ExchangeCodeForTokenAsync(
        string code,
        string? redirectUri,
        string? codeVerifier = null,
        CancellationToken cancellationToken = default)
    {
        var body = new Dictionary<string, object?>
        {
            ["client_id"] = _clientId,
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = redirectUri,
            ["code_verifier"] = codeVerifier
        };

        return await SendTokenRequestAsync(body, cancellationToken);
    }

    /// <summary>
    /// 使用刷新 Token 刷新访问 Token。
    /// </summary>
    public async Task<OAuthToken> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var body = new Dictionary<string, object?>
        {
            ["client_id"] = _clientId,
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = refreshToken
        };

        return await SendTokenRequestAsync(body, cancellationToken);
    }

    /// <summary>
    /// 使用设备码获取访问 Token。
    /// </summary>
    protected async Task<OAuthToken> GetTokenFromDeviceCodeAsync(string deviceCode, CancellationToken cancellationToken = default)
    {
        var body = new Dictionary<string, object?>
        {
            ["client_id"] = _clientId,
            ["grant_type"] = "urn:ietf:params:oauth:grant-type:device_code",
            ["device_code"] = deviceCode
        };

        return await SendTokenRequestAsync(body, cancellationToken);
    }

    private async Task<OAuthToken> SendTokenRequestAsync(Dictionary<string, object?> body, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/api/permission/oauth2/token");

        if (!string.IsNullOrEmpty(_clientSecret))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _clientSecret);
        }

        var json = JsonHelper.SerializeObject(body);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request, cancellationToken);
        return await HandleTokenResponseAsync(response, cancellationToken);
    }

    private async Task<OAuthToken> HandleTokenResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var logId = ExtractLogId(response);

        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var error = JsonHelper.DeserializeObjectCamelCase<OAuthErrorResponse>(content);
                throw new CozeAuthException(
                    MapErrorCode(error?.ErrorCode ?? error?.Error),
                    error?.ErrorDescription ?? error?.ErrorMessage ?? "OAuth error",
                    logId,
                    (int)response.StatusCode);
            }
            catch (JsonException)
            {
                throw new CozeAuthException(
                    AuthErrorCode.Unknown,
                    $"OAuth request failed: {response.StatusCode}",
                    logId,
                    (int)response.StatusCode);
            }
        }

        var token = JsonHelper.DeserializeObjectCamelCase<OAuthToken>(content ?? "{}");
        if (token == null)
        {
            throw new CozeAuthException(AuthErrorCode.Unknown, "Failed to parse token response");
        }

        return token with { LogId = logId };
    }

    #endregion

    #region 设备码

    /// <summary>
    /// 创建设备码请求。
    /// </summary>
    protected HttpRequestMessage CreateDeviceCodeRequest(string? workspaceId = null)
    {
        var endpoint = workspaceId != null
            ? $"/api/permission/oauth2/workspace_id/{workspaceId}/device/code"
            : "/api/permission/oauth2/device/code";

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}{endpoint}");
        var json = JsonHelper.SerializeObject(new { client_id = _clientId });
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        return request;
    }

    /// <summary>
    /// 处理设备码响应。
    /// </summary>
    protected static DeviceAuthResponse HandleDeviceCodeResponse(HttpResponseMessage response, string content)
    {
        if (!response.IsSuccessStatusCode)
        {
            throw new CozeAuthException(
                AuthErrorCode.ServerError,
                $"Device code request failed: {response.StatusCode}",
                statusCode: (int)response.StatusCode);
        }

        var deviceAuth = JsonHelper.DeserializeObjectCamelCase<DeviceAuthResponse>(content ?? "{}");
        return deviceAuth?.WithVerificationUrl() ?? new DeviceAuthResponse();
    }

    #endregion

    #region 辅助方法

    /// <summary>
    /// 发送 HTTP 请求并返回响应内容。
    /// </summary>
    protected async Task<(string Content, HttpResponseMessage Response)> SendRequestAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return (content, response);
    }

    private static string? ExtractLogId(HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues("x-tt-logid", out var values))
        {
            return values.FirstOrDefault();
        }
        return null;
    }

    private static AuthErrorCode MapErrorCode(string? errorCode)
    {
        return errorCode?.ToLowerInvariant() switch
        {
            "invalid_client" => AuthErrorCode.InvalidClient,
            "invalid_grant" => AuthErrorCode.InvalidGrant,
            "unauthorized_client" => AuthErrorCode.UnauthorizedClient,
            "access_denied" => AuthErrorCode.AccessDenied,
            "unsupported_response_type" => AuthErrorCode.UnsupportedResponseType,
            "invalid_scope" => AuthErrorCode.InvalidScope,
            "server_error" => AuthErrorCode.ServerError,
            "temporarily_unavailable" => AuthErrorCode.TemporarilyUnavailable,
            _ => AuthErrorCode.Unknown
        };
    }

    #endregion

    /// <inheritdoc/>
    public void Dispose()
    {
        _httpClient?.Dispose();
        GC.SuppressFinalize(this);
    }

    private record OAuthErrorResponse
    {
        [JsonProperty("error")]
        public string? Error { get; init; }

        [JsonProperty("error_code")]
        public string? ErrorCode { get; init; }

        [JsonProperty("error_message")]
        public string? ErrorMessage { get; init; }

        [JsonProperty("error_description")]
        public string? ErrorDescription { get; init; }
    }
}
