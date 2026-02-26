using Coze.Sdk.Http;
using Coze.Sdk.Models.Audio;

namespace Coze.Sdk.Clients;

/// <summary>
/// 音频操作的实现。
/// 对应 Java SDK 中的 AudioService.java。
/// </summary>
internal class AudioClient : IAudioClient
{
    private readonly CozeHttpClient _httpClient;

    public AudioClient(CozeHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CreateSpeechResponse> CreateSpeechAsync(CreateSpeechRequest request, CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest(ApiEndpoints.AudioSpeech, HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, request);

        var (stream, _) = await _httpClient.ExecuteStreamWithLogIdAsync(httpRequest, cancellationToken);

        return new CreateSpeechResponse
        {
            AudioStream = stream,
            ContentType = GetContentType(request.ResponseFormat)
        };
    }

    public async Task<CreateTranscriptionResponse> CreateTranscriptionAsync(CreateTranscriptionRequest request, CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest(ApiEndpoints.AudioTranscription, HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, request);

        return await _httpClient.ExecuteAsync<CreateTranscriptionResponse>(httpRequest, cancellationToken);
    }

    public async Task<ListVoicesResponse> ListVoicesAsync(ListVoicesRequest? request = null, CancellationToken cancellationToken = default)
    {
        request ??= new ListVoicesRequest();

        var httpRequest = _httpClient.CreateGetRequest(ApiEndpoints.AudioVoices);

        if (request.PageNumber.HasValue)
        {
            httpRequest.AddQueryParameter("page_num", request.PageNumber.Value.ToString());
        }

        if (request.PageSize.HasValue)
        {
            httpRequest.AddQueryParameter("page_size", request.PageSize.Value.ToString());
        }

        if (!string.IsNullOrEmpty(request.LanguageCode))
        {
            httpRequest.AddQueryParameter("language_code", request.LanguageCode);
        }

        return await _httpClient.ExecuteAsync<ListVoicesResponse>(httpRequest, cancellationToken);
    }

    public async Task<CloneVoiceResponse> CloneVoiceAsync(CloneVoiceRequest request, CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest(ApiEndpoints.AudioVoiceClone, HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, request);

        return await _httpClient.ExecuteAsync<CloneVoiceResponse>(httpRequest, cancellationToken);
    }

    public async Task<CreateRoomResponse> CreateRoomAsync(CreateRoomRequest request, CancellationToken cancellationToken = default)
    {
        var httpRequest = _httpClient.CreateRequest(ApiEndpoints.AudioRooms, HttpMethodType.Post);
        _httpClient.AddJsonBody(httpRequest, request);

        return await _httpClient.ExecuteAsync<CreateRoomResponse>(httpRequest, cancellationToken);
    }

    private static string GetContentType(AudioFormat format)
    {
        return format switch
        {
            AudioFormat.Mp3 => "audio/mpeg",
            AudioFormat.Wav => "audio/wav",
            AudioFormat.Pcm => "audio/pcm",
            AudioFormat.Ogg => "audio/ogg",
            AudioFormat.Aac => "audio/aac",
            AudioFormat.Flac => "audio/flac",
            _ => "audio/mpeg"
        };
    }
}
