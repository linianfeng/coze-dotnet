using Coze.Sdk;
using Coze.Sdk.Models.Commerce;

namespace CozeExample.Commerce;

/// <summary>
/// Demonstrates commerce benefit operations: create and list bill download tasks.
/// </summary>
public static class BillTaskExample
{
    public static async Task RunAsync(CozeClient client)
    {
        // Create a bill download task
        var createRequest = new CreateBillDownloadTaskRequest
        {
            StartedAt = DateTimeOffset.UtcNow.AddDays(-30).ToUnixTimeSeconds(),
            EndedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        var createResponse = await client.Commerce.Bills.CreateTaskAsync(createRequest);
        Console.WriteLine($"Created bill task: {createResponse.TaskId}");

        // Wait a bit for the task to process
        await Task.Delay(1000);

        // List bill download tasks
        var listRequest = new ListBillDownloadTasksRequest
        {
            PageNumber = 1,
            PageSize = 10
        };

        var listResponse = await client.Commerce.Bills.ListTasksAsync(listRequest);
        Console.WriteLine($"Found {listResponse.TaskInfos?.Count ?? 0} bill tasks (Total: {listResponse.Total})");
        foreach (var task in listResponse.TaskInfos ?? Array.Empty<BillTaskInfo>())
        {
            Console.WriteLine($"  - Task: {task.TaskId}, Files: {task.FileUrls?.Count ?? 0}");
        }
    }
}
