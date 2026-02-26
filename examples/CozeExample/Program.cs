using Coze.Sdk;
using Coze.Sdk.Authentication;
using CozeExample.Auth;
using CozeExample.Audio;
using CozeExample.Bot;
using CozeExample.Chat;
using CozeExample.Commerce;
using CozeExample.Config;
using CozeExample.Conversation;
using CozeExample.Dataset;
using CozeExample.File;
using CozeExample.WebSocket;
using CozeExample.Workspace;
using CozeExample.Workflow;

// ============================================================================
// Coze .NET SDK Examples
// ============================================================================
// Configuration sources (in order of priority):
// 1. Environment variables
// 2. appsettings.json / coze_oauth_config.json
// ============================================================================

// 加载配置
var configuration = ConfigHelper.LoadConfiguration();
var options = ConfigHelper.GetCozeOptions(configuration);
var oauthOptions = ConfigHelper.GetOAuthOptions(configuration);
var clientType = configuration["client_type"];

// 创建客户端
CozeClient? client = null;
JwtOAuthClient? jwtOAuthClient = null;

// 优先使用 JWT OAuth 认证
if (clientType == "jwt" &&
    !string.IsNullOrEmpty(oauthOptions.ClientId) &&
    !string.IsNullOrEmpty(oauthOptions.ClientSecret) &&
    !string.IsNullOrEmpty(oauthOptions.PublicKeyId))
{
    try
    {
        Console.WriteLine("使用 JWT OAuth 认证...");

        jwtOAuthClient = new JwtOAuthClient(oauthOptions);

        // 使用 JwtOAuthClient 作为 Auth 提供者创建客户端
        client = new CozeClient(new Coze.Sdk.CozeOptions
        {
            Auth = jwtOAuthClient,
            BaseUrl = oauthOptions.BaseUrl ?? options.ApiBase ?? "https://api.coze.cn"
        });

        Console.WriteLine("JWT OAuth 认证配置成功");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"JWT OAuth 认证失败: {ex.Message}");
        Console.WriteLine("回退到 Token 认证...");
        client = null;
    }
}

// 如果 JWT OAuth 未配置或失败，使用 Token 认证
if (client == null)
{
    if (string.IsNullOrEmpty(options.ApiToken))
    {
        Console.WriteLine("错误: 请配置认证方式");
        Console.WriteLine();
        Console.WriteLine("方式一 - JWT OAuth: 配置 coze_oauth_config.json");
        Console.WriteLine(@"{
  ""client_type"": ""jwt"",
  ""client_id"": ""your-client-id"",
  ""private_key"": ""-----BEGIN PRIVATE KEY-----
your-private-key-here
-----END PRIVATE KEY-----"",
  ""public_key_id"": ""your-public-key-id"",
  ""coze_api_base"": ""https://api.coze.cn""
}");
        Console.WriteLine();
        Console.WriteLine("方式二 - Token 认证: 设置环境变量或在 appsettings.json 中配置");
        Console.WriteLine("  环境变量: set COZE_API_TOKEN=your_token");
        Console.WriteLine("  配置文件: 编辑 appsettings.json 中的 coze.api_token");
        return;
    }

    client = new CozeClient(new Coze.Sdk.CozeOptions
    {
        Auth = new TokenAuth(options.ApiToken),
        BaseUrl = options.ApiBase
    });

    Console.WriteLine("使用 Token 认证");
}

// ============================================================================
// Authentication Examples
// ============================================================================
Console.WriteLine("\n=== Authentication Examples ===");
TokenAuthExample.Run();

if (!string.IsNullOrEmpty(oauthOptions.ClientId))
{
    Console.WriteLine($"OAuth config loaded: ClientType={clientType}, ClientId={oauthOptions.ClientId}");
}

await DeviceOAuthExample.RunAsync();

// 如果没有使用 JWT OAuth 作为主认证，则运行 JWT OAuth 示例
if (jwtOAuthClient == null)
{
    await JwtOAuthExample.RunAsync();
}

//// ============================================================================
//// Chat Examples
//// ============================================================================
//if (!string.IsNullOrEmpty(options.BotId))
//{
//    Console.WriteLine("\n=== Chat Examples ===");
//    await StreamChatExample.RunAsync(client, options.BotId);
//    await ChatPollingExample.RunAsync(client, options.BotId);
//    await ChatWithConversationExample.RunAsync(client, options.BotId);
//}

//// ============================================================================
//// Bot Examples
//// ============================================================================
//Console.WriteLine("\n=== Bot Examples ===");
//await BotRetrieveExample.RunAsync(client, options.BotId, options.WorkspaceId);

// ============================================================================
// Workflow Examples
// ============================================================================
//if (!string.IsNullOrEmpty(options.WorkflowId))
//{
//    Console.WriteLine("\n=== Workflow Examples ===");
//    await WorkflowRunExample.RunAsync(client, options.WorkflowId, options.FilePath);
//    await WorkflowStreamExample.RunAsync(client, options.WorkflowId);
//}

//// ============================================================================
//// Conversation Examples
//// ============================================================================
//if (!string.IsNullOrEmpty(options.BotId))
//{
//    Console.WriteLine("\n=== Conversation Examples ===");
//    var conversationId = await ConversationExample.RunAsync(client, options.BotId);
//    if (!string.IsNullOrEmpty(conversationId))
//    {
//        await ConversationMessageExample.RunAsync(client, conversationId);
//    }
//}

// ============================================================================
// File Examples
// ============================================================================
//if (!string.IsNullOrEmpty(options.FilePath))
//{
//    Console.WriteLine("\n=== File Examples ===");
//    await FileExample.RunAsync(client, options.FilePath);
//}

// ============================================================================
// Dataset Examples
// ============================================================================
if (!string.IsNullOrEmpty(options.WorkspaceId))
{
    Console.WriteLine("\n=== Dataset Examples ===");
    await DatasetListExample.RunAsync(client, options.WorkspaceId);
    await DatasetCrudExample.RunAsync(client, options.WorkspaceId);
}

// ============================================================================
// Audio Examples
// ============================================================================
Console.WriteLine("\n=== Audio Examples ===");
await VoiceListExample.RunAsync(client);
await SpeechExample.RunAsync(client, options.VoiceId);

// ============================================================================
// Workspace Examples
// ============================================================================
Console.WriteLine("\n=== Workspace Examples ===");
await WorkspaceListExample.RunAsync(client);

// ============================================================================
// Commerce Examples
// ============================================================================
Console.WriteLine("\n=== Commerce Examples ===");
await BillTaskExample.RunAsync(client);
await BenefitLimitationExample.RunAsync(client);

// ============================================================================
// WebSocket Examples
// ============================================================================
if (!string.IsNullOrEmpty(options.BotId))
{
    Console.WriteLine("\n=== WebSocket Examples ===");
    await WebSocketChatExample.RunAsync(client, options.BotId);
}
await WebSocketSpeechExample.RunAsync(client);

// 释放资源
client.Dispose();
jwtOAuthClient?.Dispose();

Console.WriteLine("\n=== All Examples Completed ===");
