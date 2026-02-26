using Coze.Sdk;
using WorkspaceModel = Coze.Sdk.Models.Workspaces.Workspace;

namespace CozeExample.Workspace;

/// <summary>
/// Demonstrates listing workspaces the user has access to.
/// </summary>
public static class WorkspaceListExample
{
    public static async Task RunAsync(CozeClient client)
    {
        var result = await client.Workspaces.ListAsync(new Coze.Sdk.Models.Workspaces.ListWorkspacesRequest
        {
            PageNumber = 1,
            PageSize = 10
        });

        Console.WriteLine($"Found {result.Total} workspaces:");
        foreach (var workspace in result.Workspaces ?? Array.Empty<WorkspaceModel>())
        {
            Console.WriteLine($"  - {workspace.Name} ({workspace.WorkspaceType})");
        }
    }
}
