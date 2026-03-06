using Coze.Sdk;
using Coze.Sdk.Models.Chat;
using Coze.Sdk.Models.Workflows;

namespace CozeAspNetCoreExample.Services;

public class CozeService : ICozeService
{
    private readonly ICozeClient _cozeClient;
    private const string DefaultUserId = "aspnet-core-user";
    private const int InitialStringBuilderCapacity = 1024; // 1KB 初始容量

    public CozeService(ICozeClient cozeClient)
    {
        _cozeClient = cozeClient;
    }

    public async Task<string> ChatWithBotAsync(string botId, string message, string? userId = null, CancellationToken cancellationToken = default)
    {
        var request = new ChatRequest
        {
            BotId = botId,
            UserId = userId ?? DefaultUserId,
            Messages = new List<Message>
            {
                Message.BuildUserQuestionText(message)
            }
        };

        var responseBuilder = new System.Text.StringBuilder(InitialStringBuilderCapacity);

        await foreach (var evt in _cozeClient.Chat.StreamAsync(request, cancellationToken))
        {
            switch (evt.EventType)
            {
                case ChatEventType.ConversationMessageDelta:
                    responseBuilder.Append(evt.Message?.Content);
                    break;
                case ChatEventType.ConversationChatCompleted:
                    // 聊天完成
                    break;
                case ChatEventType.Error:
                    throw new Exception($"Coze API Error: {evt.Message?.Content}");
            }
        }

        return responseBuilder.ToString();
    }

    public async IAsyncEnumerable<StreamChatEvent> ChatWithBotStreamAsync(
        string botId,
        string message,
        string? userId = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var request = new ChatRequest
        {
            BotId = botId,
            UserId = userId ?? DefaultUserId,
            Messages = new List<Message>
            {
                Message.BuildUserQuestionText(message)
            }
        };

        await foreach (var evt in _cozeClient.Chat.StreamAsync(request, cancellationToken))
        {
            switch (evt.EventType)
            {
                case ChatEventType.ConversationMessageDelta:
                    yield return new StreamChatEvent
                    {
                        EventType = "message",
                        Content = evt.Message?.Content
                    };
                    break;

                case ChatEventType.ConversationChatCompleted:
                    yield return new StreamChatEvent
                    {
                        EventType = "complete"
                    };
                    yield break;

                case ChatEventType.Error:
                    yield return new StreamChatEvent
                    {
                        EventType = "error",
                        Error = evt.Message?.Content ?? "Unknown error"
                    };
                    yield break;
            }
        }
    }

    #region 工作流方法

    public async Task<WorkflowResponse> RunWorkflowAsync(string workflowId, Dictionary<string, object?>? parameters = null, string? userId = null, CancellationToken cancellationToken = default)
    {
        var request = new WorkflowRequest
        {
            WorkflowId = workflowId,
            Parameters = parameters,
            UserId = userId ?? DefaultUserId
        };

        return await _cozeClient.Workflows.RunAsync(request, cancellationToken);
    }

    public async IAsyncEnumerable<StreamWorkflowEvent> RunWorkflowStreamAsync(
        string workflowId,
        Dictionary<string, object?>? parameters = null,
        string? userId = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var request = new WorkflowRequest
        {
            WorkflowId = workflowId,
            Parameters = parameters,
            UserId = userId ?? DefaultUserId
        };

        await foreach (var evt in _cozeClient.Workflows.StreamAsync(request, cancellationToken))
        {
            // 添加调试日志
            Console.WriteLine($"[DEBUG] Received event: Type={evt.EventType}, Message={evt.Message}, Id={evt.Id}");
            yield return MapWorkflowEvent(evt);
        }
    }

    public async IAsyncEnumerable<StreamWorkflowEvent> ResumeWorkflowStreamAsync(
        string workflowRunId,
        string eventId,
        object? resumeData = null,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var request = new ResumeWorkflowRequest
        {
            WorkflowRunId = workflowRunId,
            EventId = eventId,
            ResumeData = resumeData
        };

        await foreach (var evt in _cozeClient.Workflows.StreamResumeAsync(request, cancellationToken))
        {
            yield return MapWorkflowEvent(evt);
        }
    }

    public async Task<IReadOnlyList<WorkflowRunHistory>> GetWorkflowHistoryAsync(string workflowId, string executeId, CancellationToken cancellationToken = default)
    {
        return await _cozeClient.Workflows.GetRunHistoryAsync(workflowId, executeId, cancellationToken);
    }

    private static StreamWorkflowEvent MapWorkflowEvent(WorkflowEvent evt)
    {
        return evt.EventType switch
        {
            WorkflowEventType.Message => new StreamWorkflowEvent
            {
                EventType = "message",
                Content = evt.Message ?? string.Empty
            },
            WorkflowEventType.Error => new StreamWorkflowEvent
            {
                EventType = "error",
                Error = evt.Message ?? "Unknown error",
                Content = evt.Message ?? "Unknown error"
            },
            WorkflowEventType.Done => new StreamWorkflowEvent
            {
                EventType = "done",
                Content = evt.Message ?? string.Empty
            },
            WorkflowEventType.Interrupt => new StreamWorkflowEvent
            {
                EventType = "interrupt",
                ExecuteId = evt.Id,
                Content = evt.Message ?? string.Empty
            },
            _ => new StreamWorkflowEvent
            {
                EventType = "unknown",
                Content = evt.Message ?? string.Empty
            }
        };
    }

    #endregion
}