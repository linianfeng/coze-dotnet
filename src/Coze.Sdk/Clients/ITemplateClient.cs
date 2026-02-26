using Coze.Sdk.Models.Templates;

namespace Coze.Sdk.Clients;

/// <summary>
/// 模板操作接口。
/// 对应 Java SDK 中的 TemplateAPI.java。
/// </summary>
public interface ITemplateClient
{
    /// <summary>
    /// 复制模板。
    /// </summary>
    /// <param name="templateId">要复制的模板 ID。</param>
    /// <param name="request">请求参数。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>复制响应。</returns>
    Task<DuplicateTemplateResponse> DuplicateAsync(
        string templateId,
        DuplicateTemplateRequest request,
        CancellationToken cancellationToken = default);
}
