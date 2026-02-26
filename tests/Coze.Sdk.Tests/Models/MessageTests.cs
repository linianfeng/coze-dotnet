using Coze.Sdk.Models.Chat;
using FluentAssertions;
using Xunit;

namespace Coze.Sdk.Tests.Models;

public class MessageTests
{
    [Fact]
    public void BuildUserQuestionText_WithContent_ReturnsCorrectMessage()
    {
        // Act
        var message = Message.BuildUserQuestionText("Hello");

        // Assert
        message.Role.Should().Be(MessageRole.User);
        message.Type.Should().Be(MessageType.Question);
        message.Content.Should().Be("Hello");
        message.ContentType.Should().Be(MessageContentType.Text);
    }

    [Fact]
    public void BuildUserQuestionText_WithMetaData_ReturnsMessageWithMetaData()
    {
        // Arrange
        var metaData = new Dictionary<string, string> { ["key"] = "value" };

        // Act
        var message = Message.BuildUserQuestionText("Hello", metaData);

        // Assert
        message.MetaData.Should().ContainKey("key");
        message.MetaData!["key"].Should().Be("value");
    }

    [Fact]
    public void BuildAssistantAnswer_WithContent_ReturnsCorrectMessage()
    {
        // Act
        var message = Message.BuildAssistantAnswer("Response");

        // Assert
        message.Role.Should().Be(MessageRole.Assistant);
        message.Type.Should().Be(MessageType.Answer);
        message.Content.Should().Be("Response");
        message.ContentType.Should().Be(MessageContentType.Text);
    }

    [Fact]
    public void BuildAssistantAnswer_WithMetaData_ReturnsCorrectMessage()
    {
        // Arrange
        var metaData = new Dictionary<string, string> { ["source"] = "bot" };

        // Act
        var message = Message.BuildAssistantAnswer("Response", metaData);

        // Assert
        message.MetaData.Should().ContainKey("source");
    }

    [Fact]
    public void Audio_WithNonAudioContentType_ReturnsNull()
    {
        // Arrange
        var message = new Message
        {
            ContentType = MessageContentType.Text,
            Content = "text content"
        };

        // Act & Assert
        message.Audio.Should().BeNull();
    }

    [Fact]
    public void Audio_WithAudioContent_ReturnsDecodedBytes()
    {
        // Arrange
        var audioData = new byte[] { 1, 2, 3, 4, 5 };
        var base64Audio = Convert.ToBase64String(audioData);
        var message = new Message
        {
            Role = MessageRole.Assistant,
            Content = base64Audio,
            ContentType = MessageContentType.Audio
        };

        // Act & Assert
        message.Audio.Should().NotBeNull();
        message.Audio.Should().Equal(audioData);
    }

    [Fact]
    public void Audio_WithNullContent_ReturnsNull()
    {
        // Arrange
        var message = new Message
        {
            Role = MessageRole.Assistant,
            Content = null,
            ContentType = MessageContentType.Audio
        };

        // Act & Assert
        message.Audio.Should().BeNull();
    }

    [Fact]
    public void FromJson_WithValidJson_ReturnsMessage()
    {
        // Arrange
        var json = @"{""role"":""user"",""content"":""Hello"",""content_type"":""text""}";

        // Act
        var message = Message.FromJson(json);

        // Assert
        message.Should().NotBeNull();
        message!.Content.Should().Be("Hello");
    }

    [Fact]
    public void FromJson_WithInvalidJson_ThrowsException()
    {
        // Arrange
        var invalidJson = "not valid json";

        // Act & Assert
        Assert.Throws<Newtonsoft.Json.JsonReaderException>(() => Message.FromJson(invalidJson));
    }

    [Fact]
    public void Message_WithAllProperties_SetsCorrectValues()
    {
        // Act
        var message = new Message
        {
            Id = "msg-123",
            ConversationId = "conv-456",
            ChatId = "chat-789",
            BotId = "bot-abc",
            Role = MessageRole.User,
            Type = MessageType.Question,
            Content = "Hello",
            ContentType = MessageContentType.Text,
            CreatedAt = 1234567890,
            UpdatedAt = 1234567900,
            SectionId = "section-1",
            ReasoningContent = "reasoning text",
            MetaData = new Dictionary<string, string> { ["key"] = "value" }
        };

        // Assert
        message.Id.Should().Be("msg-123");
        message.ConversationId.Should().Be("conv-456");
        message.ChatId.Should().Be("chat-789");
        message.BotId.Should().Be("bot-abc");
        message.Role.Should().Be(MessageRole.User);
        message.Type.Should().Be(MessageType.Question);
        message.Content.Should().Be("Hello");
        message.ContentType.Should().Be(MessageContentType.Text);
        message.CreatedAt.Should().Be(1234567890);
        message.UpdatedAt.Should().Be(1234567900);
        message.SectionId.Should().Be("section-1");
        message.ReasoningContent.Should().Be("reasoning text");
        message.MetaData.Should().ContainKey("key");
    }
}

public class MessageRoleTests
{
    [Theory]
    [InlineData(MessageRole.User)]
    [InlineData(MessageRole.Assistant)]
    public void MessageRole_AllValues_AreDefined(MessageRole role)
    {
        // Assert
        ((int)role).Should().BeGreaterOrEqualTo(0);
    }
}

public class MessageTypeTests
{
    [Theory]
    [InlineData(MessageType.Question)]
    [InlineData(MessageType.Answer)]
    [InlineData(MessageType.FollowUp)]
    public void MessageType_AllValues_AreDefined(MessageType type)
    {
        // Assert
        ((int)type).Should().BeGreaterOrEqualTo(0);
    }
}

public class MessageContentTypeTests
{
    [Theory]
    [InlineData(MessageContentType.Text)]
    [InlineData(MessageContentType.Audio)]
    public void MessageContentType_AllValues_AreDefined(MessageContentType contentType)
    {
        // Assert
        ((int)contentType).Should().BeGreaterOrEqualTo(0);
    }
}
