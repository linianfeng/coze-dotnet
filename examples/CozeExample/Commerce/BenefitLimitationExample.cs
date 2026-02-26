using Coze.Sdk;
using Coze.Sdk.Models.Commerce;

namespace CozeExample.Commerce;

/// <summary>
/// Demonstrates commerce benefit limitation operations: list.
/// </summary>
public static class BenefitLimitationExample
{
    public static async Task RunAsync(CozeClient client)
    {
        // List benefit limitations
        var listRequest = new ListBenefitLimitationsRequest
        {
            EntityType = BenefitEntityType.SingleDevice,
            PageToken = ""
        };

        var listResponse = await client.Commerce.Limitations.ListAsync(listRequest);
        Console.WriteLine($"Found {listResponse.BenefitInfos?.Count ?? 0} benefit limitations");
        foreach (var benefit in listResponse.BenefitInfos ?? Array.Empty<BenefitInfo>())
        {
            Console.WriteLine($"  - Benefit: {benefit.BenefitId}, Type: {benefit.BenefitType}, Limit: {benefit.Limit}");
        }
    }
}
