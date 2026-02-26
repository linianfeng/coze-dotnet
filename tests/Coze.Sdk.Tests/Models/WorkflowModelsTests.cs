using Coze.Sdk.Models.Chat;
using Coze.Sdk.Models.Workflows;
using FluentAssertions;
using Xunit;

namespace Coze.Sdk.Tests.Models;

public class WorkflowModelsTests
{
    public class WorkflowRequestTests
    {
        [Fact]
        public void WorkflowRequest_WithRequiredProperties_SetsCorrectValues()
        {
            // Act
            var request = new WorkflowRequest
            {
                WorkflowId = "wf-123"
            };

            // Assert
            request.WorkflowId.Should().Be("wf-123");
        }

        [Fact]
        public void WorkflowRequest_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var request = new WorkflowRequest
            {
                WorkflowId = "wf-123",
                Parameters = new Dictionary<string, object?> { ["input"] = "test" },
                Stream = true,
                BotId = "bot-456",
                UserId = "user-789"
            };

            // Assert
            request.WorkflowId.Should().Be("wf-123");
            request.Parameters.Should().ContainKey("input");
            request.Stream.Should().BeTrue();
            request.BotId.Should().Be("bot-456");
            request.UserId.Should().Be("user-789");
        }

        [Fact]
        public void WorkflowRequest_WithStreamTrue_SetsStreamProperty()
        {
            // Act
            var request = new WorkflowRequest
            {
                WorkflowId = "wf-123",
                Stream = true
            };

            // Assert
            request.Stream.Should().BeTrue();
        }
    }

    public class WorkflowResponseTests
    {
        [Fact]
        public void WorkflowResponse_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var response = new WorkflowResponse
            {
                Code = 0,
                Message = "",
                Data = "{\"code\":1,\"data\":[]}",
                ExecuteId = "exec-123",
                DebugUrl = "https://debug.url"
            };

            // Assert
            response.Code.Should().Be(0);
            response.Data.Should().Be("{\"code\":1,\"data\":[]}");
            response.ExecuteId.Should().Be("exec-123");
            response.DebugUrl.Should().Be("https://debug.url");
            response.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void WorkflowResponse_WithUsage_SetsUsage()
        {
            // Act
            var response = new WorkflowResponse
            {
                Code = 0,
                ExecuteId = "exec-123",
                Usage = new ChatUsage
                {
                    TokenCount = 150,
                    InputCount = 100,
                    OutputCount = 50
                }
            };

            // Assert
            response.Usage.Should().NotBeNull();
            response.Usage!.TokenCount.Should().Be(150);
            response.Usage.InputCount.Should().Be(100);
            response.Usage.OutputCount.Should().Be(50);
        }

        [Fact]
        public void WorkflowResponse_WithError_IndicatesFailure()
        {
            // Act
            var response = new WorkflowResponse
            {
                Code = 1,
                Message = "Error occurred"
            };

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.Code.Should().Be(1);
            response.Message.Should().Be("Error occurred");
        }
    }

    public class WorkflowEventTests
    {
        [Fact]
        public void ParseEvent_WithMessageEvent_ReturnsCorrectType()
        {
            // Act
            var evt = WorkflowEvent.ParseEvent("1", "message", "Hello", "log-123");

            // Assert
            evt.EventType.Should().Be(WorkflowEventType.Message);
            evt.Id.Should().Be("1");
            evt.Message.Should().Be("Hello");
            evt.LogId.Should().Be("log-123");
            evt.IsDone.Should().BeFalse();
        }

        [Fact]
        public void ParseEvent_WithDoneEvent_ReturnsCorrectType()
        {
            // Act
            var evt = WorkflowEvent.ParseEvent("2", "done", "", null);

            // Assert
            evt.EventType.Should().Be(WorkflowEventType.Done);
            evt.IsDone.Should().BeTrue();
        }

        [Fact]
        public void ParseEvent_WithErrorEvent_ReturnsCorrectType()
        {
            // Act
            var evt = WorkflowEvent.ParseEvent("3", "error", "Error message", "log-456");

            // Assert
            evt.EventType.Should().Be(WorkflowEventType.Error);
            evt.IsDone.Should().BeTrue();
        }

        [Theory]
        [InlineData("message", WorkflowEventType.Message)]
        [InlineData("error", WorkflowEventType.Error)]
        [InlineData("done", WorkflowEventType.Done)]
        [InlineData("unknown", WorkflowEventType.Done)] // Default case
        public void ParseEvent_WithVariousTypes_ReturnsCorrectType(string eventType, WorkflowEventType expectedType)
        {
            // Act
            var evt = WorkflowEvent.ParseEvent("id", eventType, "data", null);

            // Assert
            evt.EventType.Should().Be(expectedType);
        }
    }

    public class ResumeWorkflowRequestTests
    {
        [Fact]
        public void ResumeWorkflowRequest_WithRequiredProperties_SetsCorrectValues()
        {
            // Act
            var request = new ResumeWorkflowRequest
            {
                WorkflowRunId = "run-123",
                EventId = "event-456"
            };

            // Assert
            request.WorkflowRunId.Should().Be("run-123");
            request.EventId.Should().Be("event-456");
        }

        [Fact]
        public void ResumeWorkflowRequest_WithResumeData_SetsCorrectValues()
        {
            // Act
            var request = new ResumeWorkflowRequest
            {
                WorkflowRunId = "run-123",
                EventId = "event-456",
                ResumeData = new { key = "value" },
                Stream = true
            };

            // Assert
            request.ResumeData.Should().NotBeNull();
            request.Stream.Should().BeTrue();
        }
    }

    public class WorkflowExecuteStatusTests
    {
        [Theory]
        [InlineData(WorkflowExecuteStatus.Success)]
        [InlineData(WorkflowExecuteStatus.Running)]
        [InlineData(WorkflowExecuteStatus.Fail)]
        public void WorkflowExecuteStatus_AllValues_AreDefined(WorkflowExecuteStatus status)
        {
            // Assert
            ((int)status).Should().BeGreaterOrEqualTo(0);
        }
    }

    public class WorkflowRunModeTests
    {
        [Theory]
        [InlineData(WorkflowRunMode.Sync, 0)]
        [InlineData(WorkflowRunMode.Streaming, 1)]
        [InlineData(WorkflowRunMode.Async, 2)]
        public void WorkflowRunMode_HasCorrectValues(WorkflowRunMode mode, int expectedValue)
        {
            // Assert
            ((int)mode).Should().Be(expectedValue);
        }
    }

    public class WorkflowRunHistoryTests
    {
        [Fact]
        public void WorkflowRunHistory_WithAllProperties_SetsCorrectValues()
        {
            // Act
            var history = new WorkflowRunHistory
            {
                ExecuteId = "exec-123",
                ExecuteStatus = WorkflowExecuteStatus.Success,
                BotId = "bot-456",
                RunMode = WorkflowRunMode.Sync,
                Output = "result"
            };

            // Assert
            history.ExecuteId.Should().Be("exec-123");
            history.ExecuteStatus.Should().Be(WorkflowExecuteStatus.Success);
            history.BotId.Should().Be("bot-456");
            history.RunMode.Should().Be(WorkflowRunMode.Sync);
            history.Output.Should().Be("result");
        }
    }
}
