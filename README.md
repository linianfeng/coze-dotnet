# Coze .NET SDK

Coze API .NET 8.0 SDK 从 扣子官方的 Java SDK 移植而来。，提供 Chat、Bots、Workflows 等 API 的同步/流式调用能力。

## 项目结构

```
coze-dotnet/
├── src/Coze.Sdk/           # 核心 SDK
│   ├── Clients/            # API 客户端接口 (IChatClient, IBotsClient, etc.)
│   ├── Authentication/     # 认证方式 (TokenAuth, JwtOAuthClient)
│   ├── WebSocket/          # WebSocket 客户端
│   ├── Models/             # 数据模型
│   ├── Http/               # HTTP 底层实现
│   ├── Exceptions/         # 异常定义
│   └── Extensions/         # 依赖注入扩展
├── tests/Coze.Sdk.Tests/   # 单元测试 (xUnit)
└── examples/CozeExample/   # 使用示例
```

## 安装

通过 NuGet 安装：

```bash
dotnet add package Coze.Sdk
```

## 快速开始

### 基本使用

```csharp
using Coze.Sdk;
using Coze.Sdk.Authentication;
using Coze.Sdk.Models.Chat;

// 创建客户端
using var client = new CozeClient(new CozeOptions
{
    Auth = new TokenAuth("your-api-token"),
    BaseUrl = "https://api.coze.cn"
});

// 流式对话
var request = new ChatRequest
{
    BotId = "your-bot-id",
    UserId = "user-123",
    Messages = new List<Message>
    {
        Message.BuildUserQuestionText("你好，请介绍一下你自己")
    }
};

await foreach (var evt in client.Chat.StreamAsync(request))
{
    if (evt.EventType == ChatEventType.ConversationMessageDelta)
    {
        Console.Write(evt.Message?.Content);
    }
    else if (evt.EventType == ChatEventType.ConversationChatCompleted)
    {
        Console.WriteLine($"\nToken 用量: {evt.Chat?.Usage?.TokenCount}");
    }
}
```

### 非流式对话（轮询模式）

```csharp
var result = await client.Chat.CreateAndPollAsync(request, TimeSpan.FromSeconds(60));

Console.WriteLine($"状态: {result.Chat?.Status}");
foreach (var msg in result.Messages ?? new List<Message>())
{
    if (msg.Type == MessageType.Answer)
    {
        Console.WriteLine($"回答: {msg.Content}");
    }
}
```

### 依赖注入 (ASP.NET Core)

```csharp
// 在 Program.cs 中
builder.Services.AddCozeSdk(options =>
{
    options.BaseUrl = "https://api.coze.cn";
    options.Auth = new TokenAuth(builder.Configuration["Coze:Token"]);
});

// 或从配置中读取
builder.Services.AddCozeSdk(builder.Configuration, "Coze");
```

```json
// appsettings.json
{
  "Coze": {
    "BaseUrl": "https://api.coze.cn",
    "Token": "your-api-token"
  }
}
```

## 主要功能

### Chat 聊天

```csharp
// 创建流式聊天
await foreach (var evt in client.Chat.StreamAsync(request))
{
    // 处理事件
}

// 创建非流式聊天
var response = await client.Chat.CreateAsync(request);

// 创建并轮询
var result = await client.Chat.CreateAndPollAsync(request, timeout: TimeSpan.FromSeconds(30));

// 获取聊天详情
var chat = await client.Chat.RetrieveAsync(conversationId, chatId);

// 取消聊天
var canceledChat = await client.Chat.CancelAsync(new CancelChatRequest
{
    ConversationId = conversationId,
    ChatId = chatId
});

// 获取聊天消息
var messages = await client.Chat.Messages.ListAsync(new ListMessagesRequest
{
    ConversationId = conversationId,
    ChatId = chatId
});
```

### Bots 机器人管理

```csharp
// 列出机器人
var bots = await client.Bots.ListAsync(new ListBotsRequest
{
    SpaceId = "space-id",
    PageNumber = 1,
    PageSize = 20
});

// 获取机器人详情
var bot = await client.Bots.RetrieveAsync("bot-id");

// 创建机器人
var createResponse = await client.Bots.CreateAsync(new CreateBotRequest
{
    SpaceId = "space-id",
    Name = "My Bot",
    Description = "A helpful assistant"
});

// 更新机器人
await client.Bots.UpdateAsync(new UpdateBotRequest
{
    BotId = "bot-id",
    Name = "Updated Name"
});

// 发布机器人
await client.Bots.PublishAsync(new PublishBotRequest
{
    BotId = "bot-id"
});
```

### Workflows 工作流

```csharp
// 运行工作流（非流式）
var response = await client.Workflows.RunAsync(new WorkflowRequest
{
    WorkflowId = "workflow-id",
    Parameters = new Dictionary<string, object?>
    {
        ["input"] = "Hello"
    }
});

// 运行工作流（流式）
await foreach (var evt in client.Workflows.StreamAsync(request))
{
    if (evt.EventType == WorkflowEventType.Message)
    {
        Console.WriteLine(evt.Message);
    }
}
```

### Conversations 会话管理

```csharp
// 创建会话
var conversation = await client.Conversations.CreateAsync(new CreateConversationRequest
{
    BotId = "bot-id"
});

// 获取会话列表
var conversations = await client.Conversations.ListAsync(new ListConversationsRequest
{
    BotId = "bot-id",
    PageNumber = 1,
    PageSize = 20
});

// 获取会话详情
var conv = await client.Conversations.RetrieveAsync(conversationId);
```

### Datasets 数据集管理

```csharp
// 创建数据集
var dataset = await client.Datasets.CreateAsync(new CreateDatasetRequest
{
    SpaceId = "space-id",
    Name = "My Dataset",
    Type = DatasetType.Knowledge
});

// 列出数据集
var datasets = await client.Datasets.ListAsync(new ListDatasetsRequest
{
    SpaceId = "space-id"
});

// 获取数据集记录
var records = await client.DatasetRecords.ListAsync(new ListDatasetRecordsRequest
{
    DatasetId = "dataset-id"
});

// 添加数据集记录
await client.DatasetRecords.CreateAsync(new CreateDatasetRecordRequest
{
    DatasetId = "dataset-id",
    Data = new Dictionary<string, object>
    {
        ["content"] = "文档内容"
    }
});
```

## 认证

### Token 认证

```csharp
var auth = new TokenAuth("your-personal-access-token");
```

### JWT OAuth（服务账户）

```csharp
var oauthClient = new JwtOAuthClient(new OAuthOptions
{
    ClientId = "client-id",
    ClientSecret = "private-key-content",
    BaseUrl = "https://api.coze.cn"
});

var token = await oauthClient.GetAccessTokenAsync();
```

## 错误处理

```csharp
try
{
    var response = await client.Chat.CreateAsync(request);
}
catch (CozeApiException ex)
{
    Console.WriteLine($"API 错误: {ex.Message}");
    Console.WriteLine($"状态码: {ex.StatusCode}");
    Console.WriteLine($"错误码: {ex.ErrorCode}");
    Console.WriteLine($"LogId: {ex.LogId}");
}
catch (CozeAuthException ex)
{
    Console.WriteLine($"认证错误: {ex.Message}");
    Console.WriteLine($"错误码: {ex.ErrorCode}");
}
```

## 配置选项

| 选项 | 类型 | 默认值 | 描述 |
|------|------|--------|------|
| BaseUrl | string | https://api.coze.cn | API 基础 URL |
| ReadTimeout | TimeSpan | 633秒 | 读取超时时间 |
| ConnectTimeout | TimeSpan | 5秒 | 连接超时时间 |
| Auth | IAuth | required | 认证提供者 |
| LoggerFactory | ILoggerFactory | null | 日志工厂 |

## 事件类型

### ChatEventType

| 类型 | 描述 |
|------|------|
| ConversationChatCreated | 会话创建 |
| ConversationChatInProgress | 会话处理中 |
| ConversationMessageDelta | 消息增量 |
| ConversationMessageCompleted | 消息完成 |
| ConversationChatCompleted | 会话完成 |
| ConversationChatFailed | 会话失败 |
| ConversationChatRequiresAction | 需要用户操作 |
| ConversationAudioDelta | 音频增量 |
| Error | 错误 |
| Done | 完成 |

## 命令

```bash
# 构建
dotnet build CozeSdk.sln

# 测试
dotnet test tests/Coze.Sdk.Tests

# 运行示例
dotnet run --project examples/CozeExample
```

## 许可证

MIT License

## 相关链接

- [Coze API 文档](https://www.coze.cn/docs/developer_guides/chat_v3)
- [Coze Java SDK](https://github.com/coze-dev/coze-java)
