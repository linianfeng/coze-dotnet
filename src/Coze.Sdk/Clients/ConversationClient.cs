using Coze.Sdk.Http;
using Coze.Sdk.Models.Conversations;

namespace Coze.Sdk.Clients;

/// <summary>
/// 会话操作的实现。
/// 对应 Java SDK 中的 ConversationService.java。
/// </summary>
internal class ConversationClient : IConversationClient
{
    private readonly CozeHttpClient _httpClient;
    private readonly ConversationMessageClient _messageClient;

    public ConversationClient(CozeHttpClient httpClient)
    {
        _httpClient = httpClient;
        _messageClient = new ConversationMessageClient(httpClient);
    }

    public IConversationMessageClient Messages => _messageClient;

    public async Task<CreateConversationResponse> CreateAsync(CreateConversationRequest request, CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest(ApiEndpoints.ConversationCreate, HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, request);

        var conversation = await _httpClient.ExecuteAsync<Conversation>(httpRequest, cancellationToken);

        return new CreateConversationResponse
        {
            Conversation = conversation
        };
    }

    public async Task<Conversation> RetrieveAsync(string conversationId, CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateGetRequest(ApiEndpoints.ConversationRetrieve);
        httpRequest.AddQueryParameter("conversation_id", conversationId);

        return await _httpClient.ExecuteAsync<Conversation>(httpRequest, cancellationToken);
    }

    public async Task<ListConversationsResponse> ListAsync(ListConversationsRequest request, CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateGetRequest(ApiEndpoints.ConversationList);
        httpRequest.AddQueryParameter("bot_id", request.BotId);
        httpRequest.AddQueryParameter("page_num", request.PageNumber?.ToString() ?? "1");
        httpRequest.AddQueryParameter("page_size", request.PageSize?.ToString() ?? "20");

        var conversations = await _httpClient.ExecuteAsync<ListConversationsResponse>(httpRequest, cancellationToken);
        return conversations ?? new ListConversationsResponse();
    }

    public async Task ClearAsync(string conversationId, CancellationToken cancellationToken = default)
    {
        var endpoint = $"/v1/conversations/{conversationId}/clear";
        var httpRequest = _httpClient.CreateRequest(endpoint, HttpMethodType.Post);

        await _httpClient.ExecuteAsync<object>(httpRequest, cancellationToken);
    }
}
