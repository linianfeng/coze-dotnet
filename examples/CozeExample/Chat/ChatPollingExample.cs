using Coze.Sdk;
using Coze.Sdk.Models.Chat;

namespace CozeExample.Chat;

/// <summary>
/// Demonstrates polling chat - waits for complete response before returning.
/// </summary>
public static class ChatPollingExample
{
    public static async Task RunAsync(CozeClient client, string botId)
    {
        var request = new ChatRequest
        {
            BotId = botId,
            UserId = "example-user-poll",
            Messages = new List<Message>
            {
                Message.BuildUserQuestionText("What is 2+2?")
            }
        };

        var result = await client.Chat.CreateAndPollAsync(request, TimeSpan.FromSeconds(60));
        var answerMessage = result.Messages?.FirstOrDefault(m => m.Type == MessageType.Answer);

        if (answerMessage != null)
        {
            Console.WriteLine($"Answer: {answerMessage.Content}");
        }
        Console.WriteLine($"Tokens used: {result.Chat?.Usage?.TokenCount ?? 0}");
    }
}
