using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Coze.Sdk.Models.Audio;

/// <summary>
/// 音频格式。
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum AudioFormat
{
    /// <summary>
    /// MP3 格式。
    /// </summary>
    [EnumMember(Value = "mp3")]
    Mp3,

    /// <summary>
    /// WAV 格式。
    /// </summary>
    [EnumMember(Value = "wav")]
    Wav,

    /// <summary>
    /// PCM 格式。
    /// </summary>
    [EnumMember(Value = "pcm")]
    Pcm,

    /// <summary>
    /// OGG 格式。
    /// </summary>
    [EnumMember(Value = "ogg")]
    Ogg,

    /// <summary>
    /// AAC 格式。
    /// </summary>
    [EnumMember(Value = "aac")]
    Aac,

    /// <summary>
    /// FLAC 格式。
    /// </summary>
    [EnumMember(Value = "flac")]
    Flac
}

/// <summary>
/// 音色模型。
/// </summary>
public record Voice
{
    /// <summary>
    /// 获取音色 ID。
    /// </summary>
    [JsonProperty("voice_id")]
    public string? VoiceId { get; init; }

    /// <summary>
    /// 获取音色名称。
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; init; }

    /// <summary>
    /// 获取是否为系统音色。
    /// </summary>
    [JsonProperty("is_system_voice")]
    public bool? IsSystemVoice { get; init; }

    /// <summary>
    /// 获取语言代码。
    /// </summary>
    [JsonProperty("language_code")]
    public string? LanguageCode { get; init; }

    /// <summary>
    /// 获取语言名称。
    /// </summary>
    [JsonProperty("language_name")]
    public string? LanguageName { get; init; }

    /// <summary>
    /// 获取预览文本。
    /// </summary>
    [JsonProperty("preview_text")]
    public string? PreviewText { get; init; }

    /// <summary>
    /// 获取预览音频 URL。
    /// </summary>
    [JsonProperty("preview_audio")]
    public string? PreviewAudio { get; init; }

    /// <summary>
    /// 获取剩余训练次数。
    /// </summary>
    [JsonProperty("available_training_times")]
    public int? AvailableTrainingTimes { get; init; }

    /// <summary>
    /// 获取创建时间戳。
    /// </summary>
    [JsonProperty("create_time")]
    public int? CreateTime { get; init; }

    /// <summary>
    /// 获取更新时间戳。
    /// </summary>
    [JsonProperty("update_time")]
    public int? UpdateTime { get; init; }
}

/// <summary>
/// 创建语音合成（TTS）的请求。
/// </summary>
public record CreateSpeechRequest
{
    /// <summary>
    /// 获取输入文本。
    /// </summary>
    [JsonProperty("input")]
    public required string Input { get; init; }

    /// <summary>
    /// 获取音色 ID。
    /// </summary>
    [JsonProperty("voice_id")]
    public required string VoiceId { get; init; }

    /// <summary>
    /// 获取响应格式。
    /// </summary>
    [JsonProperty("response_format")]
    public AudioFormat ResponseFormat { get; init; } = AudioFormat.Mp3;

    /// <summary>
    /// 获取语速（0.5 到 2.0）。
    /// </summary>
    [JsonProperty("speed")]
    public float Speed { get; init; } = 1.0f;

    /// <summary>
    /// 获取采样率。
    /// </summary>
    [JsonProperty("sample_rate")]
    public int? SampleRate { get; init; }
}

/// <summary>
/// 创建语音合成的响应（音频数据）。
/// </summary>
public record CreateSpeechResponse
{
    /// <summary>
    /// 获取音频数据流。
    /// </summary>
    public Stream? AudioStream { get; init; }

    /// <summary>
    /// 获取内容类型。
    /// </summary>
    public string? ContentType { get; init; }
}

/// <summary>
/// 创建语音转写的请求。
/// </summary>
public record CreateTranscriptionRequest
{
    /// <summary>
    /// 获取文件 ID。
    /// </summary>
    [JsonProperty("file_id")]
    public required string FileId { get; init; }

    /// <summary>
    /// 获取语言代码。
    /// </summary>
    [JsonProperty("language")]
    public string? Language { get; init; }

    /// <summary>
    /// 获取提示词以改善识别效果。
    /// </summary>
    [JsonProperty("prompt")]
    public string? Prompt { get; init; }
}

/// <summary>
/// 创建语音转写的响应。
/// </summary>
public record CreateTranscriptionResponse
{
    /// <summary>
    /// 获取转写文本。
    /// </summary>
    [JsonProperty("text")]
    public string? Text { get; init; }

    /// <summary>
    /// 获取检测到的语言。
    /// </summary>
    [JsonProperty("language")]
    public string? Language { get; init; }

    /// <summary>
    /// 获取时长（秒）。
    /// </summary>
    [JsonProperty("duration")]
    public float? Duration { get; init; }
}

/// <summary>
/// 列出音色的请求。
/// </summary>
public record ListVoicesRequest
{
    /// <summary>
    /// 获取页码。
    /// </summary>
    [JsonProperty("page_num")]
    public int? PageNumber { get; init; } = 1;

    /// <summary>
    /// 获取每页数量。
    /// </summary>
    [JsonProperty("page_size")]
    public int? PageSize { get; init; } = 50;

    /// <summary>
    /// 获取按语言代码过滤。
    /// </summary>
    [JsonProperty("language_code")]
    public string? LanguageCode { get; init; }
}

/// <summary>
/// 列出音色的响应。
/// </summary>
public record ListVoicesResponse
{
    /// <summary>
    /// 获取音色列表。
    /// </summary>
    [JsonProperty("data")]
    public IReadOnlyList<Voice>? Voices { get; init; }

    /// <summary>
    /// 获取是否还有更多音色。
    /// </summary>
    [JsonProperty("has_more")]
    public bool HasMore { get; init; }
}

/// <summary>
/// 克隆音色的请求。
/// </summary>
public record CloneVoiceRequest
{
    /// <summary>
    /// 获取音色名称。
    /// </summary>
    [JsonProperty("voice_name")]
    public required string VoiceName { get; init; }

    /// <summary>
    /// 获取音色样本的文件 ID。
    /// </summary>
    [JsonProperty("file_id")]
    public required string FileId { get; init; }

    /// <summary>
    /// 获取描述。
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; init; }
}

/// <summary>
/// 克隆音色的响应。
/// </summary>
public record CloneVoiceResponse
{
    /// <summary>
    /// 获取音色 ID。
    /// </summary>
    [JsonProperty("voice_id")]
    public string? VoiceId { get; init; }
}

/// <summary>
/// 创建音频房间的请求。
/// </summary>
public record CreateRoomRequest
{
    /// <summary>
    /// 获取 Bot ID。
    /// </summary>
    [JsonProperty("bot_id")]
    public required string BotId { get; init; }

    /// <summary>
    /// 获取语音合成（TTS）使用的音色 ID。
    /// </summary>
    [JsonProperty("voice_id")]
    public string? VoiceId { get; init; }

    /// <summary>
    /// 获取房间配置。
    /// </summary>
    [JsonProperty("room_config")]
    public RoomConfig? RoomConfig { get; init; }
}

/// <summary>
/// 房间配置。
/// </summary>
public record RoomConfig
{
    /// <summary>
    /// 获取音频配置。
    /// </summary>
    [JsonProperty("audio_config")]
    public RoomAudioConfig? AudioConfig { get; init; }

    /// <summary>
    /// 获取翻译配置。
    /// </summary>
    [JsonProperty("translate_config")]
    public TranslateConfig? TranslateConfig { get; init; }
}

/// <summary>
/// 房间音频配置。
/// </summary>
public record RoomAudioConfig
{
    /// <summary>
    /// 获取音频编解码器。
    /// </summary>
    [JsonProperty("codec")]
    public string? Codec { get; init; }

    /// <summary>
    /// 获取采样率。
    /// </summary>
    [JsonProperty("sample_rate")]
    public int? SampleRate { get; init; }
}

/// <summary>
/// 翻译配置。
/// </summary>
public record TranslateConfig
{
    /// <summary>
    /// 获取是否启用翻译。
    /// </summary>
    [JsonProperty("enabled")]
    public bool Enabled { get; init; }

    /// <summary>
    /// 获取目标语言代码。
    /// </summary>
    [JsonProperty("target_language")]
    public string? TargetLanguage { get; init; }
}

/// <summary>
/// 创建房间的响应。
/// </summary>
public record CreateRoomResponse
{
    /// <summary>
    /// 获取房间 ID。
    /// </summary>
    [JsonProperty("room_id")]
    public string? RoomId { get; init; }

    /// <summary>
    /// 获取 WebSocket URL。
    /// </summary>
    [JsonProperty("ws_url")]
    public string? WebSocketUrl { get; init; }
}
