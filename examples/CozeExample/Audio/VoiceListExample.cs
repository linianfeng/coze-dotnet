using Coze.Sdk;
using Coze.Sdk.Models.Audio;

namespace CozeExample.Audio;

/// <summary>
/// This example demonstrates how to list available voices.
/// </summary>
public static class VoiceListExample
{
    public static async Task RunAsync(CozeClient client)
    {
        Console.WriteLine("=== Voice List Example ===");
        Console.WriteLine();

        try
        {
            var request = new ListVoicesRequest
            {
                PageNumber = 1,
                PageSize = 10
            };

            var result = await client.Audio.ListVoicesAsync(request);

            Console.WriteLine($"Total voices: {result.Voices?.Count ?? 0}");

            foreach (var voice in result.Voices ?? Array.Empty<Voice>())
            {
                Console.WriteLine($"  - ID: {voice.VoiceId}");
                Console.WriteLine($"    Name: {voice.Name}");
                Console.WriteLine($"    Language: {voice.LanguageCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine();
    }
}
