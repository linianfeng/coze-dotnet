using Coze.Sdk.Models.Workspaces;

namespace Coze.Sdk.Clients;

/// <summary>
/// 工作空间操作接口。
/// 对应 Java SDK 中的 WorkspaceService.java。
/// </summary>
public interface IWorkspaceClient
{
    /// <summary>
    /// 列出当前用户可访问的工作空间。
    /// </summary>
    Task<ListWorkspacesResponse> ListAsync(ListWorkspacesRequest? request = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取当前默认工作空间。
    /// </summary>
    Task<Workspace> GetCurrentAsync(CancellationToken cancellationToken = default);
}
