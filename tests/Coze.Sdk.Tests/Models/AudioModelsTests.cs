using Coze.Sdk.Models.Audio;
using FluentAssertions;
using Xunit;

namespace Coze.Sdk.Tests.Models;

public class AudioModelsTests
{
    public class VoiceTests
    {
        [Fact]
        public void Voice_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var voice = new Voice
            {
                VoiceId = "voice-123",
                Name = "Test Voice",
                LanguageCode = "zh-CN",
                PreviewAudio = "https://example.com/preview.mp3"
            };

            // Assert
            voice.VoiceId.Should().Be("voice-123");
            voice.Name.Should().Be("Test Voice");
            voice.LanguageCode.Should().Be("zh-CN");
            voice.PreviewAudio.Should().Be("https://example.com/preview.mp3");
        }
    }

    public class CreateSpeechRequestTests
    {
        [Fact]
        public void CreateSpeechRequest_WithRequiredProperties_SetsCorrectValues()
        {
            // Act
            var request = new CreateSpeechRequest
            {
                Input = "Hello, world!",
                VoiceId = "voice-123"
            };

            // Assert
            request.Input.Should().Be("Hello, world!");
            request.VoiceId.Should().Be("voice-123");
        }

        [Fact]
        public void CreateSpeechRequest_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var request = new CreateSpeechRequest
            {
                Input = "Hello, world!",
                VoiceId = "voice-123",
                ResponseFormat = AudioFormat.Mp3,
                Speed = 1.0f
            };

            // Assert
            request.ResponseFormat.Should().Be(AudioFormat.Mp3);
            request.Speed.Should().Be(1.0f);
        }
    }

    public class CreateSpeechResponseTests
    {
        [Fact]
        public void CreateSpeechResponse_WithProperties_SetsCorrectValues()
        {
            // Act
            var response = new CreateSpeechResponse
            {
                ContentType = "audio/mpeg"
            };

            // Assert
            response.ContentType.Should().Be("audio/mpeg");
        }
    }

    public class CreateTranscriptionRequestTests
    {
        [Fact]
        public void CreateTranscriptionRequest_WithRequiredProperties_SetsCorrectValues()
        {
            // Act
            var request = new CreateTranscriptionRequest
            {
                FileId = "file-123"
            };

            // Assert
            request.FileId.Should().Be("file-123");
        }

        [Fact]
        public void CreateTranscriptionRequest_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var request = new CreateTranscriptionRequest
            {
                FileId = "file-123",
                Language = "zh"
            };

            // Assert
            request.Language.Should().Be("zh");
        }
    }

    public class CreateTranscriptionResponseTests
    {
        [Fact]
        public void CreateTranscriptionResponse_WithProperties_SetsCorrectValues()
        {
            // Act
            var response = new CreateTranscriptionResponse
            {
                Text = "Transcribed text",
                Language = "zh",
                Duration = 10.5f
            };

            // Assert
            response.Text.Should().Be("Transcribed text");
            response.Language.Should().Be("zh");
            response.Duration.Should().Be(10.5f);
        }
    }

    public class ListVoicesRequestTests
    {
        [Fact]
        public void ListVoicesRequest_WithDefaultValues_SetsCorrectValues()
        {
            // Act
            var request = new ListVoicesRequest();

            // Assert
            request.PageNumber.Should().Be(1);
            request.PageSize.Should().Be(50);
        }

        [Fact]
        public void ListVoicesRequest_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var request = new ListVoicesRequest
            {
                PageNumber = 2,
                PageSize = 100
            };

            // Assert
            request.PageNumber.Should().Be(2);
            request.PageSize.Should().Be(100);
        }
    }

    public class ListVoicesResponseTests
    {
        [Fact]
        public void ListVoicesResponse_WithVoices_SetsCorrectValues()
        {
            // Act
            var response = new ListVoicesResponse
            {
                Voices = new List<Voice>
                {
                    new Voice { VoiceId = "voice-1", Name = "Voice 1" },
                    new Voice { VoiceId = "voice-2", Name = "Voice 2" }
                }
            };

            // Assert
            response.Voices.Should().HaveCount(2);
        }
    }

    public class CreateRoomRequestTests
    {
        [Fact]
        public void CreateRoomRequest_WithRequiredProperties_SetsCorrectValues()
        {
            // Act
            var request = new CreateRoomRequest
            {
                BotId = "bot-123"
            };

            // Assert
            request.BotId.Should().Be("bot-123");
        }

        [Fact]
        public void CreateRoomRequest_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var request = new CreateRoomRequest
            {
                BotId = "bot-123",
                VoiceId = "voice-456",
                RoomConfig = new RoomConfig
                {
                    AudioConfig = new RoomAudioConfig
                    {
                        Codec = "opus",
                        SampleRate = 48000
                    }
                }
            };

            // Assert
            request.VoiceId.Should().Be("voice-456");
            request.RoomConfig.Should().NotBeNull();
            request.RoomConfig!.AudioConfig.Should().NotBeNull();
            request.RoomConfig!.AudioConfig!.Codec.Should().Be("opus");
        }
    }

    public class CreateRoomResponseTests
    {
        [Fact]
        public void CreateRoomResponse_WithProperties_SetsCorrectValues()
        {
            // Act
            var response = new CreateRoomResponse
            {
                RoomId = "room-123",
                WebSocketUrl = "wss://example.com/ws"
            };

            // Assert
            response.RoomId.Should().Be("room-123");
            response.WebSocketUrl.Should().Be("wss://example.com/ws");
        }
    }

    public class AudioFormatTests
    {
        [Theory]
        [InlineData(AudioFormat.Mp3)]
        [InlineData(AudioFormat.Ogg)]
        [InlineData(AudioFormat.Aac)]
        [InlineData(AudioFormat.Flac)]
        [InlineData(AudioFormat.Wav)]
        [InlineData(AudioFormat.Pcm)]
        public void AudioFormat_AllValues_AreDefined(AudioFormat format)
        {
            // Assert
            ((int)format).Should().BeGreaterOrEqualTo(0);
        }
    }
}
