using Coze.Sdk.Http;
using Coze.Sdk.Models.Workspaces;

namespace Coze.Sdk.Clients;

/// <summary>
/// 工作空间操作的实现。
/// 对应 Java SDK 中的 WorkspaceService.java。
/// </summary>
internal class WorkspaceClient : IWorkspaceClient
{
    private readonly CozeHttpClient _httpClient;

    public WorkspaceClient(CozeHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ListWorkspacesResponse> ListAsync(ListWorkspacesRequest? request = null, CancellationToken cancellationToken = default)
    {
        request ??= new ListWorkspacesRequest();

        var httpRequest = _httpClient.CreateGetRequest("/v1/workspaces");

        if (request.PageNumber.HasValue)
        {
            httpRequest.AddQueryParameter("page_num", request.PageNumber.Value.ToString());
        }

        if (request.PageSize.HasValue)
        {
            httpRequest.AddQueryParameter("page_size", request.PageSize.Value.ToString());
        }

        return await _httpClient.ExecuteAsync<ListWorkspacesResponse>(httpRequest, cancellationToken);
    }

    public async Task<Workspace> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        var response = await ListAsync(new ListWorkspacesRequest { PageSize = 1 }, cancellationToken);

        var workspace = response.Workspaces?.FirstOrDefault();
        if (workspace == null)
        {
            throw new InvalidOperationException("没有可用的工作空间");
        }

        return workspace;
    }
}
