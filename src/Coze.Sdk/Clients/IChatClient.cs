using Coze.Sdk.Models.Chat;

namespace Coze.Sdk.Clients;

/// <summary>
/// 聊天操作接口。
/// 对应 Java SDK 中的 ChatService.java。
/// </summary>
public interface IChatClient
{
    /// <summary>
    /// 创建非流式响应的聊天。
    /// </summary>
    /// <param name="request">聊天请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>聊天响应。</returns>
    Task<ChatResponse> CreateAsync(ChatRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建流式响应的聊天。
    /// </summary>
    /// <param name="request">聊天请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>聊天事件的异步枚举。</returns>
    IAsyncEnumerable<ChatEvent> StreamAsync(ChatRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建聊天并轮询等待完成。
    /// </summary>
    /// <param name="request">聊天请求。</param>
    /// <param name="timeout">可选的轮询超时时间。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>包含聊天和消息的轮询结果。</returns>
    Task<ChatPollResult> CreateAndPollAsync(
        ChatRequest request,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取聊天。
    /// </summary>
    /// <param name="conversationId">会话 ID。</param>
    /// <param name="chatId">聊天 ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>聊天对象。</returns>
    Task<Chat> RetrieveAsync(
        string conversationId,
        string chatId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消聊天。
    /// </summary>
    /// <param name="request">取消请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>已取消的聊天。</returns>
    Task<Chat> CancelAsync(CancelChatRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取聊天消息操作客户端。
    /// </summary>
    IChatMessageClient Messages { get; }
}

/// <summary>
/// 聊天消息操作接口。
/// </summary>
public interface IChatMessageClient
{
    /// <summary>
    /// 列出聊天中的消息。
    /// </summary>
    /// <param name="request">列表请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns>消息列表。</returns>
    Task<ListMessagesResponse> ListAsync(ListMessagesRequest request, CancellationToken cancellationToken = default);
}
