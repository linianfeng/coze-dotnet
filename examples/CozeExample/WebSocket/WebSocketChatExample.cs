using Coze.Sdk;
using Coze.Sdk.WebSocket;

namespace CozeExample.WebSocket;

/// <summary>
/// This example demonstrates how to use WebSocket for real-time chat.
/// Note: WebSocket client creation requires internal factory access.
/// This example shows the proper usage pattern.
/// </summary>
public static class WebSocketChatExample
{
    public static Task RunAsync(CozeClient client, string botId)
    {
        Console.WriteLine("=== WebSocket Chat Example ===");
        Console.WriteLine();
        Console.WriteLine("WebSocket chat requires accessing the internal ChatWebSocketClient.");
        Console.WriteLine("In a real application, you would create a service that manages WebSocket connections.");
        Console.WriteLine();
        Console.WriteLine("Example usage pattern:");
        Console.WriteLine(@"
// The ChatWebSocketClient is created internally by the SDK
// For WebSocket chat, use the chat client's streaming API instead:

var request = new ChatRequest
{
    BotId = botId,
    UserId = ""user-123"",
    Messages = new List<Message>
    {
        Message.BuildUserQuestionText(""Hello!"")
    }
};

await foreach (var evt in client.Chat.StreamAsync(request))
{
    if (evt.EventType == ChatEventType.ConversationMessageDelta)
    {
        Console.Write(evt.Message?.Content);
    }
}
");
        Console.WriteLine();
        Console.WriteLine("[WebSocket example demonstrated - use HTTP streaming for simple chat]");
        return Task.CompletedTask;
    }
}
