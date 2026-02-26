using Coze.Sdk.Http;
using Coze.Sdk.Models.Bots;
using Coze.Sdk.Models.Common;

namespace Coze.Sdk.Clients;

/// <summary>
/// 机器人操作的实现。
/// 对应 Java SDK 中的 BotService.java。
/// </summary>
internal class BotsClient : IBotsClient
{
    private readonly CozeHttpClient _httpClient;

    public BotsClient(CozeHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PagedResponse<SimpleBot>> ListAsync(
        ListBotsRequest request,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateGetRequest(ApiEndpoints.BotList);
        httpRequest.AddQueryParameter("space_id", request.SpaceId);
        httpRequest.AddQueryParameter("page_num", request.PageNumber ?? 1);
        httpRequest.AddQueryParameter("page_size", request.PageSize ?? 20);

        var response = await _httpClient.ExecuteAsync<ListBotsResponse>(httpRequest, cancellationToken);

        var items = response.Bots ?? new List<SimpleBot>();
        var hasMore = items.Count >= (request.PageSize ?? 20);

        return new PagedResponse<SimpleBot>
        {
            Total = response.Total,
            Items = items,
            HasMore = hasMore
        };
    }

    public async Task<Bot> RetrieveAsync(string botId, CancellationToken cancellationToken = default)
    {
        var request = _httpClient.CreateGetRequest(ApiEndpoints.BotRetrieve);
        request.AddQueryParameter("bot_id", botId);

        return await _httpClient.ExecuteAsync<Bot>(request, cancellationToken);
    }

    public async Task<CreateBotResponse> CreateAsync(
        CreateBotRequest request,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest(ApiEndpoints.BotCreate, HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, request);

        return await _httpClient.ExecuteAsync<CreateBotResponse>(httpRequest, cancellationToken);
    }

    public async Task<UpdateBotResponse> UpdateAsync(
        UpdateBotRequest request,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest(ApiEndpoints.BotUpdate, HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, request);

        await _httpClient.ExecuteAsync<object>(httpRequest, cancellationToken);

        return new UpdateBotResponse();
    }

    public async Task<PublishBotResponse> PublishAsync(
        PublishBotRequest request,
        CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest(ApiEndpoints.BotPublish, HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, request);

        return await _httpClient.ExecuteAsync<PublishBotResponse>(httpRequest, cancellationToken);
    }
}
