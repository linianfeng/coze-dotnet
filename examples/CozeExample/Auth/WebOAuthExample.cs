using Coze.Sdk;
using Coze.Sdk.Authentication;

namespace CozeExample.Auth;

/// <summary>
/// How to use Web OAuth for authorization and create CozeClient.
///
/// Web OAuth is suitable for server-side web applications that can securely store client secrets.
///
/// OAuth Flow:
/// 1. User clicks "Login with Coze"
/// 2. Redirect user to Coze authorization URL
/// 3. User authorizes the application
/// 4. Coze redirects back with authorization code
/// 5. Exchange code for access token
/// 6. Use access token to create CozeClient
/// </summary>
public static class WebOAuthExample
{
    public static void Run()
    {
        Console.WriteLine("=== Web OAuth Example ===");
        Console.WriteLine();

        var clientId = Environment.GetEnvironmentVariable("COZE_WEB_OAUTH_CLIENT_ID");
        var clientSecret = Environment.GetEnvironmentVariable("COZE_WEB_OAUTH_CLIENT_SECRET");
        var redirectUri = Environment.GetEnvironmentVariable("COZE_WEB_OAUTH_REDIRECT_URI");
        var baseUrl = Environment.GetEnvironmentVariable("COZE_API_BASE") ?? "https://api.coze.cn";

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(redirectUri))
        {
            Console.WriteLine("OAuth environment variables not set.");
            Console.WriteLine("Please set the following:");
            Console.WriteLine("  - COZE_WEB_OAUTH_CLIENT_ID");
            Console.WriteLine("  - COZE_WEB_OAUTH_CLIENT_SECRET");
            Console.WriteLine("  - COZE_WEB_OAUTH_REDIRECT_URI");
            Console.WriteLine();
            PrintSetupInstructions();
            return;
        }

        var oauthClient = new WebOAuthClient(new OAuthOptions
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            BaseUrl = baseUrl
        });

        // Step 1: Generate authorization URL
        var state = Guid.NewGuid().ToString();
        var authUrl = oauthClient.GetOAuthUrl(redirectUri, state);

        Console.WriteLine("Step 1: Authorization URL generated");
        Console.WriteLine(authUrl);
        Console.WriteLine();
        PrintOAuthFlowSteps();
    }

    /// <summary>
    /// Complete example showing how to exchange authorization code for token and create CozeClient.
    /// Call this method from your OAuth callback endpoint.
    /// </summary>
    public static async Task RunCompleteFlowAsync(string authorizationCode, string redirectUri)
    {
        Console.WriteLine("=== Web OAuth - Complete Flow ===");
        Console.WriteLine();

        var clientId = Environment.GetEnvironmentVariable("COZE_WEB_OAUTH_CLIENT_ID")
            ?? throw new InvalidOperationException("COZE_WEB_OAUTH_CLIENT_ID not set");
        var clientSecret = Environment.GetEnvironmentVariable("COZE_WEB_OAUTH_CLIENT_SECRET")
            ?? throw new InvalidOperationException("COZE_WEB_OAUTH_CLIENT_SECRET not set");
        var baseUrl = Environment.GetEnvironmentVariable("COZE_API_BASE") ?? "https://api.coze.cn";

        var options = new OAuthOptions
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            BaseUrl = baseUrl
        };

        // Method 1: Using OAuthCozeClientFactory
        Console.WriteLine("Method 1: Using OAuthCozeClientFactory");
        var (client1, token1) = await OAuthCozeClientFactory.CreateFromWebOAuthAsync(
            options,
            authorizationCode,
            redirectUri);

        Console.WriteLine($"Access Token: {token1.AccessToken?[..20]}...");
        Console.WriteLine($"Refresh Token: {token1.RefreshToken?[..20]}...");
        Console.WriteLine($"Expires In: {token1.ExpiresIn} seconds");
        Console.WriteLine();

        // Method 2: Manual approach (more control)
        Console.WriteLine("Method 2: Manual approach");
        var oauthClient = new WebOAuthClient(options);
        var token = await oauthClient.GetAccessTokenAsync(authorizationCode, redirectUri);

        // Create CozeClient using extension method
        using var client = token.CreateClient(baseUrl);

        Console.WriteLine("CozeClient created successfully!");
        Console.WriteLine();

        // Now you can use the client
        Console.WriteLine("You can now use the client:");
        Console.WriteLine("  - client.Chat for chat operations");
        Console.WriteLine("  - client.Bots for bot operations");
        Console.WriteLine("  - client.Workflows for workflow operations");
        Console.WriteLine("  - etc.");
        Console.WriteLine();

        // Example: Refresh token when expired
        if (token.IsExpired && !string.IsNullOrEmpty(token.RefreshToken))
        {
            Console.WriteLine("Token is expired, refreshing...");
            var refreshedToken = await oauthClient.RefreshTokenAsync(token.RefreshToken, CancellationToken.None);
            Console.WriteLine($"New Access Token: {refreshedToken.AccessToken?[..20]}...");
        }
    }

    private static void PrintSetupInstructions()
    {
        Console.WriteLine("To create an OAuth App:");
        Console.WriteLine("  1. Visit: https://www.coze.cn/open/oauth/apps");
        Console.WriteLine("  2. Create an OAuth App of type 'Web application'");
        Console.WriteLine("  3. Get the client ID and client secret");
        Console.WriteLine("  4. Configure redirect URI (e.g., http://localhost:8080/callback)");
        Console.WriteLine();
        Console.WriteLine("Required OAuth Scopes:");
        Console.WriteLine("  - bot:offline_access - Access to bot resources");
        Console.WriteLine("  - conversation:offline_access - Access to conversation resources");
        Console.WriteLine();
    }

    private static void PrintOAuthFlowSteps()
    {
        Console.WriteLine("OAuth Flow Steps:");
        Console.WriteLine("  1. Direct the user to the authorization URL");
        Console.WriteLine("  2. User clicks authorize");
        Console.WriteLine("  3. Coze redirects to your redirect_uri with code and state");
        Console.WriteLine("  4. Verify state matches original state");
        Console.WriteLine("  5. Exchange the code for an access token:");
        Console.WriteLine();
        Console.WriteLine("     // ASP.NET Core Controller Example:");
        Console.WriteLine("     [HttpGet(\"/callback\")]");
        Console.WriteLine("     public async Task<IActionResult> Callback(string code, string state)");
        Console.WriteLine("     {");
        Console.WriteLine("         // Verify state");
        Console.WriteLine("         if (state != expectedState) return BadRequest();");
        Console.WriteLine();
        Console.WriteLine("         // Exchange code for token and create client");
        Console.WriteLine("         var (client, token) = await OAuthCozeClientFactory.CreateFromWebOAuthAsync(");
        Console.WriteLine("             options, code, redirectUri);");
        Console.WriteLine();
        Console.WriteLine("         // Store refresh token securely");
        Console.WriteLine("         await StoreTokenAsync(token.RefreshToken);");
        Console.WriteLine();
        Console.WriteLine("         // Use client for API calls");
        Console.WriteLine("         return View();");
        Console.WriteLine("     }");
        Console.WriteLine();
    }
}
