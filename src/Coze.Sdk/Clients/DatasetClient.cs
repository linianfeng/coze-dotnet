using Coze.Sdk.Http;
using Coze.Sdk.Models.Common;
using Coze.Sdk.Models.Datasets;
using Coze.Sdk.Utils;

namespace Coze.Sdk.Clients;

/// <summary>
/// 数据集操作的实现。
/// 对应 Java SDK 中的 DatasetService.java。
/// </summary>
internal class DatasetClient : IDatasetClient
{
    private readonly CozeHttpClient _httpClient;
    private readonly DocumentClient _documentClient;
    private readonly DatasetImageClient _imageClient;

    public DatasetClient(CozeHttpClient httpClient)
    {
        _httpClient = httpClient;
        _documentClient = new DocumentClient(httpClient);
        _imageClient = new DatasetImageClient(httpClient);
    }

    public IDocumentClient Documents => _documentClient;

    public IDatasetImageClient Images => _imageClient;

    public async Task<Dataset> CreateAsync(CreateDatasetRequest request, CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest("/v1/datasets", HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, request);

        return await _httpClient.ExecuteAsync<Dataset>(httpRequest, cancellationToken);
    }

    public async Task<ListDatasetsResponse> ListAsync(ListDatasetsRequest request, CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateGetRequest("/v1/datasets");
        httpRequest.AddQueryParameter("space_id", request.SpaceId);

        if (!string.IsNullOrEmpty(request.Name))
        {
            httpRequest.AddQueryParameter("name", request.Name);
        }

        if (request.PageNumber.HasValue)
        {
            httpRequest.AddQueryParameter("page_num", request.PageNumber.Value.ToString());
        }

        if (request.PageSize.HasValue)
        {
            httpRequest.AddQueryParameter("page_size", request.PageSize.Value.ToString());
        }

        // API 返回的 data 是 { total_count: X, dataset_list: [...] }
        var rawResponse = await _httpClient.ExecuteRawAsync(httpRequest, cancellationToken);
        var dataResponse = JsonHelper.DeserializeObject<ApiResponse<ListDatasetsDataResponse>>(rawResponse);

        return new ListDatasetsResponse
        {
            Datasets = dataResponse?.Data?.DatasetList,
            Total = dataResponse?.Data?.TotalCount
        };
    }

    public async Task<UpdateDatasetResponse> UpdateAsync(UpdateDatasetRequest request, CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest($"/v1/datasets/{request.DatasetId}", HttpMethodType.Put);
        _httpClient.AddJsonBody(httpRequest, new { name = request.Name, description = request.Description });

        // 更新 API 响应没有 data 字段，使用 ExecuteRawAsync 手动处理
        var rawResponse = await _httpClient.ExecuteRawAsync(httpRequest, cancellationToken);
        var apiResponse = JsonHelper.DeserializeObject<ApiResponse<UpdateDatasetResponse>>(rawResponse);

        if (apiResponse == null || !apiResponse.IsSuccess)
        {
            throw new Exceptions.CozeApiException(
                200,
                apiResponse?.Code ?? 0,
                apiResponse?.Message ?? "Failed to update dataset");
        }

        return new UpdateDatasetResponse();
    }

    public async Task DeleteAsync(string datasetId, CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest($"/v1/datasets/{datasetId}", HttpMethodType.Delete);

        // 删除 API 响应没有 data 字段，使用 ExecuteRawAsync 手动处理
        var rawResponse = await _httpClient.ExecuteRawAsync(httpRequest, cancellationToken);
        var apiResponse = JsonHelper.DeserializeObject<ApiResponse<object>>(rawResponse);

        if (apiResponse == null || !apiResponse.IsSuccess)
        {
            throw new Exceptions.CozeApiException(
                200,
                apiResponse?.Code ?? 0,
                apiResponse?.Message ?? "Failed to delete dataset");
        }
    }
}

/// <summary>
/// 列出文档的请求。
/// </summary>
public record ListDocumentsRequest
{
    /// <summary>
    /// 获取数据集 ID。
    /// </summary>
    public required string DatasetId { get; init; }

    /// <summary>
    /// 获取页码。
    /// </summary>
    public int? PageNumber { get; init; } = 1;

    /// <summary>
    /// 获取每页数量。
    /// </summary>
    public int? PageSize { get; init; } = 20;
}

/// <summary>
/// 列出文档的响应。
/// </summary>
public record ListDocumentsResponse
{
    /// <summary>
    /// 获取文档列表。
    /// </summary>
    public IReadOnlyList<Document>? Documents { get; init; }

    /// <summary>
    /// 获取总数量。
    /// </summary>
    public int? Total { get; init; }
}

/// <summary>
/// 文档操作的实现。
/// </summary>
internal class DocumentClient : IDocumentClient
{
    private readonly CozeHttpClient _httpClient;

    public DocumentClient(CozeHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Document> CreateAsync(CreateDocumentRequest request, CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest("/v1/datasets/documents", HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, request);

        return await _httpClient.ExecuteAsync<Document>(httpRequest, cancellationToken);
    }

    public async Task<ListDocumentsResponse> ListAsync(ListDocumentsRequest request, CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateGetRequest($"/v1/datasets/{request.DatasetId}/documents");

        if (request.PageNumber.HasValue)
        {
            httpRequest.AddQueryParameter("page_num", request.PageNumber.Value.ToString());
        }

        if (request.PageSize.HasValue)
        {
            httpRequest.AddQueryParameter("page_size", request.PageSize.Value.ToString());
        }

        var documents = await _httpClient.ExecuteAsync<List<Document>>(httpRequest, cancellationToken);

        return new ListDocumentsResponse
        {
            Documents = documents,
            Total = documents?.Count ?? 0
        };
    }

    public async Task<Document> UpdateAsync(UpdateDocumentRequest request, CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest($"/v1/datasets/{request.DatasetId}/documents/{request.DocumentId}", HttpMethodType.Put);
        _httpClient.AddJsonBody(httpRequest, new { name = request.Name });

        // 更新文档 API 响应没有 data 字段，使用 ExecuteRawAsync 手动处理
        var rawResponse = await _httpClient.ExecuteRawAsync(httpRequest, cancellationToken);
        var apiResponse = JsonHelper.DeserializeObject<ApiResponse<Document>>(rawResponse);

        if (apiResponse == null || !apiResponse.IsSuccess)
        {
            throw new Exceptions.CozeApiException(
                200,
                apiResponse?.Code ?? 0,
                apiResponse?.Message ?? "Failed to update document");
        }

        // API 不返回更新后的文档，返回一个空文档对象
        return new Document { DocumentId = request.DocumentId, Name = request.Name };
    }

    public async Task DeleteAsync(string datasetId, string documentId, CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest($"/v1/datasets/{datasetId}/documents/{documentId}", HttpMethodType.Delete);

        // 删除文档 API 响应没有 data 字段，使用 ExecuteRawAsync 手动处理
        var rawResponse = await _httpClient.ExecuteRawAsync(httpRequest, cancellationToken);
        var apiResponse = JsonHelper.DeserializeObject<ApiResponse<object>>(rawResponse);

        if (apiResponse == null || !apiResponse.IsSuccess)
        {
            throw new Exceptions.CozeApiException(
                200,
                apiResponse?.Code ?? 0,
                apiResponse?.Message ?? "Failed to delete document");
        }
    }
}

/// <summary>
/// 数据集图片操作的实现。
/// 对应 Java SDK 中的 DatasetImageAPI.java。
/// </summary>
internal class DatasetImageClient : IDatasetImageClient
{
    private readonly CozeHttpClient _httpClient;

    public DatasetImageClient(CozeHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ListDatasetImagesResponse> ListAsync(ListDatasetImagesRequest request, CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateGetRequest($"v1/datasets/{request.DatasetId}/images");

        if (!string.IsNullOrEmpty(request.Keyword))
        {
            httpRequest.AddQueryParameter("keyword", request.Keyword);
        }

        if (request.HasCaption.HasValue)
        {
            httpRequest.AddQueryParameter("has_caption", request.HasCaption.Value.ToString().ToLowerInvariant());
        }

        if (request.PageNumber.HasValue)
        {
            httpRequest.AddQueryParameter("page_num", request.PageNumber.Value.ToString());
        }

        if (request.PageSize.HasValue)
        {
            httpRequest.AddQueryParameter("page_size", request.PageSize.Value.ToString());
        }

        return await _httpClient.ExecuteAsync<ListDatasetImagesResponse>(httpRequest, cancellationToken);
    }

    public async Task<UpdateDatasetImageResponse> UpdateAsync(
        string datasetId,
        string documentId,
        UpdateDatasetImageRequest request,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest($"v1/datasets/{datasetId}/images/{documentId}", HttpMethodType.Put);
        _httpClient.AddJsonBody(httpRequest, request);

        return await _httpClient.ExecuteAsync<UpdateDatasetImageResponse>(httpRequest, cancellationToken);
    }
}
