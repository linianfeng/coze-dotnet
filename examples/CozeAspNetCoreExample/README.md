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

本示例支持两种认证方式：

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

## 运行示例

1. 确保已安装 .NET 8.0 SDK
2. 配置认证信息
3. 运行以下命令：

```bash
dotnet run
```

服务器将在 `http://localhost:5000` 或 `https://localhost:5001` 上启动

## API 端点

- `GET /swagger` - Swagger UI 界面，用于浏览和测试 API 端点
- `GET /coze/chat?botId={botId}&message={message}` - 发送消息到指定机器人并获取回复
- `GET /health` - 健康检查端点

## 代码结构

- `Program.cs` - 应用程序入口和配置
- `Controllers/CozeController.cs` - Coze API 控制器
- `Controllers/HealthController.cs` - 健康检查控制器
- `Services/I CozeService.cs` 和 `Services/CozeService.cs` - Coze 功能封装服务
- `appsettings.json` - 配置文件