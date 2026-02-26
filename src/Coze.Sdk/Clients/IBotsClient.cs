using Coze.Sdk.Models.Bots;
using Coze.Sdk.Models.Common;

namespace Coze.Sdk.Clients;

/// <summary>
/// 机器人操作接口。
/// 对应 Java SDK 中的 BotService.java。
/// </summary>
public interface IBotsClient
{
    /// <summary>
    /// 列出空间中的机器人。
    /// </summary>
    /// <param name="request">列表请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>机器人的分页响应。</returns>
    Task<PagedResponse<SimpleBot>> ListAsync(ListBotsRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据 ID 获取机器人。
    /// </summary>
    /// <param name="botId">机器人 ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>机器人对象。</returns>
    Task<Bot> RetrieveAsync(string botId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建新机器人。
    /// </summary>
    /// <param name="request">创建请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>包含机器人 ID 的创建响应。</returns>
    Task<CreateBotResponse> CreateAsync(CreateBotRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新机器人。
    /// </summary>
    /// <param name="request">更新请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>更新响应。</returns>
    Task<UpdateBotResponse> UpdateAsync(UpdateBotRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发布机器人。
    /// </summary>
    /// <param name="request">发布请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>发布响应。</returns>
    Task<PublishBotResponse> PublishAsync(PublishBotRequest request, CancellationToken cancellationToken = default);
}
