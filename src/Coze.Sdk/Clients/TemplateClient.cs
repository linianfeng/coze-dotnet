using Coze.Sdk.Http;
using Coze.Sdk.Models.Templates;

namespace Coze.Sdk.Clients;

/// <summary>
/// 模板操作的实现。
/// 对应 Java SDK 中的 TemplateAPI.java。
/// </summary>
internal class TemplateClient : ITemplateClient
{
    private readonly CozeHttpClient _httpClient;

    public TemplateClient(CozeHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DuplicateTemplateResponse> DuplicateAsync(
        string templateId,
        DuplicateTemplateRequest request,
        CancellationToken cancellationToken = default)
    {
        var endpoint = $"/v1/templates/{templateId}/duplicate";
        var httpRequest = _httpClient.CreateRequest(endpoint, HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, request);
        return await _httpClient.ExecuteAsync<DuplicateTemplateResponse>(httpRequest, cancellationToken);
    }
}
