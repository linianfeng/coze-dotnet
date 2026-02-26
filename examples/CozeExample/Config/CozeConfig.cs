using Microsoft.Extensions.Configuration;
using Coze.Sdk.Authentication;

namespace CozeExample.Config;

/// <summary>
/// 示例应用程序的配置选项。
/// 包含运行示例所需的各种参数。
/// </summary>
public class CozeOptions
{
    /// <summary>
    /// API 访问令牌。
    /// </summary>
    public string? ApiToken { get; set; }

    /// <summary>
    /// API 基础 URL。
    /// </summary>
    public string? ApiBase { get; set; }

    /// <summary>
    /// Bot ID。
    /// </summary>
    public string? BotId { get; set; }

    /// <summary>
    /// 工作空间 ID。
    /// </summary>
    public string? WorkspaceId { get; set; }

    /// <summary>
    /// 工作流 ID。
    /// </summary>
    public string? WorkflowId { get; set; }

    /// <summary>
    /// 文件路径。
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// 语音 ID。
    /// </summary>
    public string? VoiceId { get; set; }
}

/// <summary>
/// 配置助手类。
/// </summary>
public static class ConfigHelper
{
    /// <summary>
    /// 加载配置。
    /// </summary>
    public static IConfiguration LoadConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("coze_oauth_config.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    /// <summary>
    /// 将配置绑定到对象，支持 snake_case 到 PascalCase 的映射。
    /// </summary>
    private static void BindOptions(IConfiguration configuration, CozeOptions options)
    {
        // 手动映射 snake_case 到 PascalCase
        options.ApiToken = configuration["api_token"];
        options.ApiBase = configuration["api_base"];
        options.BotId = configuration["bot_id"];
        options.WorkspaceId = configuration["workspace_id"];
        options.WorkflowId = configuration["workflow_id"];
        options.FilePath = configuration["file_path"];
        options.VoiceId = configuration["voice_id"];
    }

    /// <summary>
    /// 获取示例配置选项。
    /// </summary>
    public static CozeOptions GetCozeOptions(IConfiguration configuration)
    {
        var options = new CozeOptions();

        // 从 coze 配置节绑定，支持 snake_case
        var cozeSection = configuration.GetSection("coze");
        BindOptions(cozeSection, options);

        // 环境变量优先
        var envToken = Environment.GetEnvironmentVariable("COZE_API_TOKEN");
        var envBase = Environment.GetEnvironmentVariable("COZE_API_BASE");
        var envBotId = Environment.GetEnvironmentVariable("COZE_BOT_ID");
        var envWorkspaceId = Environment.GetEnvironmentVariable("WORKSPACE_ID");
        var envWorkflowId = Environment.GetEnvironmentVariable("WORKFLOW_ID");
        var envFilePath = Environment.GetEnvironmentVariable("FILE_PATH");
        var envVoiceId = Environment.GetEnvironmentVariable("COZE_VOICE_ID");

        if (!string.IsNullOrEmpty(envToken)) options.ApiToken = envToken;
        if (!string.IsNullOrEmpty(envBase)) options.ApiBase = envBase;
        if (!string.IsNullOrEmpty(envBotId)) options.BotId = envBotId;
        if (!string.IsNullOrEmpty(envWorkspaceId)) options.WorkspaceId = envWorkspaceId;
        if (!string.IsNullOrEmpty(envWorkflowId)) options.WorkflowId = envWorkflowId;
        if (!string.IsNullOrEmpty(envFilePath)) options.FilePath = envFilePath;
        if (!string.IsNullOrEmpty(envVoiceId)) options.VoiceId = envVoiceId;

        return options;
    }

    /// <summary>
    /// 获取 SDK 的 OAuth 配置选项。
    /// </summary>
    public static OAuthOptions GetOAuthOptions(IConfiguration configuration)
    {
        var options = new OAuthOptions();

        // 从配置文件读取
        var clientId = configuration["client_id"];
        var cozeApiBase = configuration["coze_api_base"];
        var cozeWwwBase = configuration["coze_www_base"];
        var privateKey = configuration["private_key"];
        var publicKeyId = configuration["public_key_id"];

        // 设置 OAuth 选项
        if (!string.IsNullOrEmpty(clientId))
        {
            options.ClientId = clientId;
        }
        if (!string.IsNullOrEmpty(cozeApiBase))
        {
            options.BaseUrl = cozeApiBase;
        }
        if (!string.IsNullOrEmpty(cozeWwwBase))
        {
            options.WwwUrl = cozeWwwBase;
        }
        if (!string.IsNullOrEmpty(privateKey))
        {
            options.ClientSecret = privateKey;
        }
        if (!string.IsNullOrEmpty(publicKeyId))
        {
            options.PublicKeyId = publicKeyId;
        }

        // 环境变量优先
        var envClientId = Environment.GetEnvironmentVariable("COZE_JWT_OAUTH_CLIENT_ID");
        var envPrivateKey = Environment.GetEnvironmentVariable("COZE_JWT_OAUTH_PRIVATE_KEY");
        var envApiBase = Environment.GetEnvironmentVariable("COZE_API_BASE");
        var envPublicKeyId = Environment.GetEnvironmentVariable("COZE_JWT_OAUTH_PUBLIC_KEY_ID");

        if (!string.IsNullOrEmpty(envClientId))
        {
            options.ClientId = envClientId;
        }
        if (!string.IsNullOrEmpty(envPrivateKey))
        {
            options.ClientSecret = envPrivateKey;
        }
        if (!string.IsNullOrEmpty(envApiBase))
        {
            options.BaseUrl = envApiBase;
        }
        if (!string.IsNullOrEmpty(envPublicKeyId))
        {
            options.PublicKeyId = envPublicKeyId;
        }

        return options;
    }
}
