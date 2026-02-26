using Coze.Sdk;
using Coze.Sdk.Authentication;

namespace CozeExample.Auth;

/// <summary>
/// How to use PKCE OAuth for authorization and create CozeClient.
///
/// PKCE (Proof Key for Code Exchange) is suitable for:
/// - Mobile applications
/// - Native desktop applications
/// - Single Page Applications (SPAs)
/// - Any public client that cannot securely store client secrets
///
/// PKCE Flow:
/// 1. Generate code_verifier and code_challenge
/// 2. Redirect user to Coze authorization URL with code_challenge
/// 3. User authorizes the application
/// 4. Coze redirects back with authorization code
/// 5. Exchange code + code_verifier for access token
/// 6. Use access token to create CozeClient
/// </summary>
public static class PkceOAuthExample
{
    public static void Run()
    {
        Console.WriteLine("=== PKCE OAuth Example ===");
        Console.WriteLine();

        var clientId = Environment.GetEnvironmentVariable("COZE_PKCE_OAUTH_CLIENT_ID");
        var redirectUri = Environment.GetEnvironmentVariable("COZE_PKCE_OAUTH_REDIRECT_URI");
        var baseUrl = Environment.GetEnvironmentVariable("COZE_API_BASE") ?? "https://api.coze.cn";

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(redirectUri))
        {
            Console.WriteLine("PKCE OAuth environment variables not set.");
            Console.WriteLine("Please set the following:");
            Console.WriteLine("  - COZE_PKCE_OAUTH_CLIENT_ID");
            Console.WriteLine("  - COZE_PKCE_OAUTH_REDIRECT_URI (e.g., myapp://callback)");
            Console.WriteLine();
            PrintSetupInstructions();
            return;
        }

        var options = new OAuthOptions
        {
            ClientId = clientId,
            BaseUrl = baseUrl
        };

        var oauthClient = new PkceOAuthClient(options);

        // Step 1: Generate authorization URL with PKCE
        var state = Guid.NewGuid().ToString();
        var authResponse = oauthClient.GenerateOAuthUrl(
            redirectUri,
            state,
            CodeChallengeMethod.S256); // Using SHA-256 for better security

        Console.WriteLine("Step 1: Authorization URL with PKCE generated");
        Console.WriteLine(authResponse.AuthorizationUrl);
        Console.WriteLine();
        Console.WriteLine($"Code Verifier (store securely): {authResponse.CodeVerifier}");
        Console.WriteLine();
        PrintPkceFlowSteps(authResponse, redirectUri, baseUrl);
    }

    /// <summary>
    /// Complete example showing how to exchange authorization code for token using PKCE.
    /// Call this method after receiving the authorization code from OAuth callback.
    /// </summary>
    public static async Task RunCompleteFlowAsync(string authorizationCode, string redirectUri, string codeVerifier)
    {
        Console.WriteLine("=== PKCE OAuth - Complete Flow ===");
        Console.WriteLine();

        var clientId = Environment.GetEnvironmentVariable("COZE_PKCE_OAUTH_CLIENT_ID")
            ?? throw new InvalidOperationException("COZE_PKCE_OAUTH_CLIENT_ID not set");
        var baseUrl = Environment.GetEnvironmentVariable("COZE_API_BASE") ?? "https://api.coze.cn";

        var options = new OAuthOptions
        {
            ClientId = clientId,
            BaseUrl = baseUrl
        };

        // Method 1: Using OAuthCozeClientFactory (Recommended)
        Console.WriteLine("Method 1: Using OAuthCozeClientFactory");
        var (client1, token1) = await OAuthCozeClientFactory.CreateFromPkceOAuthAsync(
            options,
            authorizationCode,
            redirectUri,
            codeVerifier);

        var accessTokenPreview = token1.AccessToken?.Length > 20 ? token1.AccessToken[..20] : token1.AccessToken;
        var refreshTokenPreview = token1.RefreshToken?.Length > 20 ? token1.RefreshToken[..20] : token1.RefreshToken;
        Console.WriteLine($"Access Token: {accessTokenPreview}...");
        Console.WriteLine($"Refresh Token: {refreshTokenPreview}...");
        Console.WriteLine($"Expires In: {token1.ExpiresIn} seconds");
        Console.WriteLine();

        // Method 2: Manual approach (More control)
        Console.WriteLine("Method 2: Manual approach");
        var oauthClient = new PkceOAuthClient(options);
        var oauthToken = await oauthClient.GetAccessTokenAsync(authorizationCode, redirectUri, codeVerifier);

        // Create CozeClient using extension method
        using var client = oauthToken.CreateClient(baseUrl);

        Console.WriteLine("CozeClient created successfully!");
        Console.WriteLine();

        // Now you can use the client
        PrintClientUsageExamples(client);
    }

    /// <summary>
    /// Demonstrates the complete PKCE flow for a desktop/mobile application.
    /// </summary>
    public static async Task RunDesktopFlowAsync()
    {
        Console.WriteLine("=== PKCE OAuth - Desktop/Mobile Flow ===");
        Console.WriteLine();

        var clientId = Environment.GetEnvironmentVariable("COZE_PKCE_OAUTH_CLIENT_ID");
        if (string.IsNullOrEmpty(clientId))
        {
            Console.WriteLine("COZE_PKCE_OAUTH_CLIENT_ID not set");
            return;
        }

        var baseUrl = Environment.GetEnvironmentVariable("COZE_API_BASE") ?? "https://api.coze.cn";
        var redirectUri = Environment.GetEnvironmentVariable("COZE_PKCE_OAUTH_REDIRECT_URI") ?? "http://localhost:8080/callback";

        var options = new OAuthOptions
        {
            ClientId = clientId,
            BaseUrl = baseUrl
        };

        // 1. Generate authorization URL
        var oauthClient = new PkceOAuthClient(options);
        var state = Guid.NewGuid().ToString();

        var authResponse = oauthClient.GenerateOAuthUrl(redirectUri, state, CodeChallengeMethod.S256);

        // 2. Store code_verifier securely
        var codeVerifier = authResponse.CodeVerifier;

        // 3. Open browser for user authorization
        Console.WriteLine("Step 1: Opening browser for authorization...");
        Console.WriteLine($"URL: {authResponse.AuthorizationUrl}");
        Console.WriteLine();
        Console.WriteLine($"IMPORTANT: Save this code verifier: {codeVerifier}");
        Console.WriteLine();

        // 4. In a real desktop app, you would:
        Console.WriteLine("In a real desktop/mobile application:");
        Console.WriteLine();
        Console.WriteLine("  For Desktop Apps:");
        Console.WriteLine("    - Start a local HTTP server on port 8080");
        Console.WriteLine("    - Open the authorization URL in the default browser");
        Console.WriteLine("    - Wait for callback with authorization code");
        Console.WriteLine("    - Exchange code + code_verifier for token");
        Console.WriteLine();
        Console.WriteLine("  For Mobile Apps:");
        Console.WriteLine("    - Register a custom URL scheme (e.g., myapp://)");
        Console.WriteLine("    - Open authorization URL in system browser");
        Console.WriteLine("    - Handle callback from custom URL scheme");
        Console.WriteLine("    - Exchange code + code_verifier for token");
        Console.WriteLine();

        // 5. Example code after receiving callback
        Console.WriteLine("After receiving callback with authorization code:");
        Console.WriteLine("```csharp");
        Console.WriteLine("// Using OAuthCozeClientFactory (Recommended)");
        Console.WriteLine("var (client, token) = await OAuthCozeClientFactory.CreateFromPkceOAuthAsync(");
        Console.WriteLine("    options,");
        Console.WriteLine("    receivedCode,");
        Console.WriteLine($"    \"{redirectUri}\",");
        Console.WriteLine($"    \"{codeVerifier}\");");
        Console.WriteLine();
        Console.WriteLine("// Use client for API calls");
        Console.WriteLine("var chat = await client.Chat.CreateAsync(new ChatRequest");
        Console.WriteLine("{");
        Console.WriteLine("    BotId = \"your-bot-id\",");
        Console.WriteLine("    UserId = \"user-123\",");
        Console.WriteLine("    Messages = new[] { Message.BuildUserQuestionText(\"Hello!\") }");
        Console.WriteLine("});");
        Console.WriteLine("```");
        Console.WriteLine();

        // 6. Refresh token example
        Console.WriteLine("To refresh an expired token:");
        Console.WriteLine("```csharp");
        Console.WriteLine("var (newClient, newToken) = await OAuthCozeClientFactory.CreateFromRefreshTokenAsync(");
        Console.WriteLine("    options,");
        Console.WriteLine("    refreshToken,");
        Console.WriteLine("    OAuthClientType.PKCE);");
        Console.WriteLine("```");
        Console.WriteLine();
    }

    private static void PrintSetupInstructions()
    {
        Console.WriteLine("To create a PKCE OAuth App:");
        Console.WriteLine("  1. Visit: https://www.coze.cn/open/oauth/apps");
        Console.WriteLine("  2. Create an OAuth App of type 'Single-page application' or 'Native/Mobile'");
        Console.WriteLine("  3. Get the client ID (no client secret for public clients)");
        Console.WriteLine("  4. Configure redirect URI (custom scheme or localhost)");
        Console.WriteLine();
        Console.WriteLine("Security Notes:");
        Console.WriteLine("  - PKCE protects against authorization code interception");
        Console.WriteLine("  - Use S256 (SHA-256) code challenge method for better security");
        Console.WriteLine("  - Store code_verifier securely during the auth flow");
        Console.WriteLine();
        Console.WriteLine("Required OAuth Scopes:");
        Console.WriteLine("  - bot:offline_access - Access to bot resources");
        Console.WriteLine("  - conversation:offline_access - Access to conversation resources");
        Console.WriteLine();
    }

    private static void PrintPkceFlowSteps(PkceAuthUrlResponse response, string redirectUri, string baseUrl)
    {
        Console.WriteLine("PKCE OAuth Flow Steps:");
        Console.WriteLine("  1. Store code_verifier securely (it will be needed later)");
        Console.WriteLine("  2. Open the authorization URL in browser/webview");
        Console.WriteLine("  3. User authorizes the application");
        Console.WriteLine("  4. Receive callback with authorization code");
        Console.WriteLine("  5. Exchange code + code_verifier for token:");
        Console.WriteLine();
        Console.WriteLine("```csharp");
        Console.WriteLine("// Using OAuthCozeClientFactory (Recommended)");
        Console.WriteLine("var options = new OAuthOptions");
        Console.WriteLine("{");
        Console.WriteLine("    ClientId = \"your-client-id\",");
        Console.WriteLine($"    BaseUrl = \"{baseUrl}\"");
        Console.WriteLine("};");
        Console.WriteLine();
        Console.WriteLine("var (client, token) = await OAuthCozeClientFactory.CreateFromPkceOAuthAsync(");
        Console.WriteLine("    options,");
        Console.WriteLine("    code,");
        Console.WriteLine($"    \"{redirectUri}\",");
        Console.WriteLine($"    \"{response.CodeVerifier}\");");
        Console.WriteLine();
        Console.WriteLine("// Or manually:");
        Console.WriteLine("var oauthClient = new PkceOAuthClient(options);");
        Console.WriteLine("var token = await oauthClient.GetAccessTokenAsync(code, redirectUri, codeVerifier);");
        Console.WriteLine("var client = token.CreateClient();");
        Console.WriteLine("```");
        Console.WriteLine();
    }

    private static void PrintClientUsageExamples(CozeClient client)
    {
        Console.WriteLine("You can now use the client:");
        Console.WriteLine();
        Console.WriteLine("```csharp");
        Console.WriteLine("// Chat example");
        Console.WriteLine("var chatRequest = new ChatRequest");
        Console.WriteLine("{");
        Console.WriteLine("    BotId = \"your-bot-id\",");
        Console.WriteLine("    UserId = \"user-123\",");
        Console.WriteLine("    Messages = new[] { Message.BuildUserQuestionText(\"Hello!\") }");
        Console.WriteLine("};");
        Console.WriteLine("var chat = await client.Chat.CreateAsync(chatRequest);");
        Console.WriteLine();
        Console.WriteLine("// Streaming chat");
        Console.WriteLine("await foreach (var evt in client.Chat.StreamAsync(chatRequest))");
        Console.WriteLine("{");
        Console.WriteLine("    if (evt.EventType == ChatEventType.ConversationMessageDelta)");
        Console.WriteLine("    {");
        Console.WriteLine("        Console.Write(evt.Message?.Content);");
        Console.WriteLine("    }");
        Console.WriteLine("}");
        Console.WriteLine();
        Console.WriteLine("// Bot operations");
        Console.WriteLine("var bots = await client.Bots.ListAsync(new ListBotsRequest { SpaceId = \"space-id\" });");
        Console.WriteLine();
        Console.WriteLine("// Workflow operations");
        Console.WriteLine("var result = await client.Workflows.RunAsync(new WorkflowRequest { WorkflowId = \"workflow-id\" });");
        Console.WriteLine("```");
        Console.WriteLine();
    }
}
