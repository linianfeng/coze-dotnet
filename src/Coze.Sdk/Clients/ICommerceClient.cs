using Coze.Sdk.Models.Commerce;

namespace Coze.Sdk.Clients;

/// <summary>
/// 商业权益账单操作接口。
/// 对应 Java SDK 中的 CommerceBenefitBillAPI.java。
/// </summary>
public interface IBenefitBillClient
{
    /// <summary>
    /// 创建账单下载任务。
    /// </summary>
    /// <param name="request">创建请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>任务信息。</returns>
    Task<BillTaskInfo> CreateTaskAsync(
        CreateBillDownloadTaskRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 列出账单下载任务。
    /// </summary>
    /// <param name="request">列表请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>列表响应。</returns>
    Task<ListBillDownloadTasksResponse> ListTasksAsync(
        ListBillDownloadTasksRequest? request = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 商业权益限制操作接口。
/// 对应 Java SDK 中的 CommerceBenefitLimitationAPI.java。
/// </summary>
public interface IBenefitLimitationClient
{
    /// <summary>
    /// 创建权益限制。
    /// </summary>
    /// <param name="request">创建请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>创建的权益信息。</returns>
    Task<BenefitInfo> CreateAsync(
        CreateBenefitLimitationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新权益限制。
    /// </summary>
    /// <param name="request">更新请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>更新后的权益信息。</returns>
    Task<BenefitInfo> UpdateAsync(
        UpdateBenefitLimitationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 列出权益限制。
    /// </summary>
    /// <param name="request">列表请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>列表响应。</returns>
    Task<ListBenefitLimitationsResponse> ListAsync(
        ListBenefitLimitationsRequest? request = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 商业操作接口。
/// </summary>
public interface ICommerceClient
{
    /// <summary>
    /// 获取权益账单客户端。
    /// </summary>
    IBenefitBillClient Bills { get; }

    /// <summary>
    /// 获取权益限制客户端。
    /// </summary>
    IBenefitLimitationClient Limitations { get; }
}
