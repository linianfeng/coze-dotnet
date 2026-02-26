using Coze.Sdk;
using Coze.Sdk.Models.Files;
using Coze.Sdk.Models.Workflows;

namespace CozeExample.Workflow;

/// <summary>
/// 工作流执行示例 - 运行工作流并等待完成。
/// </summary>
public static class WorkflowRunExample
{
    public static async Task RunAsync(CozeClient client, string workflowId, string filePath )
    {
        var request = new WorkflowRequest
        {
            WorkflowId = workflowId,
            Parameters = new Dictionary<string, object?>
            {
                ["input"] = "Hello from .NET SDK"
            }
        };

        var result = await client.Workflows.RunAsync(request);
        Console.WriteLine($"工作流执行完成");
        Console.WriteLine($"  Execute ID: {result.ExecuteId}");
        Console.WriteLine($"  Debug URL: {result.DebugUrl}");
  

        if (!string.IsNullOrEmpty(result.Data))
        {
            Console.WriteLine($"  Data: {result.Data}");
        }


        if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
        {
            Console.WriteLine("FILE_PATH not set or file not found, skipping.");
            return;
        }

        // Upload file
        var uploadResult = await client.Files.UploadAsync(UploadFileRequest.FromPath(filePath));
        var fileId = uploadResult.File?.Id;

        Console.WriteLine($"Uploaded file: {fileId}, size: {uploadResult.File?.Bytes} bytes");
        string fileWorkFlowId = "7530243569945804834";

        request = new WorkflowRequest
        {
            WorkflowId = fileWorkFlowId,
            Parameters = new Dictionary<string, object?>
            {
                ["pofile"] = new { file_id =　fileId }
            }
        };

        result = await client.Workflows.RunAsync(request);
        Console.WriteLine($"文件参数工作流执行完成");
        Console.WriteLine($"  Execute ID: {result.ExecuteId}");
        Console.WriteLine($"  Debug URL: {result.DebugUrl}");
        if (!string.IsNullOrEmpty(result.Data))
        {
            Console.WriteLine($"  Data: {result.Data}");
        }
    }
}
