using Coze.Sdk;
using DatasetModel = Coze.Sdk.Models.Datasets.Dataset;

namespace CozeExample.Dataset;

/// <summary>
/// This example demonstrates how to list datasets.
/// </summary>
public static class DatasetListExample
{
    public static async Task RunAsync(CozeClient client, string workspaceId)
    {
        Console.WriteLine("=== Dataset List Example ===");
        Console.WriteLine();

        try
        {
            var request = new Coze.Sdk.Models.Datasets.ListDatasetsRequest
            {
                SpaceId = workspaceId,
                PageNumber = 1,
                PageSize = 10
            };

            var result = await client.Datasets.ListAsync(request);

            Console.WriteLine($"Total: {result.Total}");

            foreach (var dataset in result.Datasets ?? Array.Empty<DatasetModel>())
            {
                Console.WriteLine($"  - ID: {dataset.DatasetId}");
                Console.WriteLine($"    Name: {dataset.Name}");
                Console.WriteLine($"    Description: {dataset.Description}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine();
    }
}
