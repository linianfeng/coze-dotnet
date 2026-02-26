using System.Runtime.Serialization;
using Coze.Sdk.Http;
using Coze.Sdk.Models.Commerce;

namespace Coze.Sdk.Clients;

/// <summary>
/// 商业操作的实现。
/// </summary>
internal class CommerceClient : ICommerceClient
{
    private readonly BenefitBillClient _billClient;
    private readonly BenefitLimitationClient _limitationClient;

    public CommerceClient(CozeHttpClient httpClient)
    {
        _billClient = new BenefitBillClient(httpClient);
        _limitationClient = new BenefitLimitationClient(httpClient);
    }

    public IBenefitBillClient Bills => _billClient;

    public IBenefitLimitationClient Limitations => _limitationClient;
}

/// <summary>
/// 商业权益账单操作的实现。
/// 对应 Java SDK 中的 CommerceBenefitBillAPI.java。
/// </summary>
internal class BenefitBillClient : IBenefitBillClient
{
    private readonly CozeHttpClient _httpClient;

    public BenefitBillClient(CozeHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BillTaskInfo> CreateTaskAsync(
        CreateBillDownloadTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest("/v1/commerce/benefit/bill_tasks", HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, request);
        return await _httpClient.ExecuteAsync<BillTaskInfo>(httpRequest, cancellationToken);
    }

    public async Task<ListBillDownloadTasksResponse> ListTasksAsync(
        ListBillDownloadTasksRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        request ??= new ListBillDownloadTasksRequest();

        var httpRequest = _httpClient.CreateGetRequest("/v1/commerce/benefit/bill_tasks");

        if (request.TaskIds != null && request.TaskIds.Count > 0)
        {
            foreach (var taskId in request.TaskIds)
            {
                httpRequest.AddQueryParameter("task_ids", taskId);
            }
        }

        if (request.PageNumber.HasValue)
        {
            httpRequest.AddQueryParameter("page_num", request.PageNumber.Value.ToString());
        }

        if (request.PageSize.HasValue)
        {
            httpRequest.AddQueryParameter("page_size", request.PageSize.Value.ToString());
        }

        return await _httpClient.ExecuteAsync<ListBillDownloadTasksResponse>(httpRequest, cancellationToken);
    }
}

/// <summary>
/// 商业权益限制操作的实现。
/// 对应 Java SDK 中的 CommerceBenefitLimitationAPI.java。
/// </summary>
internal class BenefitLimitationClient : IBenefitLimitationClient
{
    private readonly CozeHttpClient _httpClient;

    public BenefitLimitationClient(CozeHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BenefitInfo> CreateAsync(
        CreateBenefitLimitationRequest request,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest("/v1/commerce/benefit/limitations", HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, request);
        return await _httpClient.ExecuteAsync<BenefitInfo>(httpRequest, cancellationToken);
    }

    public async Task<BenefitInfo> UpdateAsync(
        UpdateBenefitLimitationRequest request,
        CancellationToken cancellationToken = default)
    {
        var endpoint = $"/v1/commerce/benefit/limitations/{request.BenefitId}";
        var httpRequest = _httpClient.CreateRequest(endpoint, HttpMethodType.Put);
        _httpClient.AddJsonBody(httpRequest, new
        {
            started_at = request.StartedAt,
            ended_at = request.EndedAt,
            limit = request.Limit,
            status = request.Status
        });
        return await _httpClient.ExecuteAsync<BenefitInfo>(httpRequest, cancellationToken);
    }

    public async Task<ListBenefitLimitationsResponse> ListAsync(
        ListBenefitLimitationsRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        request ??= new ListBenefitLimitationsRequest();

        var httpRequest = _httpClient.CreateGetRequest("/v1/commerce/benefit/limitations");

        if (request.EntityType.HasValue)
        {
            httpRequest.AddQueryParameter("entity_type", request.EntityType.Value.ToEnumString());
        }

        if (!string.IsNullOrEmpty(request.EntityId))
        {
            httpRequest.AddQueryParameter("entity_id", request.EntityId);
        }

        if (request.BenefitType.HasValue)
        {
            httpRequest.AddQueryParameter("benefit_type", request.BenefitType.Value.ToEnumString());
        }

        if (request.Status.HasValue)
        {
            httpRequest.AddQueryParameter("status", request.Status.Value.ToEnumString());
        }

        if (!string.IsNullOrEmpty(request.PageToken))
        {
            httpRequest.AddQueryParameter("page_token", request.PageToken);
        }

        if (request.PageSize.HasValue)
        {
            httpRequest.AddQueryParameter("page_size", request.PageSize.Value.ToString());
        }

        return await _httpClient.ExecuteAsync<ListBenefitLimitationsResponse>(httpRequest, cancellationToken);
    }
}

/// <summary>
/// 枚举转换扩展方法。
/// </summary>
internal static class CommerceEnumExtensions
{
    /// <summary>
    /// 将枚举转换为其字符串值。
    /// </summary>
    public static string ToEnumString<T>(this T value) where T : Enum
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = (EnumMemberAttribute?)field?
            .GetCustomAttributes(typeof(EnumMemberAttribute), false)
            .FirstOrDefault();
        return attribute?.Value ?? value.ToString().ToLowerInvariant();
    }
}
