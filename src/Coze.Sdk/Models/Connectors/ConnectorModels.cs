using Newtonsoft.Json;

namespace Coze.Sdk.Models.Connectors;

/// <summary>
/// 安装连接器的请求。
/// 对应 Java SDK 中的 InstallConnectorReq.java。
/// </summary>
public record InstallConnectorRequest
{
    /// <summary>
    /// 获取工作空间 ID。
    /// </summary>
    [JsonProperty("workspace_id")]
    public required string WorkspaceId { get; init; }
}

/// <summary>
/// 安装连接器的响应。
/// 对应 Java SDK 中的 InstallConnectorResp.java。
/// </summary>
public record InstallConnectorResponse
{
    // 空响应，继承自 BaseResp
}
