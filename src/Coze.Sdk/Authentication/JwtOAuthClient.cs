using System.Text;
using Coze.Sdk.Exceptions;
using Coze.Sdk.Utils;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Asn1.Pkcs;

namespace Coze.Sdk.Authentication;

/// <summary>
/// 用于服务账户认证的 JWT OAuth 客户端。
/// 自动管理 Token 生命周期并实现 Auth 接口，可直接与 CozeClient 一起使用。
/// </summary>
public class JwtOAuthClient : OAuthClient
{
    private readonly string _privateKey;
    private readonly string? _publicKeyId;
    private readonly string _hostName;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private OAuthToken? _cachedToken;

    /// <summary>
    /// 初始化 <see cref="JwtOAuthClient"/> 类的新实例。
    /// </summary>
    public JwtOAuthClient(OAuthOptions options) : base(options)
    {
        _privateKey = options.ClientSecret
            ?? throw new ArgumentException("ClientSecret (private key) is required for JWT OAuth", nameof(options));
        _publicKeyId = options.PublicKeyId;

        // 从 BaseUrl 提取 host name
        if (!string.IsNullOrEmpty(BaseUrl))
        {
            try
            {
                var uri = new Uri(BaseUrl);
                _hostName = uri.Host;
            }
            catch
            {
                _hostName = BaseUrl;
            }
        }
        else
        {
            _hostName = "api.coze.cn";
        }
    }

    /// <summary>
    /// 获取当前访问 Token，必要时刷新。线程安全。
    /// </summary>
    public override string GetToken()
    {
        return GetAccessTokenAsync().GetAwaiter().GetResult().AccessToken!;
    }

    /// <summary>
    /// 获取访问 Token，必要时刷新。
    /// </summary>
    public async Task<OAuthToken> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        // 快速路径：如果缓存的 Token 有效则直接返回
        if (_cachedToken != null && !_cachedToken.IsExpired)
        {
            return _cachedToken;
        }

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            // 获取锁后再次检查
            if (_cachedToken != null && !_cachedToken.IsExpired)
            {
                return _cachedToken;
            }

            return await RefreshTokenInternalAsync(cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// 强制刷新 Token。
    /// </summary>
    public async Task<OAuthToken> RefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            return await RefreshTokenInternalAsync(cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<OAuthToken> RefreshTokenInternalAsync(CancellationToken cancellationToken)
    {
        var jwt = GenerateJwt();

        // 请求体 - 与 Java SDK 保持一致
        // JWT token 放在 Authorization header 中，而不是请求体
        var body = new Dictionary<string, object?>
        {
            ["grant_type"] = "urn:ietf:params:oauth:grant-type:jwt-bearer",
            ["duration_seconds"] = 600
        };

        var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/api/permission/oauth2/token");

        // JWT token 放入 Authorization header
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

        var json = JsonHelper.SerializeObject(body);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await HttpClient.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new CozeAuthException(
                AuthErrorCode.ServerError,
                $"Failed to get access token: {response.StatusCode}, Response: {content}",
                statusCode: (int)response.StatusCode);
        }

        var token = JsonHelper.DeserializeObjectCamelCase<OAuthToken>(content);
        if (token == null)
        {
            throw new CozeAuthException(AuthErrorCode.ServerError, "Failed to parse token response");
        }

        _cachedToken = token;
        return token;
    }

    private string GenerateJwt()
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var ttl = 600;

        // JWT Header - 与 Java SDK 保持一致
        var header = new Dictionary<string, object>
        {
            ["alg"] = "RS256",
            ["typ"] = "JWT"
        };

        // kid 是必需的
        if (!string.IsNullOrEmpty(_publicKeyId))
        {
            header["kid"] = _publicKeyId;
        }

        // JWT Payload - 与 Java SDK 保持一致
        var payload = new Dictionary<string, object?>
        {
            ["iss"] = ClientId,
            ["aud"] = _hostName,  // 使用 host name，不是完整 URL
            ["exp"] = now + ttl,
            ["iat"] = now,
            ["session_name"] = $"session_{now}",
            ["jti"] = GenerateRandomString(16)  // JWT ID
        };

        var headerB64 = Base64UrlEncode(Encoding.UTF8.GetBytes(JsonHelper.SerializeObject(header)));
        var payloadB64 = Base64UrlEncode(Encoding.UTF8.GetBytes(JsonHelper.SerializeObject(payload)));
        var signingInput = $"{headerB64}.{payloadB64}";
        var signature = SignWithRsa(_privateKey, Encoding.UTF8.GetBytes(signingInput));

        return $"{signingInput}.{Base64UrlEncode(signature)}";
    }

    private static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private static byte[] SignWithRsa(string privateKey, byte[] data)
    {
        try
        {
            var rsaKey = ParsePrivateKey(privateKey);
            var signer = SignerUtilities.GetSigner("SHA256withRSA");
            signer.Init(true, rsaKey);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.GenerateSignature();
        }
        catch (CozeAuthException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new CozeAuthException(AuthErrorCode.InvalidClient, "Failed to sign JWT with private key", ex);
        }
    }

    /// <summary>
    /// 解析 PKCS8 格式的 RSA 私钥。
    /// 支持标准 PEM 格式和 JSON 中转义的格式。
    /// </summary>
    private static RsaPrivateCrtKeyParameters ParsePrivateKey(string privateKeyPem)
    {
        try
        {
            // 处理 JSON 中转义的换行符
            var normalizedKey = privateKeyPem
                .Replace("\\n", "\n")
                .Replace("\\r", "\r")
                .Trim();

            // 移除 PEM 头尾标记和所有空白字符
            var keyContent = normalizedKey
                .Replace("-----BEGIN PRIVATE KEY-----", "")
                .Replace("-----END PRIVATE KEY-----", "")
                .Replace("-----BEGIN RSA PRIVATE KEY-----", "")
                .Replace("-----END RSA PRIVATE KEY-----", "")
                .Replace("\n", "")
                .Replace("\r", "")
                .Replace(" ", "")
                .Trim();

            // Base64 解码
            byte[] privateKeyBytes;
            try
            {
                privateKeyBytes = Convert.FromBase64String(keyContent);
            }
            catch (FormatException ex)
            {
                throw new CozeAuthException(AuthErrorCode.InvalidClient, "Invalid private key: Base64 decoding failed", ex);
            }

            // 解析 PKCS8 格式的私钥
            var privateKeyInfo = PrivateKeyInfo.GetInstance(privateKeyBytes);
            var rsaKey = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(privateKeyInfo);

            return rsaKey;
        }
        catch (CozeAuthException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new CozeAuthException(AuthErrorCode.InvalidClient, $"Invalid private key format: {ex.Message}", ex);
        }
    }

    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    /// <summary>
    /// 释放资源，包括信号量。
    /// </summary>
    public new void Dispose()
    {
        _semaphore.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}
