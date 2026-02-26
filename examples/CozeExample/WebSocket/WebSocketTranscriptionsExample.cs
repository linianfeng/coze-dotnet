using Coze.Sdk;
using Coze.Sdk.WebSocket;

namespace CozeExample.WebSocket;

/// <summary>
/// This example demonstrates how to use WebSocket for real-time speech-to-text.
/// Note: WebSocket support requires additional setup. This is a placeholder example.
/// </summary>
public static class WebSocketTranscriptionsExample
{
    public static Task RunAsync(CozeClient client, string? audioFilePath)
    {
        Console.WriteLine("=== WebSocket Transcriptions (Speech-to-Text) Example ===");
        Console.WriteLine();

        if (string.IsNullOrEmpty(audioFilePath) || !System.IO.File.Exists(audioFilePath))
        {
            Console.WriteLine("Audio file not found or AUDIO_FILE_PATH not set.");
            Console.WriteLine();
        }

        Console.WriteLine("WebSocket transcriptions requires direct client instantiation.");
        Console.WriteLine();
        Console.WriteLine("Example usage:");
        Console.WriteLine(@"
// Create a custom callback handler
public class MyTranscriptionsHandler : TranscriptionsWebSocketCallbackHandler
{
    public override Task OnTranscriptionsMessageUpdateAsync(TranscriptionsWebSocketClient client, TranscriptionsMessageUpdateEvent evt)
    {
        Console.WriteLine($""Transcription: {evt.Data?.Text}"");
        return Task.CompletedTask;
    }

    public override Task OnTranscriptionsMessageCompletedAsync(TranscriptionsWebSocketClient client, TranscriptionsMessageCompletedEvent evt)
    {
        Console.WriteLine(""Transcription completed!"");
        return Task.CompletedTask;
    }
}

// Create and connect
var wsClient = new TranscriptionsWebSocketClient(baseUrl, auth, new MyTranscriptionsHandler());
await wsClient.ConnectAsync();

// Send audio data
await wsClient.InputAudioBufferAppendAsync(audioBytes);
await wsClient.InputAudioBufferCompleteAsync();

// Close when done
await wsClient.CloseAsync();
");
        Console.WriteLine();
        return Task.CompletedTask;
    }
}
