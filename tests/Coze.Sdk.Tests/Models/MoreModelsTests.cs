using Coze.Sdk.Models.Bots;
using Coze.Sdk.Models.Chat;
using Coze.Sdk.Models.Conversations;
using Coze.Sdk.Models.Datasets;
using Coze.Sdk.Models.Files;
using Coze.Sdk.Models.Templates;
using Coze.Sdk.Models.Workspaces;
using Coze.Sdk.Models.Variables;
using Coze.Sdk.Models.Connectors;
using Coze.Sdk.Models.Commerce;
using FluentAssertions;
using Xunit;

namespace Coze.Sdk.Tests.Models;

public class MoreModelsTests
{
    public class CreateBotRequestTests
    {
        [Fact]
        public void CreateBotRequest_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var request = new CreateBotRequest
            {
                SpaceId = "space-123",
                Name = "New Bot",
                Description = "Test description",
                IconFileId = "icon-456"
            };

            // Assert
            request.SpaceId.Should().Be("space-123");
            request.Name.Should().Be("New Bot");
            request.Description.Should().Be("Test description");
            request.IconFileId.Should().Be("icon-456");
        }
    }

    public class CreateBotResponseTests
    {
        [Fact]
        public void CreateBotResponse_WithBotId_SetsCorrectValues()
        {
            // Act
            var response = new CreateBotResponse
            {
                BotId = "bot-123"
            };

            // Assert
            response.BotId.Should().Be("bot-123");
        }
    }

    public class UpdateBotRequestTests
    {
        [Fact]
        public void UpdateBotRequest_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var request = new UpdateBotRequest
            {
                BotId = "bot-123",
                Name = "Updated Bot",
                Description = "Updated description"
            };

            // Assert
            request.BotId.Should().Be("bot-123");
            request.Name.Should().Be("Updated Bot");
            request.Description.Should().Be("Updated description");
        }
    }

    public class RetrieveBotRequestTests
    {
        [Fact]
        public void RetrieveBotRequest_WithBotId_SetsCorrectValues()
        {
            // Act
            var request = new RetrieveBotRequest
            {
                BotId = "bot-123"
            };

            // Assert
            request.BotId.Should().Be("bot-123");
        }
    }

    public class PublishBotRequestTests
    {
        [Fact]
        public void PublishBotRequest_WithBotId_SetsCorrectValues()
        {
            // Act
            var request = new PublishBotRequest
            {
                BotId = "bot-123"
            };

            // Assert
            request.BotId.Should().Be("bot-123");
        }
    }

    public class PublishBotResponseTests
    {
        [Fact]
        public void PublishBotResponse_WithBotId_SetsCorrectValues()
        {
            // Act
            var response = new PublishBotResponse
            {
                BotId = "bot-123"
            };

            // Assert
            response.BotId.Should().Be("bot-123");
        }
    }

    public class CancelChatRequestTests
    {
        [Fact]
        public void CancelChatRequest_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var request = new CancelChatRequest
            {
                ConversationId = "conv-123",
                ChatId = "chat-456"
            };

            // Assert
            request.ConversationId.Should().Be("conv-123");
            request.ChatId.Should().Be("chat-456");
        }
    }

    public class ListMessagesRequestTests
    {
        [Fact]
        public void ListMessagesRequest_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var request = new ListMessagesRequest
            {
                ConversationId = "conv-123",
                ChatId = "chat-456"
            };

            // Assert
            request.ConversationId.Should().Be("conv-123");
            request.ChatId.Should().Be("chat-456");
        }
    }

    public class RetrieveConversationRequestTests
    {
        [Fact]
        public void RetrieveConversationRequest_WithConversationId_SetsCorrectValues()
        {
            // Act
            var request = new RetrieveConversationRequest
            {
                ConversationId = "conv-123"
            };

            // Assert
            request.ConversationId.Should().Be("conv-123");
        }
    }

    public class ClearConversationRequestTests
    {
        [Fact]
        public void ClearConversationRequest_WithConversationId_SetsCorrectValues()
        {
            // Act
            var request = new ClearConversationRequest
            {
                ConversationId = "conv-123"
            };

            // Assert
            request.ConversationId.Should().Be("conv-123");
        }
    }

    public class ConversationMessageTests
    {
        [Fact]
        public void ConversationMessage_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var message = new ConversationMessage
            {
                Role = "user",
                Content = "Hello",
                ContentType = "text"
            };

            // Assert
            message.Role.Should().Be("user");
            message.Content.Should().Be("Hello");
            message.ContentType.Should().Be("text");
        }
    }

    public class CreateDatasetResponseTests
    {
        [Fact]
        public void CreateDatasetResponse_WithDataset_SetsCorrectValues()
        {
            // Act
            var response = new CreateDatasetResponse
            {
                Dataset = new Dataset
                {
                    DatasetId = "dataset-123",
                    Name = "Test Dataset"
                }
            };

            // Assert
            response.Dataset.Should().NotBeNull();
            response.Dataset!.DatasetId.Should().Be("dataset-123");
        }
    }

    public class CreateDocumentRequestTests
    {
        [Fact]
        public void CreateDocumentRequest_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var request = new CreateDocumentRequest
            {
                DatasetId = "dataset-123",
                Name = "Test Document"
            };

            // Assert
            request.DatasetId.Should().Be("dataset-123");
            request.Name.Should().Be("Test Document");
        }
    }

    public class DocumentTests
    {
        [Fact]
        public void Document_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var document = new Document
            {
                DocumentId = "doc-123",
                Name = "Test Document",
                DatasetId = "dataset-456",
                Status = DocumentStatus.Completed
            };

            // Assert
            document.DocumentId.Should().Be("doc-123");
            document.Name.Should().Be("Test Document");
            document.DatasetId.Should().Be("dataset-456");
            document.Status.Should().Be(DocumentStatus.Completed);
        }
    }

    public class DuplicateTemplateRequestTests
    {
        [Fact]
        public void DuplicateTemplateRequest_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var request = new DuplicateTemplateRequest
            {
                WorkspaceId = "workspace-456",
                Name = "Duplicated Bot"
            };

            // Assert
            request.WorkspaceId.Should().Be("workspace-456");
            request.Name.Should().Be("Duplicated Bot");
        }
    }

    public class DuplicateTemplateResponseTests
    {
        [Fact]
        public void DuplicateTemplateResponse_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var response = new DuplicateTemplateResponse
            {
                EntityId = "entity-123",
                EntityType = TemplateEntityType.Agent
            };

            // Assert
            response.EntityId.Should().Be("entity-123");
            response.EntityType.Should().Be(TemplateEntityType.Agent);
        }
    }

    public class ListWorkspacesRequestTests
    {
        [Fact]
        public void ListWorkspacesRequest_WithDefaultValues_SetsCorrectValues()
        {
            // Act
            var request = new ListWorkspacesRequest();

            // Assert
            request.PageNumber.Should().Be(1);
            request.PageSize.Should().Be(20);
        }
    }

    public class WorkspaceTests
    {
        [Fact]
        public void Workspace_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var workspace = new Workspace
            {
                Id = "workspace-123",
                Name = "Test Workspace",
                IconUrl = "https://example.com/icon.png"
            };

            // Assert
            workspace.Id.Should().Be("workspace-123");
            workspace.Name.Should().Be("Test Workspace");
            workspace.IconUrl.Should().Be("https://example.com/icon.png");
        }
    }

    public class InstallConnectorRequestTests
    {
        [Fact]
        public void InstallConnectorRequest_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var request = new InstallConnectorRequest
            {
                WorkspaceId = "workspace-123"
            };

            // Assert
            request.WorkspaceId.Should().Be("workspace-123");
        }
    }

    public class BenefitInfoTests
    {
        [Fact]
        public void BenefitInfo_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var info = new BenefitInfo
            {
                BenefitId = "benefit-123",
                BenefitType = BenefitType.ResourcePoint,
                Status = BenefitStatus.Valid
            };

            // Assert
            info.BenefitId.Should().Be("benefit-123");
            info.BenefitType.Should().Be(BenefitType.ResourcePoint);
            info.Status.Should().Be(BenefitStatus.Valid);
        }
    }

    public class BillTaskInfoTests
    {
        [Fact]
        public void BillTaskInfo_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var info = new BillTaskInfo
            {
                TaskId = "task-123",
                FileUrls = new List<string> { "https://example.com/file.csv" }
            };

            // Assert
            info.TaskId.Should().Be("task-123");
            info.FileUrls.Should().HaveCount(1);
        }
    }

    public class DocumentStatusTests
    {
        [Theory]
        [InlineData(DocumentStatus.Unknown)]
        [InlineData(DocumentStatus.Initializing)]
        [InlineData(DocumentStatus.Processing)]
        [InlineData(DocumentStatus.Completed)]
        [InlineData(DocumentStatus.Failed)]
        public void DocumentStatus_AllValues_AreDefined(DocumentStatus status)
        {
            // Assert
            ((int)status).Should().BeGreaterOrEqualTo(0);
        }
    }

    public class DocumentSourceTypeTests
    {
        [Theory]
        [InlineData(DocumentSourceType.FileUpload)]
        [InlineData(DocumentSourceType.WebPage)]
        [InlineData(DocumentSourceType.Notepad)]
        public void DocumentSourceType_AllValues_AreDefined(DocumentSourceType type)
        {
            // Assert
            ((int)type).Should().BeGreaterOrEqualTo(0);
        }
    }

    public class DocumentChunkStrategyTests
    {
        [Theory]
        [InlineData(DocumentChunkStrategy.Auto)]
        [InlineData(DocumentChunkStrategy.Manual)]
        public void DocumentChunkStrategy_AllValues_AreDefined(DocumentChunkStrategy strategy)
        {
            // Assert
            ((int)strategy).Should().BeGreaterOrEqualTo(0);
        }
    }

    public class TemplateEntityTypeTests
    {
        [Theory]
        [InlineData(TemplateEntityType.Agent)]
        public void TemplateEntityType_AllValues_AreDefined(TemplateEntityType type)
        {
            // Assert
            ((int)type).Should().BeGreaterOrEqualTo(0);
        }
    }

    public class BenefitStatusTests
    {
        [Theory]
        [InlineData(BenefitStatus.Valid)]
        [InlineData(BenefitStatus.Frozen)]
        public void BenefitStatus_AllValues_AreDefined(BenefitStatus status)
        {
            // Assert
            ((int)status).Should().BeGreaterOrEqualTo(0);
        }
    }

    public class BenefitTypeTests
    {
        [Theory]
        [InlineData(BenefitType.ResourcePoint)]
        public void BenefitType_AllValues_AreDefined(BenefitType type)
        {
            // Assert
            ((int)type).Should().BeGreaterOrEqualTo(0);
        }
    }

    public class ActiveModeTests
    {
        [Theory]
        [InlineData(ActiveMode.AbsoluteTime)]
        public void ActiveMode_AllValues_AreDefined(ActiveMode mode)
        {
            // Assert
            ((int)mode).Should().BeGreaterOrEqualTo(0);
        }
    }

    public class BenefitEntityTypeTests
    {
        [Theory]
        [InlineData(BenefitEntityType.EnterpriseAllDevices)]
        [InlineData(BenefitEntityType.EnterpriseAllIdentifiers)]
        [InlineData(BenefitEntityType.SingleDevice)]
        [InlineData(BenefitEntityType.SingleIdentifier)]
        public void BenefitEntityType_AllValues_AreDefined(BenefitEntityType type)
        {
            // Assert
            ((int)type).Should().BeGreaterOrEqualTo(0);
        }
    }
}
