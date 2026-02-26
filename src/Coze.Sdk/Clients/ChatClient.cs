using System.Runtime.CompilerServices;
using Coze.Sdk.Http;
using Coze.Sdk.Models.Chat;

namespace Coze.Sdk.Clients;

/// <summary>
/// 聊天操作的实现。
/// 对应 Java SDK 中的 ChatService.java。
/// </summary>
internal class ChatClient : IChatClient
{
    private readonly CozeHttpClient _httpClient;
    private readonly ChatMessageClient _messageClient;

    public ChatClient(CozeHttpClient httpClient)
    {
        _httpClient = httpClient;
        _messageClient = new ChatMessageClient(httpClient);
    }

    public IChatMessageClient Messages => _messageClient;

    public async Task<ChatResponse> CreateAsync(ChatRequest request, CancellationToken cancellationToken = default)
    {
        var nonStreamingRequest = request.WithoutStream();
        var conversationId = nonStreamingRequest.ConversationId;

        var httpRequest = _httpClient.CreateRequest(ApiEndpoints.Chat, HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, nonStreamingRequest);

        // 如果提供了 conversation_id，则添加为查询参数
        if (!string.IsNullOrEmpty(conversationId))
        {
            httpRequest.AddQueryParameter("conversation_id", conversationId);
        }

        var chat = await _httpClient.ExecuteAsync<Chat>(httpRequest, cancellationToken);

        return new ChatResponse
        {
            Chat = chat,
            LogId = null // LogId 将从响应头中提取
        };
    }

    public async IAsyncEnumerable<ChatEvent> StreamAsync(
        ChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var streamingRequest = request.WithStream();
        var conversationId = streamingRequest.ConversationId;

        var httpRequest = _httpClient.CreateRequest(ApiEndpoints.Chat, HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, streamingRequest);

        // 如果提供了 conversation_id，则添加为查询参数
        if (!string.IsNullOrEmpty(conversationId))
        {
            httpRequest.AddQueryParameter("conversation_id", conversationId);
        }

        var (stream, logId) = await _httpClient.ExecuteStreamWithLogIdAsync(httpRequest, cancellationToken);

        await foreach (var (eventType, data) in SseReader.ReadChatEventsAsync(stream, cancellationToken))
        {
            yield return ChatEvent.ParseEvent(eventType, data, logId);
        }
    }

    public async Task<ChatPollResult> CreateAndPollAsync(
        ChatRequest request,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        var nonStreamingRequest = request.WithoutStream();
        var conversationId = nonStreamingRequest.ConversationId;

        var httpRequest = _httpClient.CreateRequest(ApiEndpoints.Chat, HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, nonStreamingRequest);

        if (!string.IsNullOrEmpty(conversationId))
        {
            httpRequest.AddQueryParameter("conversation_id", conversationId);
        }

        var chat = await _httpClient.ExecuteAsync<Chat>(httpRequest, cancellationToken);

        // 从响应中更新 conversation ID
        conversationId = chat.ConversationId;
        var chatId = chat.Id;

        var startTime = DateTime.UtcNow;

        // 轮询直到完成
        while (chat.Status == ChatStatus.InProgress)
        {
            await Task.Delay(1000, cancellationToken);

            if (timeout.HasValue && (DateTime.UtcNow - startTime) > timeout)
            {
                // 超时时取消
                if (!string.IsNullOrEmpty(conversationId) && !string.IsNullOrEmpty(chatId))
                {
                    await CancelAsync(new CancelChatRequest
                    {
                        ConversationId = conversationId,
                        ChatId = chatId
                    }, cancellationToken);
                }
                break;
            }

            chat = await RetrieveAsync(conversationId!, chatId!, cancellationToken);

            if (chat.Status == ChatStatus.Completed)
            {
                break;
            }
        }

        // 获取消息
        ListMessagesResponse? messages = null;
        if (!string.IsNullOrEmpty(conversationId) && !string.IsNullOrEmpty(chatId))
        {
            messages = await _messageClient.ListAsync(new ListMessagesRequest
            {
                ConversationId = conversationId,
                ChatId = chatId
            }, cancellationToken);
        }

        return new ChatPollResult
        {
            Chat = chat,
            Messages = messages?.Messages
        };
    }

    public async Task<Chat> RetrieveAsync(
        string conversationId,
        string chatId,
        CancellationToken cancellationToken = default)
    {
        var request = _httpClient.CreateGetRequest(ApiEndpoints.ChatRetrieve);
        request.AddQueryParameter("conversation_id", conversationId);
        request.AddQueryParameter("chat_id", chatId);

        return await _httpClient.ExecuteAsync<Chat>(request, cancellationToken);
    }

    public async Task<Chat> CancelAsync(CancelChatRequest request, CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest(ApiEndpoints.ChatCancel, HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, request);

        return await _httpClient.ExecuteAsync<Chat>(httpRequest, cancellationToken);
    }
}

/// <summary>
/// 聊天消息操作的实现。
/// </summary>
internal class ChatMessageClient : IChatMessageClient
{
    private readonly CozeHttpClient _httpClient;

    public ChatMessageClient(CozeHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ListMessagesResponse> ListAsync(
        ListMessagesRequest request,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateGetRequest(ApiEndpoints.MessagesList);
        httpRequest.AddQueryParameter("conversation_id", request.ConversationId);
        httpRequest.AddQueryParameter("chat_id", request.ChatId);

        var messages = await _httpClient.ExecuteAsync<List<Message>>(httpRequest, cancellationToken);

        return new ListMessagesResponse
        {
            Messages = messages
        };
    }
}
