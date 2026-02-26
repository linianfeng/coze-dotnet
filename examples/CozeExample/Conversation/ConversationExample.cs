using Coze.Sdk;
using Coze.Sdk.Models.Conversations;

namespace CozeExample.Conversation;

/// <summary>
/// 会话操作示例：创建、获取、列出。
/// </summary>
public static class ConversationExample
{
    public static async Task<string?> RunAsync(CozeClient client, string botId)
    {
        // Create conversation
        var createResult = await client.Conversations.CreateAsync(new CreateConversationRequest
        {
            MetaData = new Dictionary<string, string>
            {
                ["source"] = "dotnet-sdk-example"
            }
        });

        var conversationId = createResult.Conversation?.Id;
        Console.WriteLine($"Created conversation: {conversationId}");

        // Retrieve conversation
        var retrieved = await client.Conversations.RetrieveAsync(conversationId!);
        Console.WriteLine($"Retrieved conversation: {retrieved.Id}");

        // List conversations
        var listResult = await client.Conversations.ListAsync(new ListConversationsRequest
        {
            BotId = botId,
            PageNumber = 1,
            PageSize = 5
        });
        Console.WriteLine($"Found {listResult.Conversations?.Count ?? 0} conversations (has more: {listResult.HasMore})");

        return conversationId;
    }
}
