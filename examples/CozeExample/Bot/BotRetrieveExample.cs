using Coze.Sdk;
using Coze.Sdk.Models.Bots;

namespace CozeExample.Bot;

/// <summary>
/// Demonstrates bot operations: retrieve and list bots in a workspace.
/// </summary>
public static class BotRetrieveExample
{
    public static async Task RunAsync(CozeClient client, string? botId, string? workspaceId)
    {
        // Retrieve a specific bot
        if (!string.IsNullOrEmpty(botId))
        {
            var bot = await client.Bots.RetrieveAsync(botId);
            Console.WriteLine($"Bot: {bot.Name} (ID: {bot.Id})");
        }

        // List bots in workspace
        if (!string.IsNullOrEmpty(workspaceId))
        {
            var result = await client.Bots.ListAsync(new ListBotsRequest
            {
                SpaceId = workspaceId,
                PageNumber = 1,
                PageSize = 10
            });
            Console.WriteLine($"Found {result.Total} bots in workspace");
            foreach (var bot in result.Items ?? Array.Empty<SimpleBot>())
            {
                Console.WriteLine($"  - {bot.Name}");
            }
        }
    }
}
