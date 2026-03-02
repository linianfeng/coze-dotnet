using Coze.Sdk;
using Coze.Sdk.Models.Chat;

namespace CozeAspNetCoreExample.Services;

public class CozeService : ICozeService
{
    private readonly ICozeClient _cozeClient;

    public CozeService(ICozeClient cozeClient)
    {
        _cozeClient = cozeClient;
    }

    public async Task<string> ChatWithBotAsync(string botId, string message, string? userId = null, CancellationToken cancellationToken = default)
    {
        var request = new ChatRequest
        {
            BotId = botId,
            UserId = userId ?? "aspnet-core-user",
            Messages = new List<Message>
            {
                Message.BuildUserQuestionText(message)
            }
        };

        var responseBuilder = new System.Text.StringBuilder();

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
}