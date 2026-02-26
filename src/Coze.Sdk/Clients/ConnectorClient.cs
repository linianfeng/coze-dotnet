using Coze.Sdk.Http;
using Coze.Sdk.Models.Connectors;

namespace Coze.Sdk.Clients;

/// <summary>
/// 连接器操作的实现。
/// 对应 Java SDK 中的 ConnectorAPI.java。
/// </summary>
internal class ConnectorClient : IConnectorClient
{
    private readonly CozeHttpClient _httpClient;

    public ConnectorClient(CozeHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<InstallConnectorResponse> InstallAsync(
        string connectorId,
        InstallConnectorRequest request,
        CancellationToken cancellationToken = default)
    {
        var endpoint = $"v1/connectors/{connectorId}/install";
        var httpRequest = _httpClient.CreateRequest(endpoint, HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, request);
        return await _httpClient.ExecuteAsync<InstallConnectorResponse>(httpRequest, cancellationToken);
    }
}
