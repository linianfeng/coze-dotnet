# Coze ASP.NET Core 示例

此示例展示了如何在 ASP.NET Core Web API 项目中使用 Coze SDK。

## 项目概述

本项目演示了以下内容：

1. 在 ASP.NET Core 项目中配置 Coze SDK
2. 通过依赖注入使用 CozeClient
3. 创建 API 控制器来访问 Coze 功能
4. 支持多种认证方式
5. 集成 Swagger 用于 API 文档和测试

## 准备工作

在运行示例之前，您需要配置认证信息。

### 认证方式

本示例支持两种主要认证方式：

#### 方式一：Token 认证（推荐用于快速测试）

在 `appsettings.json` 中配置：

```json
{
  "Coze": {
    "Token": "your-api-token-here",
    "BaseUrl": "https://api.coze.cn"
  }
}
```

或者通过环境变量设置：

```bash
export COZE__TOKEN="your-api-token-here"
```

#### 方式二：JWT OAuth 认证（推荐用于生产环境）

在 `appsettings.json` 中配置：

```json
{
  "Coze": {
    "Jwt": {
      "ClientId": "your-oauth-client-id",
      "PrivateKey": "-----BEGIN PRIVATE KEY-----\nyour-private-key\n-----END PRIVATE KEY-----",
      "PublicKeyId": "your-public-key-id",
      "BaseUrl": "https://api.coze.cn"
    }
  }
}
```

然后在 `Program.cs` 中启用 JWT OAuth：

```csharp
// 替换 Token 认证配置
builder.Services.AddCozeSdkWithJwtOAuth(builder.Configuration, "Coze:Jwt");
```

## 运行示例

1. 确保已安装 .NET 8.0 SDK
2. 配置认证信息
3. 运行以下命令：

```bash
dotnet run
```

服务器将在 `http://localhost:5000` 或 `https://localhost:5001` 上启动

## API 端点

- `GET /health` - 健康检查端点
- `GET /swagger` - Swagger UI 界面，用于浏览和测试 API 端点
- `GET /coze/chat?botId={botId}&message={message}&userId={userId?}` - 发送消息到指定机器人并获取回复

## 代码结构

- `Program.cs` - 应用程序入口和配置
- `Controllers/CozeController.cs` - Coze API 控制器
- `Controllers/HealthController.cs` - 健康检查控制器
- `Services/ICozeService.cs` 和 `Services/CozeService.cs` - Coze 功能封装服务
- `appsettings.json` - 配置文件

## 实现细节

### 依赖注入配置

在 `Program.cs` 中，我们通过以下方式注册 Coze SDK：

```csharp
// 使用配置文件中的 Token 认证
builder.Services.AddCozeSdk(builder.Configuration);
```

### 服务层封装

`CozeService` 类封装了对 Coze SDK 的调用，提供了简化的方法供控制器使用：

```csharp
public async Task<string> ChatWithBotAsync(string botId, string message, string? userId = null, CancellationToken cancellationToken = default)
```

### 控制器实现

`CozeController` 暴露 REST API 端点，允许通过 HTTP 请求使用 Coze 功能。

### Swagger 集成

本项目集成了 Swashbuckle.AspNetCore 来生成 API 文档和提供交互式 API 测试界面：

1. 在 `Program.cs` 中添加了 Swagger 服务
2. 配置了 Swagger UI 中间件
3. 为控制器添加了适当的 XML 注释（在实际项目中还需要启用 XML 文档生成）

## 扩展示例

您可以轻松地扩展此示例以包含更多功能：

1. 添加其他 Coze 功能（如工作流、文件处理等）
2. 添加认证和授权
3. 实现更复杂的业务逻辑
4. 添加缓存和错误处理
5. 为 API 控制器添加 XML 注释以改进 Swagger 文档

## 故障排除

- 如果遇到认证错误，请检查您的 API 密钥或 OAuth 配置
- 如果遇到 SSL/TLS 错误，请确认 BaseUrl 设置正确
- 检查网络连接以确保可以访问 Coze API
- 如果 Swagger 不显示，请确认应用在 Development 环境中运行，或修改 Program.cs 中的条件逻辑