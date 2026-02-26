using Coze.Sdk.Models.Conversations;

namespace Coze.Sdk.Clients;

/// <summary>
/// 会话消息操作接口。
/// 对应 Java SDK 中的 ConversationMessageAPI.java。
/// </summary>
public interface IConversationMessageClient
{
    /// <summary>
    /// 在会话中创建新消息。
    /// </summary>
    /// <param name="request">创建请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>创建的消息。</returns>
    Task<ConversationMessageDetail> CreateAsync(
        CreateConversationMessageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 列出会话中的消息。
    /// </summary>
    /// <param name="request">列表请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>列表响应。</returns>
    Task<ListConversationMessagesResponse> ListAsync(
        ListConversationMessagesRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取会话中的特定消息。
    /// </summary>
    /// <param name="request">获取请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>消息对象。</returns>
    Task<ConversationMessageDetail> RetrieveAsync(
        RetrieveConversationMessageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新会话中的消息。
    /// </summary>
    /// <param name="request">更新请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>更新后的消息。</returns>
    Task<ConversationMessageDetail> UpdateAsync(
        UpdateConversationMessageRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除会话中的消息。
    /// </summary>
    /// <param name="request">删除请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    Task DeleteAsync(
        DeleteConversationMessageRequest request,
        CancellationToken cancellationToken = default);
}
