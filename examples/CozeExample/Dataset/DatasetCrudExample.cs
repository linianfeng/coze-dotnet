using Coze.Sdk;
using DatasetModel = Coze.Sdk.Models.Datasets.Dataset;

namespace CozeExample.Dataset;

/// <summary>
/// Demonstrates dataset CRUD operations: create, update, list, delete.
/// </summary>
public static class DatasetCrudExample
{
    public static async Task RunAsync(CozeClient client, string workspaceId)
    {
        // Create dataset
        var createRequest = new Coze.Sdk.Models.Datasets.CreateDatasetRequest
        {
            Name = $"Test Dataset {DateTimeOffset.UtcNow.ToUnixTimeSeconds()}",
            SpaceId = workspaceId,
            Description = "Test dataset from .NET SDK"
        };
        var createdDataset = await client.Datasets.CreateAsync(createRequest);
        var datasetId = createdDataset?.DatasetId;
        Console.WriteLine($"Created dataset: {datasetId}");

        // Wait for dataset to be ready
        await Task.Delay(1000);

        // Update dataset
        var updateRequest = new Coze.Sdk.Models.Datasets.UpdateDatasetRequest
        {
            DatasetId = datasetId,
            Name = $"Updated Dataset {DateTimeOffset.UtcNow.ToUnixTimeSeconds()}",
            Description = "Updated description"
        };
        await client.Datasets.UpdateAsync(updateRequest);
        Console.WriteLine($"Updated dataset: {datasetId}");

        // List datasets
        var listResult = await client.Datasets.ListAsync(new Coze.Sdk.Models.Datasets.ListDatasetsRequest
        {
            SpaceId = workspaceId,
            PageNumber = 1,
            PageSize = 10
        });
        Console.WriteLine($"Total datasets in workspace: {listResult.Total}");

        // Delete dataset
        await Task.Delay(1000);
        await client.Datasets.DeleteAsync(datasetId);
        Console.WriteLine($"Deleted dataset: {datasetId}");
    }
}
