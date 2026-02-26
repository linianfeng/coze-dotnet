using Coze.Sdk;
using Coze.Sdk.Models.Workflows;

namespace CozeExample.Workflow;

/// <summary>
/// This example describes how to use the workflow interface to stream workflow execution.
/// </summary>
public static class WorkflowStreamExample
{
    public static async Task RunAsync(CozeClient client, string workflowId)
    {
        Console.WriteLine("=== Workflow Stream Example ===");
        Console.WriteLine();

        var request = new WorkflowRequest
        {
            WorkflowId = workflowId,
            Parameters = new Dictionary<string, object?>
            {
                ["input"] = "Stream this message"
            }
        };

        try
        {
            Console.WriteLine("Streaming workflow events:");

            await foreach (var evt in client.Workflows.StreamAsync(request))
            {
                Console.WriteLine($"  Event: {evt.EventType}");
                if (!string.IsNullOrEmpty(evt.Message))
                {
                    Console.WriteLine($"    Message: {evt.Message}");
                }
            }

            Console.WriteLine("Stream completed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine();
    }
}
