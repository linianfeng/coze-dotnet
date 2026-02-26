using Coze.Sdk;
using Coze.Sdk.Models.Chat;

namespace CozeExample.Chat;

/// <summary>
/// Demonstrates streaming chat functionality - receives real-time responses from the bot.
/// </summary>
public static class StreamChatExample
{
    public static async Task RunAsync(CozeClient client, string botId)
    {
        var request = new ChatRequest
        {
            BotId = botId,
            UserId = "example-user-stream",
            Messages = new List<Message>
            {
                Message.BuildUserQuestionText("What can you do? Answer in one short sentence.")
            }
        };

        var responseBuilder = new System.Text.StringBuilder();

        await foreach (var evt in client.Chat.StreamAsync(request))
        {
            switch (evt.EventType)
            {
                case ChatEventType.ConversationMessageDelta:
                    responseBuilder.Append(evt.Message?.Content);
                    break;
                case ChatEventType.ConversationChatCompleted:
                    var tokenCount = evt.Chat?.Usage?.TokenCount ?? 0;
                    Console.WriteLine($"[Stream completed - {tokenCount} tokens used]");
                    break;
                case ChatEventType.Error:
                    Console.WriteLine($"[Error: {evt.Message?.Content}]");
                    break;
            }
        }

        var fullResponse = responseBuilder.ToString();
        if (!string.IsNullOrEmpty(fullResponse))
        {
            Console.WriteLine($"Response: {fullResponse.Substring(0, Math.Min(100, fullResponse.Length))}...");
        }
    }
}
