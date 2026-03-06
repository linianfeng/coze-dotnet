# Coze Vue Example

基于 Vue 3 + Element Plus 的 Coze 聊天前端示例，与 CozeAspNetCoreExample 后端配合使用。

## 技术栈

- **框架**: Vue 3 + Vite
- **语言**: TypeScript
- **UI**: Element Plus
- **状态管理**: Pinia (支持持久化)
- **HTTP**: Fetch API
- **工具**: marked (Markdown 渲染)

## 项目结构

```
src/
├── api/
│   ├── coze.ts            # Coze API 封装
│   └── types.ts           # API 类型定义
├── components/
│   ├── ChatPanel.vue      # 聊天面板
│   ├── MessageItem.vue    # 消息项
│   ├── BotSelector.vue    # Bot 选择器
│   ├── SessionList.vue    # 会话列表
│   └── FileUploader.vue   # 文件上传
├── stores/
│   ├── chat.ts            # 聊天状态
│   ├── bot.ts             # Bot 配置
│   └── session.ts         # 会话管理
├── views/
│   └── HomeView.vue       # 主页面
├── types/
│   └── index.ts           # 类型定义
├── App.vue
└── main.ts
```

## 功能特性

- Bot 管理：添加、选择 Bot
- 会话管理：创建、切换、删除会话
- 聊天功能：发送消息、流式响应
- 文件上传：支持多种文件格式
- 会话持久化：刷新后保留历史会话
- Markdown 渲染：消息支持 Markdown 格式

## 快速开始

### 1. 启动后端

```bash
cd examples/CozeAspNetCoreExample
dotnet run
```

后端将在 http://localhost:5000 启动。

### 2. 配置 Bot ID

编辑 `appsettings.json`，设置你的 Coze Token：

```json
{
  "Coze": {
    "Token": "your-coze-token",
    "BaseUrl": "https://api.coze.cn"
  }
}
```

### 3. 启动前端

```bash
cd examples/CozeVueExample
npm install
npm run dev
```

前端将在 http://localhost:5173 启动。

### 4. 访问应用

打开浏览器访问 http://localhost:5173

## 使用说明

1. 首次使用时，点击右上角的 "+" 按钮添加 Bot
2. 输入 Bot ID 和名称（Bot ID 可在 Coze 平台获取）
3. 在左侧会话栏点击 "新建" 创建新会话
4. 在底部输入框输入消息并发送

## API 端点

| 端点 | 方法 | 描述 |
|------|------|------|
| `/coze/chat` | GET | 非流式聊天 |
| `/coze/chat/stream` | GET | 流式聊天 (SSE) |
| `/api/file/upload` | POST | 文件上传 |
| `/api/file/download/{id}` | GET | 文件下载 |
| `/health` | GET | 健康检查 |

## 环境变量

- `VITE_API_BASE_URL`: 后端 API 地址

开发环境默认: `http://localhost:5000`
生产环境默认: 相对路径（同源）

## 构建生产版本

```bash
npm run build
```

构建产物将生成在 `dist/` 目录。

## 注意事项

1. 确保后端服务已启动并可访问
2. CORS 已在后端配置中允许 `localhost:5173`
3. 会话和 Bot 配置存储在 localStorage 中
4. 文件上传大小限制为 10MB

## License

MIT
