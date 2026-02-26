using Coze.Sdk.Authentication;

namespace CozeExample.Auth;

/// <summary>
/// How to use Device OAuth for CLI/TV apps.
/// </summary>
public static class DeviceOAuthExample
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Device OAuth Example ===");
        Console.WriteLine();

        var clientId = Environment.GetEnvironmentVariable("COZE_DEVICE_OAUTH_CLIENT_ID");
        var baseUrl = Environment.GetEnvironmentVariable("COZE_API_BASE") ?? "https://api.coze.cn";

        if (string.IsNullOrEmpty(clientId))
        {
            Console.WriteLine("COZE_DEVICE_OAUTH_CLIENT_ID environment variable not set.");
            Console.WriteLine();
            Console.WriteLine("Device OAuth is suitable for:");
            Console.WriteLine("  - CLI applications");
            Console.WriteLine("  - TV apps");
            Console.WriteLine("  - Devices without a browser");
            Console.WriteLine();
            Console.WriteLine("To use Device OAuth:");
            Console.WriteLine("  1. Create an OAuth App at https://www.coze.cn/open/oauth/apps");
            Console.WriteLine("  2. Use Device OAuth flow for device authorization");
            return;
        }

        var deviceClient = new DeviceOAuthClient(new OAuthOptions
        {
            ClientId = clientId,
            BaseUrl = baseUrl
        });

        try
        {
            // Get device code
            var deviceCode = await deviceClient.GetDeviceCodeAsync();

            Console.WriteLine("Device authorization initiated:");
            Console.WriteLine($"  Verification URL: {deviceCode.VerificationUrl}");
            Console.WriteLine($"  User Code: {deviceCode.UserCode}");
            Console.WriteLine($"  Expires in: {deviceCode.ExpiresIn} seconds");
            Console.WriteLine();
            Console.WriteLine("Please visit the URL and enter the code to authorize.");
            Console.WriteLine();
            Console.WriteLine("After authorization, poll for token:");
            Console.WriteLine("  var token = await deviceClient.PollAccessTokenAsync(deviceCode.DeviceCode);");
            Console.WriteLine("  Console.WriteLine($\"Access Token: {token.AccessToken}\");");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine();
    }
}
