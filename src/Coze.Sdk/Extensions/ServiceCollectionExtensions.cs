using Coze.Sdk.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Coze.Sdk.Extensions;

/// <summary>
/// 用于在依赖注入中配置 Coze SDK 的扩展方法。
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 将 Coze SDK 添加到服务集合中。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">配置选项的操作。</param>
    /// <returns>服务集合。</returns>
    public static IServiceCollection AddCozeSdk(
        this IServiceCollection services,
        Action<CozeOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<ICozeClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<CozeOptions>>().Value;
            var loggerFactory = sp.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();
            return new CozeClient(options with { LoggerFactory = loggerFactory });
        });
        return services;
    }

    /// <summary>
    /// 使用配置将 Coze SDK 添加到服务集合中。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configuration">配置节。</param>
    /// <param name="sectionName">配置节名称（默认值："Coze"）。</param>
    /// <returns>服务集合。</returns>
    public static IServiceCollection AddCozeSdk(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "Coze")
    {
        var section = configuration.GetSection(sectionName);
        services.Configure<CozeOptions>(section);

        services.AddSingleton<ICozeClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<CozeOptions>>().Value;
            var loggerFactory = sp.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();

            // 如果未提供认证，从配置创建认证
            var auth = options.Auth;
            if (auth == null)
            {
                var token = section["Token"] ?? section["AccessToken"];
                if (!string.IsNullOrEmpty(token))
                {
                    auth = new TokenAuth(token);
                }
            }

            return new CozeClient(options with
            {
                LoggerFactory = loggerFactory,
                Auth = auth ?? throw new InvalidOperationException("Coze authentication not configured")
            });
        });

        return services;
    }

    /// <summary>
    /// 使用 JWT OAuth 认证添加 Coze SDK（适用于服务账户）。
    /// SDK 将使用 JWT 断言自动获取和刷新令牌。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">配置 OAuth 选项的操作。</param>
    /// <returns>服务集合。</returns>
    /// <remarks>
    /// 必需的配置：
    /// - ClientId：OAuth 客户端 ID（会话名称）
    /// - ClientSecret：PEM 格式的 RSA 私钥
    /// - BaseUrl：可选的 API 基础 URL（默认值：https://api.coze.cn）
    /// </remarks>
    public static IServiceCollection AddCozeSdkWithJwtOAuth(
        this IServiceCollection services,
        Action<OAuthOptions> configure)
    {
        var options = new OAuthOptions
        {
            ClientId = string.Empty,
            BaseUrl = "https://api.coze.cn"
        };
        configure(options);

        services.AddSingleton(new JwtOAuthClient(options));
        services.AddSingleton<ICozeClient>(sp =>
        {
            var jwtClient = sp.GetRequiredService<JwtOAuthClient>();
            var loggerFactory = sp.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();

            return new CozeClient(new CozeOptions
            {
                Auth = jwtClient,
                BaseUrl = options.BaseUrl,
                LoggerFactory = loggerFactory
            });
        });

        return services;
    }

    /// <summary>
    /// 使用配置中的 JWT OAuth 认证添加 Coze SDK。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configuration">配置节。</param>
    /// <param name="sectionName">配置节名称（默认值："Coze:Jwt"）。</param>
    /// <returns>服务集合。</returns>
    public static IServiceCollection AddCozeSdkWithJwtOAuth(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "Coze:Jwt")
    {
        var section = configuration.GetSection(sectionName);

        var options = new OAuthOptions
        {
            ClientId = section["ClientId"] ?? throw new InvalidOperationException("ClientId is required"),
            ClientSecret = section["PrivateKey"] ?? section["ClientSecret"]
                ?? throw new InvalidOperationException("PrivateKey is required"),
            BaseUrl = section["BaseUrl"] ?? "https://api.coze.cn",
            Timeout = TimeSpan.TryParse(section["Timeout"], out var timeout) ? timeout : TimeSpan.FromSeconds(50)
        };

        return services.AddCozeSdkWithJwtOAuth(o =>
        {
            o.ClientId = options.ClientId;
            o.ClientSecret = options.ClientSecret;
            o.BaseUrl = options.BaseUrl;
            o.Timeout = options.Timeout;
        });
    }

    /// <summary>
    /// 使用 Web OAuth 认证添加 Coze SDK（自动令牌刷新）。
    /// SDK 将在令牌过期时自动刷新令牌。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">配置 OAuth 选项的操作。</param>
    /// <param name="refreshToken">用于获取访问令牌的刷新令牌。</param>
    /// <returns>服务集合。</returns>
    public static IServiceCollection AddCozeSdkWithWebOAuth(
        this IServiceCollection services,
        Action<OAuthOptions> configure,
        string refreshToken)
    {
        var options = new OAuthOptions
        {
            ClientId = string.Empty,
            BaseUrl = "https://api.coze.cn"
        };
        configure(options);

        services.AddSingleton(new WebOAuthClient(options));
        services.AddSingleton(sp =>
        {
            var oauthClient = sp.GetRequiredService<WebOAuthClient>();
            return new RefreshableTokenAuth(
                async ct =>
                {
                    var token = await oauthClient.RefreshTokenAsync(refreshToken, ct);
                    return (token.AccessToken!, token.ExpiresIn ?? 3600);
                });
        });
        services.AddSingleton<ICozeClient>(sp =>
        {
            var auth = sp.GetRequiredService<RefreshableTokenAuth>();
            var loggerFactory = sp.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();

            return new CozeClient(new CozeOptions
            {
                Auth = auth,
                BaseUrl = options.BaseUrl,
                LoggerFactory = loggerFactory
            });
        });

        return services;
    }

    /// <summary>
    /// 使用配置中的 Web OAuth 认证添加 Coze SDK。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configuration">配置节。</param>
    /// <param name="sectionName">配置节名称（默认值："Coze:OAuth"）。</param>
    /// <returns>服务集合。</returns>
    public static IServiceCollection AddCozeSdkWithWebOAuth(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "Coze:OAuth")
    {
        var section = configuration.GetSection(sectionName);

        var options = new OAuthOptions
        {
            ClientId = section["ClientId"] ?? throw new InvalidOperationException("ClientId is required"),
            ClientSecret = section["ClientSecret"]
                ?? throw new InvalidOperationException("ClientSecret is required"),
            BaseUrl = section["BaseUrl"] ?? "https://api.coze.cn",
            Timeout = TimeSpan.TryParse(section["Timeout"], out var timeout) ? timeout : TimeSpan.FromSeconds(50)
        };

        var refreshToken = section["RefreshToken"]
            ?? throw new InvalidOperationException("RefreshToken is required");

        return services.AddCozeSdkWithWebOAuth(o =>
        {
            o.ClientId = options.ClientId;
            o.ClientSecret = options.ClientSecret;
            o.BaseUrl = options.BaseUrl;
            o.Timeout = options.Timeout;
        }, refreshToken);
    }

    /// <summary>
    /// 使用 PKCE OAuth 认证添加 Coze SDK（自动令牌刷新）。
    /// 适用于使用 PKCE 流程的移动端/原生应用。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="configure">配置 OAuth 选项的操作。</param>
    /// <param name="refreshToken">用于获取访问令牌的刷新令牌。</param>
    /// <returns>服务集合。</returns>
    public static IServiceCollection AddCozeSdkWithPkceOAuth(
        this IServiceCollection services,
        Action<OAuthOptions> configure,
        string refreshToken)
    {
        var options = new OAuthOptions
        {
            ClientId = string.Empty,
            BaseUrl = "https://api.coze.cn"
        };
        configure(options);

        services.AddSingleton(new PkceOAuthClient(options));
        services.AddSingleton(sp =>
        {
            var oauthClient = sp.GetRequiredService<PkceOAuthClient>();
            return new RefreshableTokenAuth(
                async ct =>
                {
                    var token = await oauthClient.RefreshTokenAsync(refreshToken, ct);
                    return (token.AccessToken!, token.ExpiresIn ?? 3600);
                });
        });
        services.AddSingleton<ICozeClient>(sp =>
        {
            var auth = sp.GetRequiredService<RefreshableTokenAuth>();
            var loggerFactory = sp.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();

            return new CozeClient(new CozeOptions
            {
                Auth = auth,
                BaseUrl = options.BaseUrl,
                LoggerFactory = loggerFactory
            });
        });

        return services;
    }

    /// <summary>
    /// 使用令牌工厂函数添加 Coze SDK。
    /// 适用于令牌在外部管理的场景（例如数据库、分布式缓存）。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="tokenFactory">返回当前有效令牌的工厂函数。</param>
    /// <param name="configure">可选的配置额外选项的操作。</param>
    /// <returns>服务集合。</returns>
    public static IServiceCollection AddCozeSdkWithTokenFactory(
        this IServiceCollection services,
        Func<IServiceProvider, CancellationToken, Task<string>> tokenFactory,
        Action<CozeOptions>? configure = null)
    {
        services.AddSingleton<ICozeClient>(sp =>
        {
            var loggerFactory = sp.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();
            var auth = new DelegatingAuth(async ct => await tokenFactory(sp, ct));

            var options = new CozeOptions
            {
                Auth = auth,
                BaseUrl = "https://api.coze.cn",
                LoggerFactory = loggerFactory
            };
            configure?.Invoke(options);

            return new CozeClient(options);
        });

        return services;
    }
}

/// <summary>
/// 将令牌获取委托给函数的简单认证实现。
/// </summary>
internal class DelegatingAuth : Auth
{
    private readonly Func<CancellationToken, Task<string>> _getTokenFunc;

    public DelegatingAuth(Func<CancellationToken, Task<string>> getTokenFunc)
    {
        _getTokenFunc = getTokenFunc;
    }

    public override string GetToken()
    {
        return _getTokenFunc(CancellationToken.None).GetAwaiter().GetResult();
    }
}
