using Coze.Sdk.Models.Audio;

namespace Coze.Sdk.Clients;

/// <summary>
/// 音频操作接口。
/// 对应 Java SDK 中的 AudioService.java。
/// </summary>
public interface IAudioClient
{
    /// <summary>
    /// 从文本创建语音。
    /// </summary>
    Task<CreateSpeechResponse> CreateSpeechAsync(CreateSpeechRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 从音频创建转录。
    /// </summary>
    Task<CreateTranscriptionResponse> CreateTranscriptionAsync(CreateTranscriptionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 列出可用的语音。
    /// </summary>
    Task<ListVoicesResponse> ListVoicesAsync(ListVoicesRequest? request = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 从样本克隆语音。
    /// </summary>
    Task<CloneVoiceResponse> CloneVoiceAsync(CloneVoiceRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 创建用于实时通信的音频房间。
    /// </summary>
    Task<CreateRoomResponse> CreateRoomAsync(CreateRoomRequest request, CancellationToken cancellationToken = default);
}
