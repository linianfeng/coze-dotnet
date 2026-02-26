using Coze.Sdk.Http;
using Coze.Sdk.Models.Conversations;
using Coze.Sdk.Utils;

namespace Coze.Sdk.Clients;

/// <summary>
/// 会话消息操作的实现。
/// 对应 Java SDK 中的 ConversationMessageAPI.java。
/// </summary>
internal class ConversationMessageClient : IConversationMessageClient
{
    private readonly CozeHttpClient _httpClient;

    public ConversationMessageClient(CozeHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ConversationMessageDetail> CreateAsync(
        CreateConversationMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest("/v1/conversation/message/create", HttpMethodType.Post);
        httpRequest.AddQueryParameter("conversation_id", request.ConversationId);

        var body = new
        {
            role = request.Role,
            content = request.Content,
            content_type = request.ContentType,
            meta_data = request.MetaData
        };
        _httpClient.AddJsonBody(httpRequest, body);

        return await _httpClient.ExecuteAsync<ConversationMessageDetail>(httpRequest, cancellationToken);
    }

    public async Task<ListConversationMessagesResponse> ListAsync(
        ListConversationMessagesRequest request,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest("/v1/conversation/message/list", HttpMethodType.Post);
        httpRequest.AddQueryParameter("conversation_id", request.ConversationId);

        var body = new
        {
            order = request.Order,
            chat_id = request.ChatId,
            before_id = request.BeforeId,
            after_id = request.AfterId,
            limit = request.Limit,
            bot_id = request.BotId
        };
        _httpClient.AddJsonBody(httpRequest, body);

        return await _httpClient.ExecuteAsync<ListConversationMessagesResponse>(httpRequest, cancellationToken);
    }

    public async Task<ConversationMessageDetail> RetrieveAsync(
        RetrieveConversationMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest("/v1/conversation/message/retrieve", HttpMethodType.Post);
        httpRequest.AddQueryParameter("conversation_id", request.ConversationId);
        httpRequest.AddQueryParameter("message_id", request.MessageId);

        return await _httpClient.ExecuteAsync<ConversationMessageDetail>(httpRequest, cancellationToken);
    }

    public async Task<ConversationMessageDetail> UpdateAsync(
        UpdateConversationMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest("/v1/conversation/message/modify", HttpMethodType.Post);
        httpRequest.AddQueryParameter("conversation_id", request.ConversationId);
        httpRequest.AddQueryParameter("message_id", request.MessageId);

        var body = new
        {
            content = request.Content,
            content_type = request.ContentType,
            meta_data = request.MetaData
        };
        _httpClient.AddJsonBody(httpRequest, body);

        return await _httpClient.ExecuteAsync<ConversationMessageDetail>(httpRequest, cancellationToken);
    }

    public async Task DeleteAsync(
        DeleteConversationMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest("/v1/conversation/message/delete", HttpMethodType.Post);
        httpRequest.AddQueryParameter("conversation_id", request.ConversationId);
        httpRequest.AddQueryParameter("message_id", request.MessageId);

        await _httpClient.ExecuteAsync<object>(httpRequest, cancellationToken);
    }
}
