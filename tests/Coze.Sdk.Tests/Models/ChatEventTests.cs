using Coze.Sdk.Exceptions;
using Coze.Sdk.Models.Chat;
using FluentAssertions;
using Xunit;

namespace Coze.Sdk.Tests.Models;

public class ChatEventTests
{
    [Fact]
    public void ParseEvent_WithDoneEvent_ReturnsDoneEvent()
    {
        // Act
        var result = ChatEvent.ParseEvent("done", "", "log-123");

        // Assert
        result.EventType.Should().Be(ChatEventType.Done);
        result.LogId.Should().Be("log-123");
        result.IsDone.Should().BeTrue();
    }

    [Fact]
    public void ParseEvent_WithErrorEvent_ThrowsCozeApiException()
    {
        // Act
        var act = () => ChatEvent.ParseEvent("error", "Error message", "log-123");

        // Assert
        act.Should().Throw<CozeApiException>()
            .Where(e => e.LogId == "log-123");
    }

    [Fact]
    public void ParseEvent_WithMessageDeltaEvent_ReturnsMessageEvent()
    {
        // Arrange
        var messageJson = "{\"content\":\"Hello\",\"role\":\"assistant\"}";

        // Act
        var result = ChatEvent.ParseEvent("conversation.message.delta", messageJson, "log-123");

        // Assert
        result.EventType.Should().Be(ChatEventType.ConversationMessageDelta);
        result.Message.Should().NotBeNull();
        result.Message!.Content.Should().Be("Hello");
    }

    [Theory]
    [InlineData("conversation.chat.created", ChatEventType.ConversationChatCreated)]
    [InlineData("conversation.chat.in_progress", ChatEventType.ConversationChatInProgress)]
    [InlineData("conversation.message.delta", ChatEventType.ConversationMessageDelta)]
    [InlineData("conversation.message.completed", ChatEventType.ConversationMessageCompleted)]
    [InlineData("conversation.chat.completed", ChatEventType.ConversationChatCompleted)]
    [InlineData("conversation.chat.failed", ChatEventType.ConversationChatFailed)]
    [InlineData("conversation.chat.requires_action", ChatEventType.ConversationChatRequiresAction)]
    [InlineData("conversation.audio.delta", ChatEventType.ConversationAudioDelta)]
    [InlineData("done", ChatEventType.Done)]
    public void ParseEvent_WithVariousEventTypes_ReturnsCorrectEventType(string eventTypeString, ChatEventType expectedType)
    {
        // Act
        var result = ChatEvent.ParseEvent(eventTypeString, "", null);

        // Assert
        result.EventType.Should().Be(expectedType);
    }

    [Fact]
    public void IsDone_WithDoneEvent_ReturnsTrue()
    {
        // Arrange
        var evt = new ChatEvent { EventType = ChatEventType.Done };

        // Act & Assert
        evt.IsDone.Should().BeTrue();
    }

    [Fact]
    public void IsDone_WithErrorEvent_ReturnsTrue()
    {
        // Arrange
        var evt = new ChatEvent { EventType = ChatEventType.Error };

        // Act & Assert
        evt.IsDone.Should().BeTrue();
    }

    [Fact]
    public void IsDone_WithOtherEvent_ReturnsFalse()
    {
        // Arrange
        var evt = new ChatEvent { EventType = ChatEventType.ConversationMessageDelta };

        // Act & Assert
        evt.IsDone.Should().BeFalse();
    }
}
