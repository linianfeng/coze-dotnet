using Coze.Sdk;
using Coze.Sdk.Authentication;
using CozeExample.Config;

namespace CozeExample.Auth;

/// <summary>
/// JWT OAuth 认证示例，用于服务账户场景。
/// 从 coze_oauth_config.json 或环境变量读取配置。
/// </summary>
public static class JwtOAuthExample
{
    public static async Task RunAsync()
    {
        // 加载配置
        var configuration = ConfigHelper.LoadConfiguration();

        // 从配置文件读取 JWT OAuth 相关配置
        var clientType = configuration["client_type"];
        var clientId = configuration["client_id"];
        var privateKey = configuration["private_key"];
        var cozeApiBase = configuration["coze_api_base"];
        var publicKeyId = configuration["public_key_id"];

        // 环境变量优先
        var envClientId = Environment.GetEnvironmentVariable("COZE_JWT_OAUTH_CLIENT_ID");
        var envPrivateKey = Environment.GetEnvironmentVariable("COZE_JWT_OAUTH_PRIVATE_KEY");
        var envApiBase = Environment.GetEnvironmentVariable("COZE_API_BASE");
        var envPublicKeyId = Environment.GetEnvironmentVariable("COZE_JWT_OAUTH_PUBLIC_KEY_ID");

        if (!string.IsNullOrEmpty(envClientId)) clientId = envClientId;
        if (!string.IsNullOrEmpty(envPrivateKey)) privateKey = envPrivateKey;
        if (!string.IsNullOrEmpty(envApiBase)) cozeApiBase = envApiBase;
        if (!string.IsNullOrEmpty(envPublicKeyId)) publicKeyId = envPublicKeyId;

        // 检查 JWT OAuth 是否已配置
        if (clientType != "jwt" ||
            string.IsNullOrEmpty(clientId) ||
            string.IsNullOrEmpty(privateKey) ||
            string.IsNullOrEmpty(publicKeyId) ||
            privateKey.Contains("YOUR_PRIVATE_KEY"))
        {
            Console.WriteLine("JWT OAuth 未配置。跳过。");
            Console.WriteLine();
            Console.WriteLine("要启用 JWT OAuth，请配置 coze_oauth_config.json:");
            Console.WriteLine(@"{
  ""client_type"": ""jwt"",
  ""client_id"": ""your-client-id"",
  ""private_key"": ""-----BEGIN PRIVATE KEY-----
your-private-key-here
-----END PRIVATE KEY-----"",
  ""public_key_id"": ""your-public-key-id""
}");
            Console.WriteLine();
            Console.WriteLine("或设置环境变量:");
            Console.WriteLine("  - COZE_JWT_OAUTH_CLIENT_ID");
            Console.WriteLine("  - COZE_JWT_OAUTH_PRIVATE_KEY");
            Console.WriteLine("  - COZE_JWT_OAUTH_PUBLIC_KEY_ID");
            return;
        }

        try
        {
            // 创建 JWT OAuth 客户端
            var oauthOptions = new OAuthOptions
            {
                ClientId = clientId,
                ClientSecret = privateKey,
                BaseUrl = cozeApiBase ?? "https://api.coze.cn",
                PublicKeyId = publicKeyId
            };

            var jwtClient = new JwtOAuthClient(oauthOptions);

            // 获取访问令牌
            var token = await jwtClient.GetAccessTokenAsync();

            if (token != null && !string.IsNullOrEmpty(token.AccessToken))
            {
                Console.WriteLine($"JWT OAuth: 成功获取访问令牌（有效期 {token.ExpiresIn} 秒）");

                // 使用令牌创建 CozeClient
                using var client = new CozeClient(new Coze.Sdk.CozeOptions
                {
                    Auth = new TokenAuth(token.AccessToken),
                    BaseUrl = cozeApiBase ?? "https://api.coze.cn"
                });

                // 通过列出工作空间测试客户端
                var workspaces = await client.Workspaces.ListAsync(new Coze.Sdk.Models.Workspaces.ListWorkspacesRequest
                {
                    PageNumber = 1,
                    PageSize = 5
                });

                Console.WriteLine($"JWT OAuth: 成功认证，找到 {workspaces.Total} 个工作空间");
            }
            else
            {
                Console.WriteLine("JWT OAuth: 获取访问令牌失败");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"JWT OAuth 错误: {ex.Message}");
        }
    }
}
