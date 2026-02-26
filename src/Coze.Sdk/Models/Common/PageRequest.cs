using Newtonsoft.Json;

namespace Coze.Sdk.Models.Common;

/// <summary>
/// 分页请求的基类。
/// </summary>
public record PageRequest
{
    /// <summary>
    /// 获取或设置页码（从 1 开始）。
    /// </summary>
    [JsonProperty("page_num")]
    public int? PageNumber { get; init; } = 1;

    /// <summary>
    /// 获取或设置每页大小。
    /// </summary>
    [JsonProperty("page_size")]
    public int? PageSize { get; init; } = 20;
}
