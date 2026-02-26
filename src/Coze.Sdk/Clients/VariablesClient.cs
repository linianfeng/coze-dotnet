using Coze.Sdk.Http;
using Coze.Sdk.Models.Variables;

namespace Coze.Sdk.Clients;

/// <summary>
/// 变量操作的实现。
/// 对应 Java SDK 中的 VariablesAPI.java。
/// </summary>
internal class VariablesClient : IVariablesClient
{
    private readonly CozeHttpClient _httpClient;

    public VariablesClient(CozeHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UpdateVariablesResponse> UpdateAsync(
        UpdateVariablesRequest request,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest("/v1/variables", HttpMethodType.Put);
        _httpClient.AddJsonBody(httpRequest, request);
        return await _httpClient.ExecuteAsync<UpdateVariablesResponse>(httpRequest, cancellationToken);
    }

    public async Task<RetrieveVariablesResponse> RetrieveAsync(
        RetrieveVariablesRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        request ??= new RetrieveVariablesRequest();

        var httpRequest = _httpClient.CreateGetRequest("/v1/variables");

        if (request.AppId != null)
        {
            httpRequest.AddQueryParameter("app_id", request.AppId);
        }

        if (request.BotId != null)
        {
            httpRequest.AddQueryParameter("bot_id", request.BotId);
        }

        if (request.ConnectorId != null)
        {
            httpRequest.AddQueryParameter("connector_id", request.ConnectorId);
        }

        if (request.ConnectorUid != null)
        {
            httpRequest.AddQueryParameter("connector_uid", request.ConnectorUid);
        }

        if (request.Keywords != null)
        {
            httpRequest.AddQueryParameter("keywords", request.Keywords);
        }

        return await _httpClient.ExecuteAsync<RetrieveVariablesResponse>(httpRequest, cancellationToken);
    }
}
