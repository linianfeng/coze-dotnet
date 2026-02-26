using Coze.Sdk.Models.Conversations;

namespace Coze.Sdk.Clients;

/// <summary>
/// 会话操作接口。
/// 对应 Java SDK 中的 ConversationService.java。
/// </summary>
public interface IConversationClient
{
    /// <summary>
    /// 创建新会话。
    /// </summary>
    Task<CreateConversationResponse> CreateAsync(CreateConversationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据 ID 获取会话。
    /// </summary>
    Task<Conversation> RetrieveAsync(string conversationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 列出会话。
    /// </summary>
    Task<ListConversationsResponse> ListAsync(ListConversationsRequest? request = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 清空会话。
    /// </summary>
    Task ClearAsync(string conversationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取会话消息操作客户端。
    /// </summary>
    IConversationMessageClient Messages { get; }
}
