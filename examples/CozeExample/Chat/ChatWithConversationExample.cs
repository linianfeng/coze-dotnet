using Coze.Sdk;
using Coze.Sdk.Models.Chat;

namespace CozeExample.Chat;

/// <summary>
/// Demonstrates multi-turn conversation using conversation ID to maintain context.
/// </summary>
public static class ChatWithConversationExample
{
    public static async Task RunAsync(CozeClient client, string botId)
    {
        // First turn - create conversation and ask bot to remember something
        var firstRequest = new ChatRequest
        {
            BotId = botId,
            UserId = "example-user-conversation",
            Messages = new List<Message>
            {
                Message.BuildUserQuestionText("Remember the number 42.")
            }
        };

        var firstResult = await client.Chat.CreateAndPollAsync(firstRequest, TimeSpan.FromSeconds(60));
        var conversationId = firstResult.Chat?.ConversationId;

        // Second turn - ask about the remembered number (should work with context)
        var secondRequest = new ChatRequest
        {
            BotId = botId,
            ConversationId = conversationId,
            UserId = "example-user-conversation",
            Messages = new List<Message>
            {
                Message.BuildUserQuestionText("What number did I ask you to remember?")
            }
        };

        var secondResult = await client.Chat.CreateAndPollAsync(secondRequest, TimeSpan.FromSeconds(60));
        var answer = secondResult.Messages?.FirstOrDefault(m => m.Type == MessageType.Answer)?.Content;

        Console.WriteLine($"Conversation ID: {conversationId}");
        Console.WriteLine($"Bot remembered: {answer}");
    }
}
