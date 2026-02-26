using Coze.Sdk.Models.Chat;
using FluentAssertions;
using Xunit;

namespace Coze.Sdk.Tests.Models;

public class ChatModelsTests
{
    public class ChatTests
    {
        [Fact]
        public void Chat_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var chat = new Chat
            {
                Id = "chat-123",
                ConversationId = "conv-456",
                BotId = "bot-789",
                CreatedAt = 1234567890,
                CompletedAt = 1234567900,
                Status = ChatStatus.Completed,
                MetaData = new Dictionary<string, string> { ["key"] = "value" }
            };

            // Assert
            chat.Id.Should().Be("chat-123");
            chat.ConversationId.Should().Be("conv-456");
            chat.BotId.Should().Be("bot-789");
            chat.CreatedAt.Should().Be(1234567890);
            chat.CompletedAt.Should().Be(1234567900);
            chat.Status.Should().Be(ChatStatus.Completed);
            chat.MetaData.Should().ContainKey("key");
        }

        [Fact]
        public void Chat_FromJson_ParsesCorrectly()
        {
            // Arrange
            var json = @"{""id"":""chat-123"",""conversation_id"":""conv-456"",""bot_id"":""bot-789"",""status"":""completed""}";

            // Act
            var chat = Chat.FromJson(json);

            // Assert
            chat.Should().NotBeNull();
            chat!.Id.Should().Be("chat-123");
            chat.ConversationId.Should().Be("conv-456");
            chat.BotId.Should().Be("bot-789");
        }

        [Fact]
        public void Chat_FromJson_WithInvalidJson_ThrowsException()
        {
            // Arrange
            var invalidJson = "not valid json";

            // Act & Assert
            Assert.Throws<Newtonsoft.Json.JsonReaderException>(() => Chat.FromJson(invalidJson));
        }
    }

    public class ChatUsageTests
    {
        [Fact]
        public void ChatUsage_WithProperties_SetsCorrectValues()
        {
            // Act
            var usage = new ChatUsage
            {
                TokenCount = 100,
                InputCount = 80,
                OutputCount = 20
            };

            // Assert
            usage.TokenCount.Should().Be(100);
            usage.InputCount.Should().Be(80);
            usage.OutputCount.Should().Be(20);
        }
    }

    public class ChatErrorTests
    {
        [Fact]
        public void ChatError_WithProperties_SetsCorrectValues()
        {
            // Act
            var error = new ChatError
            {
                Code = 400,
                Message = "Error message"
            };

            // Assert
            error.Code.Should().Be(400);
            error.Message.Should().Be("Error message");
        }
    }

    public class ChatStatusTests
    {
        [Theory]
        [InlineData(ChatStatus.Created)]
        [InlineData(ChatStatus.InProgress)]
        [InlineData(ChatStatus.Completed)]
        [InlineData(ChatStatus.Failed)]
        [InlineData(ChatStatus.RequiresAction)]
        public void ChatStatus_AllValues_AreDefined(ChatStatus status)
        {
            // Assert
            ((int)status).Should().BeGreaterOrEqualTo(0);
        }
    }

    public class ChatRequestTests
    {
        [Fact]
        public void ChatRequest_WithRequiredProperties_HasCorrectValues()
        {
            // Act
            var request = new ChatRequest
            {
                BotId = "bot-123",
                UserId = "user-456"
            };

            // Assert
            request.BotId.Should().Be("bot-123");
            request.UserId.Should().Be("user-456");
        }

        [Fact]
        public void WithStream_ReturnsRequestWithStreamTrue()
        {
            // Arrange
            var request = new ChatRequest
            {
                BotId = "bot-123",
                UserId = "user-456"
            };

            // Act
            var streamingRequest = request.WithStream();

            // Assert
            streamingRequest.Stream.Should().BeTrue();
        }

        [Fact]
        public void WithoutStream_ReturnsRequestWithStreamFalse()
        {
            // Arrange
            var request = new ChatRequest
            {
                BotId = "bot-123",
                UserId = "user-456"
            };

            // Act
            var nonStreamingRequest = request.WithoutStream();

            // Assert
            nonStreamingRequest.Stream.Should().BeFalse();
            nonStreamingRequest.AutoSaveHistory.Should().BeTrue();
        }

        [Fact]
        public void ChatRequest_WithAllOptionalProperties_SetsCorrectValues()
        {
            // Act
            var request = new ChatRequest
            {
                BotId = "bot-123",
                UserId = "user-456",
                ConversationId = "conv-789",
                Messages = new List<Message> { Message.BuildUserQuestionText("Hello") },
                CustomVariables = new Dictionary<string, string> { ["var1"] = "value1" },
                MetaData = new Dictionary<string, string> { ["meta"] = "data" },
                AutoSaveHistory = true
            };

            // Assert
            request.ConversationId.Should().Be("conv-789");
            request.Messages.Should().HaveCount(1);
            request.CustomVariables.Should().ContainKey("var1");
            request.MetaData.Should().ContainKey("meta");
            request.AutoSaveHistory.Should().BeTrue();
        }
    }
}
