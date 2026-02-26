using Coze.Sdk;
using Coze.Sdk.Authentication;
using Coze.Sdk.Models.Audio;

namespace CozeExample.Audio;

/// <summary>
/// This example demonstrates how to create speech (text-to-speech).
/// </summary>
public static class SpeechExample
{
    public static async Task RunAsync(CozeClient client, string? voiceId)
    {
        Console.WriteLine("=== Speech (Text-to-Speech) Example ===");
        Console.WriteLine();

        var voice = voiceId ?? "longxiaochun";

        try
        {
            var request = new CreateSpeechRequest
            {
                Input = "Hello! This is a test from Coze .NET SDK.",
                VoiceId = voice,
                ResponseFormat = AudioFormat.Mp3,
                Speed = 1.0f
            };

            var result = await client.Audio.CreateSpeechAsync(request);

            Console.WriteLine($"Speech created successfully");
            Console.WriteLine($"Content type: {result.ContentType}");

            // Save to file
            var outputPath = "output_speech.mp3";
            using (var fileStream = System.IO.File.Create(outputPath))
            {
                if (result.AudioStream != null)
                {
                    await result.AudioStream.CopyToAsync(fileStream);
                }
            }
            Console.WriteLine($"Saved to: {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine();
    }
}
