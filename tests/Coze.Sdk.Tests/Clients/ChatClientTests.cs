using Coze.Sdk.Models.Chat;
using FluentAssertions;
using Xunit;

namespace Coze.Sdk.Tests.Clients;

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
}
