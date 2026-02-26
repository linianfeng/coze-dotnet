using Coze.Sdk.Models.Conversations;
using FluentAssertions;
using Xunit;

namespace Coze.Sdk.Tests.Models;

public class ConversationModelsTests
{
    public class ConversationTests
    {
        [Fact]
        public void Conversation_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var conversation = new Conversation
            {
                Id = "conv-123",
                CreatedAt = 1234567890,
                MetaData = new Dictionary<string, string> { ["key"] = "value" }
            };

            // Assert
            conversation.Id.Should().Be("conv-123");
            conversation.CreatedAt.Should().Be(1234567890);
            conversation.MetaData.Should().ContainKey("key");
        }
    }

    public class CreateConversationRequestTests
    {
        [Fact]
        public void CreateConversationRequest_WithMetaData_SetsCorrectValues()
        {
            // Act
            var request = new CreateConversationRequest
            {
                MetaData = new Dictionary<string, string>
                {
                    ["source"] = "test",
                    ["version"] = "1.0"
                }
            };

            // Assert
            request.MetaData.Should().HaveCount(2);
            request.MetaData["source"].Should().Be("test");
        }
    }

    public class CreateConversationResponseTests
    {
        [Fact]
        public void CreateConversationResponse_WithConversation_SetsCorrectValues()
        {
            // Act
            var response = new CreateConversationResponse
            {
                Conversation = new Conversation
                {
                    Id = "conv-123",
                    CreatedAt = 1234567890
                }
            };

            // Assert
            response.Conversation.Should().NotBeNull();
            response.Conversation!.Id.Should().Be("conv-123");
        }
    }

    public class ListConversationsRequestTests
    {
        [Fact]
        public void ListConversationsRequest_WithRequiredValues_SetsCorrectValues()
        {
            // Act
            var request = new ListConversationsRequest
            {
                BotId = "bot-123"
            };

            // Assert
            request.BotId.Should().Be("bot-123");
            request.PageNumber.Should().Be(1);
            request.PageSize.Should().Be(20);
        }

        [Fact]
        public void ListConversationsRequest_WithCustomValues_SetsCorrectValues()
        {
            // Act
            var request = new ListConversationsRequest
            {
                BotId = "bot-123",
                PageNumber = 2,
                PageSize = 50
            };

            // Assert
            request.BotId.Should().Be("bot-123");
            request.PageNumber.Should().Be(2);
            request.PageSize.Should().Be(50);
        }
    }

    public class ListConversationsResponseTests
    {
        [Fact]
        public void ListConversationsResponse_WithConversations_SetsCorrectValues()
        {
            // Act
            var response = new ListConversationsResponse
            {
                Conversations = new List<Conversation>
                {
                    new Conversation { Id = "conv-1" },
                    new Conversation { Id = "conv-2" }
                },
                HasMore = true
            };

            // Assert
            response.Conversations.Should().HaveCount(2);
            response.HasMore.Should().BeTrue();
        }
    }

    public class CreateConversationMessageRequestTests
    {
        [Fact]
        public void CreateConversationMessageRequest_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var request = new CreateConversationMessageRequest
            {
                ConversationId = "conv-123",
                Role = ConversationMessageRole.User,
                Content = "Hello, world!",
                ContentType = ConversationMessageContentType.Text
            };

            // Assert
            request.ConversationId.Should().Be("conv-123");
            request.Role.Should().Be(ConversationMessageRole.User);
            request.Content.Should().Be("Hello, world!");
            request.ContentType.Should().Be(ConversationMessageContentType.Text);
        }
    }

    public class ConversationMessageRoleTests
    {
        [Theory]
        [InlineData(ConversationMessageRole.User)]
        [InlineData(ConversationMessageRole.Assistant)]
        public void ConversationMessageRole_AllValues_AreDefined(ConversationMessageRole role)
        {
            // Assert
            ((int)role).Should().BeGreaterOrEqualTo(0);
        }
    }

    public class ConversationMessageContentTypeTests
    {
        [Theory]
        [InlineData(ConversationMessageContentType.Text)]
        [InlineData(ConversationMessageContentType.ObjectString)]
        [InlineData(ConversationMessageContentType.Audio)]
        [InlineData(ConversationMessageContentType.Card)]
        public void ConversationMessageContentType_AllValues_AreDefined(ConversationMessageContentType contentType)
        {
            // Assert
            ((int)contentType).Should().BeGreaterOrEqualTo(0);
        }
    }
}
