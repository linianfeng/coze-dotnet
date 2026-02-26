using Coze.Sdk;
using Coze.Sdk.WebSocket;

namespace CozeExample.WebSocket;

/// <summary>
/// This example demonstrates how to use WebSocket for real-time text-to-speech.
/// Note: WebSocket support requires additional setup. This is a placeholder example.
/// </summary>
public static class WebSocketSpeechExample
{
    public static Task RunAsync(CozeClient client)
    {
        Console.WriteLine("=== WebSocket Speech (Text-to-Speech) Example ===");
        Console.WriteLine();
        Console.WriteLine("WebSocket speech synthesis requires direct client instantiation.");
        Console.WriteLine();
        Console.WriteLine("Example usage:");
        Console.WriteLine(@"
// Create a custom callback handler
public class MySpeechHandler : SpeechWebSocketCallbackHandler
{
    private readonly List<byte> _audioBuffer = new();

    public override Task OnSpeechAudioUpdateAsync(SpeechWebSocketClient client, SpeechAudioUpdateEvent evt)
    {
        if (evt.Data?.Audio != null)
        {
            _audioBuffer.AddRange(evt.Data.Audio);
        }
        return Task.CompletedTask;
    }

    public override Task OnSpeechAudioCompletedAsync(SpeechWebSocketClient client, SpeechAudioCompletedEvent evt)
    {
        Console.WriteLine(""Audio synthesis completed!"");
        // Save audio buffer to file...
        return Task.CompletedTask;
    }
}

// Create and connect
var wsClient = new SpeechWebSocketClient(baseUrl, auth, new MySpeechHandler());
await wsClient.ConnectAsync();

// Send text to synthesize
await wsClient.InputTextBufferAppendAsync(""Hello, this is a test."");
await wsClient.InputTextBufferCompleteAsync();

// Close when done
await wsClient.CloseAsync();
");
        Console.WriteLine();
        return Task.CompletedTask;
    }
}
