using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Coze.Sdk.Authentication;
using Coze.Sdk.Exceptions;
using Coze.Sdk.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Coze.Sdk.Http;

/// <summary>
/// HTTP 方法枚举。
/// </summary>
public enum HttpMethodType
{
    Get,
    Post,
    Put,
    Delete,
    Patch
}

/// <summary>
/// 表示一个 HTTP 请求。
/// </summary>
public class CozeHttpRequest
{
    /// <summary>
    /// 获取或设置端点路径。
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置 HTTP 方法。
    /// </summary>
    public HttpMethodType Method { get; set; } = HttpMethodType.Post;

    /// <summary>
    /// 获取或设置查询参数。
    /// </summary>
    public Dictionary<string, string?> QueryParameters { get; } = new();

    /// <summary>
    /// 获取或设置请求头。
    /// </summary>
    public Dictionary<string, string> Headers { get; } = new();

    /// <summary>
    /// 获取或设置请求体对象（将被序列化为 JSON）。
    /// </summary>
    public object? Body { get; set; }

    /// <summary>
    /// 获取或设置原始请求体内容（用于 multipart/form-data）。
    /// </summary>
    public HttpContent? RawContent { get; set; }

    /// <summary>
    /// 添加查询参数。
    /// </summary>
    public CozeHttpRequest AddQueryParameter(string name, object? value)
    {
        QueryParameters[name] = value?.ToString();
        return this;
    }

    /// <summary>
    /// 添加请求头。
    /// </summary>
    public CozeHttpRequest AddHeader(string name, string value)
    {
        Headers[name] = value;
        return this;
    }

    /// <summary>
    /// 设置 JSON 请求体。
    /// </summary>
    public CozeHttpRequest SetJsonBody(object body)
    {
        Body = body;
        return this;
    }
}

/// <summary>
/// Coze API 的 HTTP 客户端封装。
/// 使用 HttpClient 处理所有请求，包括 SSE 流式传输。
/// 对应 Java SDK 中的 OkHttpClient + Retrofit。
/// </summary>
internal class CozeHttpClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly Auth _auth;
    private readonly ILogger? _logger;
    private readonly string _baseUrl;
    private bool _disposed;

    private const string UserAgent = "CozeSdk.Net/1.0.0";
    private const string LogIdHeader = "X-Tt-Logid";
    private const string ContentTypeJson = "application/json";

    /// <summary>
    /// 初始化 <see cref="CozeHttpClient"/> 类的新实例。
    /// </summary>
    /// <param name="options">SDK 选项。</param>
    public CozeHttpClient(CozeOptions options)
    {
        _auth = options.Auth ?? throw new InvalidOperationException("Auth is required");
        _logger = options.LoggerFactory?.CreateLogger<CozeHttpClient>();
        _baseUrl = options.BaseUrl.TrimEnd('/');

        _httpClient = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        })
        {
            Timeout = options.ReadTimeout
        };

        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
    }

    /// <summary>
    /// 执行请求并返回反序列化后的响应。
    /// </summary>
    /// <typeparam name="T">响应类型。</typeparam>
    /// <param name="request">HTTP 请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>反序列化后的响应。</returns>
    public async Task<T> ExecuteAsync<T>(CozeHttpRequest request, CancellationToken cancellationToken = default)
    {
        var httpRequest = BuildHttpRequestMessage(request);
        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        var logId = ExtractLogId(response);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleError(response.StatusCode, content, logId, response.ReasonPhrase);
        }

        if (string.IsNullOrEmpty(content))
        {
            throw new CozeApiException(
                (int)response.StatusCode,
                0,
                "Empty response from API",
                logId);
        }

        var apiResponse = JsonHelper.DeserializeObject<Models.Common.ApiResponse<T>>(content);

        if (apiResponse == null)
        {
            throw new CozeApiException(
                (int)response.StatusCode,
                0,
                "Failed to deserialize API response",
                logId,
                content);
        }

        if (!apiResponse.IsSuccess)
        {
            throw new CozeApiException(
                (int)response.StatusCode,
                apiResponse.Code,
                apiResponse.Message ?? "Unknown error",
                logId,
                content);
        }

        if (apiResponse.Data == null)
        {
            throw new CozeApiException(
                (int)response.StatusCode,
                0,
                "API response data is null",
                logId,
                content);
        }

        return apiResponse.Data;
    }

    /// <summary>
    /// 执行请求并返回原始响应（用于非标准 API 响应）。
    /// </summary>
    /// <param name="request">HTTP 请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>原始响应内容。</returns>
    public async Task<string> ExecuteRawAsync(CozeHttpRequest request, CancellationToken cancellationToken = default)
    {
        var httpRequest = BuildHttpRequestMessage(request);
        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

        var logId = ExtractLogId(response);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleError(response.StatusCode, content, logId, response.ReasonPhrase);
        }

        return content ?? string.Empty;
    }

    /// <summary>
    /// 执行请求并返回用于 SSE 处理的流。
    /// 使用 HttpCompletionOption.ResponseHeadersRead 实现真正的流式支持。
    /// </summary>
    /// <param name="request">HTTP 请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>响应流。</returns>
    public async Task<Stream> ExecuteStreamAsync(CozeHttpRequest request, CancellationToken cancellationToken = default)
    {
        var (stream, _) = await ExecuteStreamWithLogIdAsync(request, cancellationToken);
        return stream;
    }

    /// <summary>
    /// 执行流式请求并直接返回响应流。
    /// 使用 HttpCompletionOption.ResponseHeadersRead 实现真正的流式支持。
    /// </summary>
    /// <param name="request">HTTP 请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>响应流和日志 ID。</returns>
    public async Task<(Stream Stream, string? LogId)> ExecuteStreamWithLogIdAsync(
        CozeHttpRequest request,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = BuildHttpRequestMessage(request);

        // 使用 ResponseHeadersRead 在接收到头信息后立即获取流
        // 这是实现真正流式传输的关键 - 不需要等待整个响应体
        var response = await _httpClient.SendAsync(
            httpRequest,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var logId = ExtractLogId(response);
            HandleError(response.StatusCode, content, logId, response.ReasonPhrase);
        }

        var responseLogId = ExtractLogId(response);
        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        return (stream, responseLogId);
    }

    /// <summary>
    /// 使用 multipart/form-data 执行文件上传请求。
    /// </summary>
    /// <typeparam name="T">响应类型。</typeparam>
    /// <param name="request">HTTP 请求。</param>
    /// <param name="fileContent">文件内容。</param>
    /// <param name="fileName">文件名。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>反序列化后的响应。</returns>
    public async Task<T> ExecuteMultipartAsync<T>(
        CozeHttpRequest request,
        Stream fileContent,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = BuildHttpRequestMessage(request, includeContentType: false);

        var multipartContent = new MultipartFormDataContent();

        // 添加文件内容
        var fileStreamContent = new StreamContent(fileContent);
        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        multipartContent.Add(fileStreamContent, "file", fileName);

        // 从请求体添加其他表单参数
        if (request.Body != null)
        {
            var bodyProps = request.Body.GetType().GetProperties();
            foreach (var prop in bodyProps)
            {
                var value = prop.GetValue(request.Body);
                if (value != null)
                {
                    var jsonAttr = prop.GetCustomAttributes(typeof(JsonPropertyAttribute), false)
                        .FirstOrDefault() as JsonPropertyAttribute;
                    var propName = jsonAttr?.PropertyName ?? prop.Name;
                    multipartContent.Add(new StringContent(value?.ToString() ?? ""), propName);
                }
            }
        }

        httpRequest.Content = multipartContent;

        var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
        var logId = ExtractLogId(response);
        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            HandleError(response.StatusCode, responseContent, logId, response.ReasonPhrase);
        }

        var apiResponse = JsonHelper.DeserializeObject<Models.Common.ApiResponse<T>>(responseContent);

        if (apiResponse == null || !apiResponse.IsSuccess || apiResponse.Data == null)
        {
            throw new CozeApiException(
                (int)response.StatusCode,
                apiResponse?.Code ?? 0,
                apiResponse?.Message ?? "Failed to upload file",
                logId,
                responseContent);
        }

        return apiResponse.Data;
    }

    /// <summary>
    /// 从 CozeHttpRequest 构建 HttpRequestMessage。
    /// </summary>
    private HttpRequestMessage BuildHttpRequestMessage(CozeHttpRequest request, bool includeContentType = true)
    {
        var method = request.Method switch
        {
            HttpMethodType.Get => HttpMethod.Get,
            HttpMethodType.Post => HttpMethod.Post,
            HttpMethodType.Put => HttpMethod.Put,
            HttpMethodType.Delete => HttpMethod.Delete,
            HttpMethodType.Patch => HttpMethod.Patch,
            _ => HttpMethod.Post
        };

        // 构建带查询参数的完整 URL
        var urlBuilder = new StringBuilder(_baseUrl);
        urlBuilder.Append(request.Endpoint);

        if (request.QueryParameters.Count > 0)
        {
            urlBuilder.Append('?');
            var first = true;
            foreach (var param in request.QueryParameters)
            {
                if (!first) urlBuilder.Append('&');
                urlBuilder.Append(Uri.EscapeDataString(param.Key));
                urlBuilder.Append('=');
                if (param.Value != null)
                {
                    urlBuilder.Append(Uri.EscapeDataString(param.Value));
                }
                first = false;
            }
        }

        var httpRequest = new HttpRequestMessage(method, urlBuilder.ToString());

        // 添加授权头
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue(
            _auth.TokenType,
            _auth.GetToken());

        // 添加自定义请求头
        foreach (var header in request.Headers)
        {
            httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        // 添加内容
        if (request.RawContent != null)
        {
            httpRequest.Content = request.RawContent;
        }
        else if (request.Body != null)
        {
            var json = JsonHelper.SerializeObjectCamelCase(request.Body);
            httpRequest.Content = new StringContent(json, Encoding.UTF8, ContentTypeJson);
        }
        else if (includeContentType && method != HttpMethod.Get)
        {
            // 为空请求体添加空内容类型头
            httpRequest.Content = new StringContent("{}", Encoding.UTF8, ContentTypeJson);
        }

        return httpRequest;
    }

    /// <summary>
    /// 处理 HTTP 错误。
    /// </summary>
    private void HandleError(HttpStatusCode statusCode, string content, string? logId, string? reasonPhrase)
    {
        // 尝试解析错误响应
        if (!string.IsNullOrEmpty(content))
        {
            try
            {
                var errorResponse = JsonHelper.DeserializeObject<Models.Common.ApiResponse<object>>(content);
                if (errorResponse != null)
                {
                    throw new CozeApiException(
                        (int)statusCode,
                        errorResponse.Code,
                        errorResponse.Message ?? reasonPhrase ?? "Unknown error",
                        logId ?? errorResponse.LogId,
                        content);
                }
            }
            catch (JsonException)
            {
                // 忽略 JSON 解析错误
            }
        }

        throw new CozeApiException(
            (int)statusCode,
            0,
            reasonPhrase ?? "Unknown HTTP error",
            logId,
            content);
    }

    /// <summary>
    /// 从 HttpResponseMessage 中提取日志 ID。
    /// </summary>
    private static string? ExtractLogId(HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues(LogIdHeader, out var values))
        {
            return values.FirstOrDefault();
        }
        return null;
    }

    /// <summary>
    /// 创建新的 HTTP 请求。
    /// </summary>
    /// <param name="endpoint">API 端点。</param>
    /// <param name="method">HTTP 方法。</param>
    /// <returns>新的 CozeHttpRequest 实例。</returns>
    public CozeHttpRequest CreateRequest(string endpoint, HttpMethodType method = HttpMethodType.Post)
    {
        return new CozeHttpRequest
        {
            Endpoint = endpoint,
            Method = method
        };
    }

    /// <summary>
    /// 创建 GET 请求。
    /// </summary>
    /// <param name="endpoint">API 端点。</param>
    /// <returns>新的 CozeHttpRequest 实例。</returns>
    public CozeHttpRequest CreateGetRequest(string endpoint)
    {
        return CreateRequest(endpoint, HttpMethodType.Get);
    }

    /// <summary>
    /// 创建 POST 请求。
    /// </summary>
    /// <param name="endpoint">API 端点。</param>
    /// <returns>新的 CozeHttpRequest 实例。</returns>
    public CozeHttpRequest CreatePostRequest(string endpoint)
    {
        return CreateRequest(endpoint, HttpMethodType.Post);
    }

    /// <summary>
    /// 创建 PUT 请求。
    /// </summary>
    /// <param name="endpoint">API 端点。</param>
    /// <returns>新的 CozeHttpRequest 实例。</returns>
    public CozeHttpRequest CreatePutRequest(string endpoint)
    {
        return CreateRequest(endpoint, HttpMethodType.Put);
    }

    /// <summary>
    /// 创建 DELETE 请求。
    /// </summary>
    /// <param name="endpoint">API 端点。</param>
    /// <returns>新的 CozeHttpRequest 实例。</returns>
    public CozeHttpRequest CreateDeleteRequest(string endpoint)
    {
        return CreateRequest(endpoint, HttpMethodType.Delete);
    }

    /// <summary>
    /// 向请求添加 JSON 请求体。
    /// </summary>
    /// <param name="request">请求对象。</param>
    /// <param name="body">请求体对象。</param>
    public void AddJsonBody(CozeHttpRequest request, object body)
    {
        request.Body = body;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient.Dispose();
            _disposed = true;
        }
    }
}
