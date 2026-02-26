using Coze.Sdk.Models.Connectors;

namespace Coze.Sdk.Clients;

/// <summary>
/// 连接器操作接口。
/// 对应 Java SDK 中的 ConnectorAPI.java。
/// </summary>
public interface IConnectorClient
{
    /// <summary>
    /// 安装连接器。
    /// </summary>
    /// <param name="connectorId">要安装的连接器 ID。</param>
    /// <param name="request">安装请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>安装响应。</returns>
    Task<InstallConnectorResponse> InstallAsync(
        string connectorId,
        InstallConnectorRequest request,
        CancellationToken cancellationToken = default);
}
