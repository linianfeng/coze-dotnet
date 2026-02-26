using Coze.Sdk;
using Coze.Sdk.Authentication;
using Coze.Sdk.Models.Audio;

namespace CozeExample.Audio;

/// <summary>
/// This example demonstrates how to create transcription (speech-to-text).
/// </summary>
public static class TranscriptionExample
{
    public static async Task RunAsync(CozeClient client, string? audioFileId)
    {
        Console.WriteLine("=== Transcription (Speech-to-Text) Example ===");
        Console.WriteLine();

        if (string.IsNullOrEmpty(audioFileId))
        {
            Console.WriteLine("AUDIO_FILE_ID not set, skipping transcription example.");
            Console.WriteLine("Note: Transcription requires a file_id from a previously uploaded audio file.");
            Console.WriteLine();
            return;
        }

        try
        {
            var request = new CreateTranscriptionRequest
            {
                FileId = audioFileId,
                Language = "zh"
            };

            var result = await client.Audio.CreateTranscriptionAsync(request);

            Console.WriteLine($"Transcription completed:");
            Console.WriteLine($"Text: {result.Text}");
            Console.WriteLine($"Language: {result.Language}");
            Console.WriteLine($"Duration: {result.Duration}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine();
    }
}
