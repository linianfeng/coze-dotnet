using Coze.Sdk.Models.Variables;

namespace Coze.Sdk.Clients;

/// <summary>
/// 变量操作接口。
/// 对应 Java SDK 中的 VariablesAPI.java。
/// </summary>
public interface IVariablesClient
{
    /// <summary>
    /// 更新变量。
    /// </summary>
    /// <param name="request">更新请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>更新响应。</returns>
    Task<UpdateVariablesResponse> UpdateAsync(
        UpdateVariablesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取变量。
    /// </summary>
    /// <param name="request">获取请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>获取响应。</returns>
    Task<RetrieveVariablesResponse> RetrieveAsync(
        RetrieveVariablesRequest? request = null,
        CancellationToken cancellationToken = default);
}
