using Coze.Sdk;
using Coze.Sdk.Models.Conversations;

namespace CozeExample.Conversation;

/// <summary>
/// This example demonstrates how to create, list, and clear messages in a conversation.
/// </summary>
public static class ConversationMessageExample
{
    public static async Task RunAsync(CozeClient client, string conversationId)
    {
        Console.WriteLine("=== Conversation Message Example ===");
        Console.WriteLine();

        try
        {
            // Create a message
            var createResult = await client.Conversations.Messages.CreateAsync(
                new CreateConversationMessageRequest
                {
                    ConversationId = conversationId,
                    Role = ConversationMessageRole.User,
                    Content = "Hello from .NET SDK!",
                    ContentType = ConversationMessageContentType.Text
                });

            Console.WriteLine($"Created message ID: {createResult.Id}");
            Console.WriteLine($"Role: {createResult.Role}");
            Console.WriteLine($"Content: {createResult.Content}");

            // List messages
            var listResult = await client.Conversations.Messages.ListAsync(
                new ListConversationMessagesRequest
                {
                    ConversationId = conversationId
                });

            Console.WriteLine($"Messages count: {listResult.Messages?.Count ?? 0}");

            // Clear the conversation
            await client.Conversations.ClearAsync(conversationId);
            Console.WriteLine("Conversation cleared.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine();
    }
}
