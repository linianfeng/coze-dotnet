using Coze.Sdk.Models.Bots;
using FluentAssertions;
using Xunit;

namespace Coze.Sdk.Tests.Models;

public class BotModelsTests
{
    public class BotTests
    {
        [Fact]
        public void Bot_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var bot = new Bot
            {
                Id = "bot-123",
                Name = "Test Bot",
                Description = "A test bot",
                IconUrl = "https://example.com/icon.png",
                CreateTime = 1234567890,
                UpdateTime = 1234567900
            };

            // Assert
            bot.Id.Should().Be("bot-123");
            bot.Name.Should().Be("Test Bot");
            bot.Description.Should().Be("A test bot");
            bot.IconUrl.Should().Be("https://example.com/icon.png");
            bot.CreateTime.Should().Be(1234567890);
            bot.UpdateTime.Should().Be(1234567900);
        }

        [Fact]
        public void Bot_WithPromptInfo_SetsPromptInfo()
        {
            // Act
            var bot = new Bot
            {
                Id = "bot-123",
                PromptInfo = new BotPromptInfo { Prompt = "You are a helpful assistant" }
            };

            // Assert
            bot.PromptInfo.Should().NotBeNull();
            bot.PromptInfo!.Prompt.Should().Be("You are a helpful assistant");
        }

        [Fact]
        public void Bot_WithPluginInfo_SetsPluginInfo()
        {
            // Act
            var bot = new Bot
            {
                Id = "bot-123",
                PluginInfo = new BotPluginInfo { PluginIds = new List<string> { "plugin-1", "plugin-2" } }
            };

            // Assert
            bot.PluginInfo.Should().NotBeNull();
            bot.PluginInfo!.PluginIds.Should().HaveCount(2);
        }

        [Fact]
        public void Bot_WithKnowledgeInfo_SetsKnowledgeInfo()
        {
            // Act
            var bot = new Bot
            {
                Id = "bot-123",
                KnowledgeInfo = new BotKnowledgeInfo { KnowledgeIds = new List<string> { "kb-1" } }
            };

            // Assert
            bot.KnowledgeInfo.Should().NotBeNull();
            bot.KnowledgeInfo!.KnowledgeIds.Should().Contain("kb-1");
        }

        [Fact]
        public void Bot_WithModelInfo_SetsModelInfo()
        {
            // Act
            var bot = new Bot
            {
                Id = "bot-123",
                ModelInfo = new BotModelInfo { ModelId = "gpt-4" }
            };

            // Assert
            bot.ModelInfo.Should().NotBeNull();
            bot.ModelInfo!.ModelId.Should().Be("gpt-4");
        }
    }

    public class SimpleBotTests
    {
        [Fact]
        public void SimpleBot_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var bot = new SimpleBot
            {
                Id = "bot-123",
                Name = "Simple Bot",
                Description = "A simple bot",
                IconUrl = "https://example.com/icon.png",
                PublishTime = "2024-01-01T00:00:00Z"
            };

            // Assert
            bot.Id.Should().Be("bot-123");
            bot.Name.Should().Be("Simple Bot");
            bot.Description.Should().Be("A simple bot");
            bot.IconUrl.Should().Be("https://example.com/icon.png");
            bot.PublishTime.Should().Be("2024-01-01T00:00:00Z");
        }
    }

    public class ListBotsRequestTests
    {
        [Fact]
        public void ListBotsRequest_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var request = new ListBotsRequest
            {
                SpaceId = "space-123",
                PageNumber = 1,
                PageSize = 20
            };

            // Assert
            request.SpaceId.Should().Be("space-123");
            request.PageNumber.Should().Be(1);
            request.PageSize.Should().Be(20);
        }
    }

    public class ListBotsResponseTests
    {
        [Fact]
        public void ListBotsResponse_WithBots_SetsCorrectValues()
        {
            // Act
            var response = new ListBotsResponse
            {
                Bots = new List<SimpleBot>
                {
                    new SimpleBot { Id = "bot-1", Name = "Bot 1" },
                    new SimpleBot { Id = "bot-2", Name = "Bot 2" }
                },
                Total = 2
            };

            // Assert
            response.Bots.Should().HaveCount(2);
            response.Total.Should().Be(2);
        }
    }
}
