using Coze.Sdk;
using Coze.Sdk.Authentication;

namespace CozeExample.Auth;

/// <summary>
/// Demonstrates using personal access token (PAT) for authentication.
/// </summary>
public static class TokenAuthExample
{
    public static void Run()
    {
        var token = Environment.GetEnvironmentVariable("COZE_API_TOKEN");
        var baseUrl = Environment.GetEnvironmentVariable("COZE_API_BASE") ?? "https://api.coze.cn";

        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("COZE_API_TOKEN not set. Get your token from: https://www.coze.cn/open/oauth/pats");
            return;
        }

        using var client = new CozeClient(new CozeOptions
        {
            Auth = new TokenAuth(token),
            BaseUrl = baseUrl
        });

        Console.WriteLine("TokenAuth: Client initialized successfully");
    }
}
