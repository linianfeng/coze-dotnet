using Coze.Sdk;
using Coze.Sdk.Authentication;
using Coze.Sdk.Models.Audio;

namespace CozeExample.Audio;

/// <summary>
/// This example demonstrates how to create an audio room for real-time communication.
/// </summary>
public static class RoomExample
{
    public static async Task RunAsync(CozeClient client, string botId, string? voiceId)
    {
        Console.WriteLine("=== Audio Room Example ===");
        Console.WriteLine();

        var voice = voiceId ?? "longxiaochun";

        try
        {
            var request = new CreateRoomRequest
            {
                BotId = botId,
                VoiceId = voice
            };

            var result = await client.Audio.CreateRoomAsync(request);

            Console.WriteLine($"Room created successfully:");
            Console.WriteLine($"Room ID: {result.RoomId}");
            Console.WriteLine($"WebSocket URL: {result.WebSocketUrl}");
            Console.WriteLine();
            Console.WriteLine("Use the WebSocket URL to connect for real-time audio communication.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine();
    }
}
