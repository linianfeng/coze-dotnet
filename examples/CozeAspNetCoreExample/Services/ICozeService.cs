using Coze.Sdk;

namespace CozeAspNetCoreExample.Services;

public interface ICozeService
{
    Task<string> ChatWithBotAsync(string botId, string message, string? userId = null, CancellationToken cancellationToken = default);
}
