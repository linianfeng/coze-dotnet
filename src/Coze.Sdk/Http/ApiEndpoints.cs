namespace Coze.Sdk.Http;

/// <summary>
/// API 端点常量。
/// </summary>
internal static class ApiEndpoints
{
    // Chat 端点
    public const string Chat = "/v3/chat";
    public const string ChatRetrieve = "/v3/chat/retrieve";
    public const string ChatCancel = "/v3/chat/cancel";
    public const string ChatSubmitToolOutputs = "/v3/chat/submit_tool_outputs";

    // 消息端点
    public const string MessagesList = "/v3/chat/message/list";

    // Bot 端点
    public const string BotList = "/v1/space/published_bots_list";
    public const string BotRetrieve = "/v1/bot/get_online_info";
    public const string BotCreate = "/v1/bot/create";
    public const string BotUpdate = "/v1/bot/update";
    public const string BotPublish = "/v1/bot/publish";

    // Workflow 端点
    public const string WorkflowRun = "/v1/workflow/run";
    public const string WorkflowRunHistory = "/v1/workflow/runs/{workflow_run_id}/history";

    // 文件端点
    public const string FileUpload = "/v1/files/upload";
    public const string FileRetrieve = "/v1/files/retrieve";

    // 会话端点
    public const string ConversationCreate = "/v1/conversation/create";
    public const string ConversationRetrieve = "/v1/conversation/retrieve";
    public const string ConversationList = "/v1/conversations";

    // 会话消息端点
    public const string ConversationMessageCreate = "/v1/conversation/message/create";
    public const string ConversationMessageList = "/v1/conversation/message/list";
    public const string ConversationMessageRetrieve = "/v1/conversation/message/retrieve";
    public const string ConversationMessageModify = "/v1/conversation/message/modify";
    public const string ConversationMessageDelete = "/v1/conversation/message/delete";

    // 数据集端点
    public const string DatasetCreate = "/v1/datasets/create";
    public const string DatasetList = "/v1/datasets/list";
    public const string DatasetDelete = "/v1/datasets/delete";

    // 音频端点
    public const string AudioSpeech = "/v1/audio/speech";
    public const string AudioTranscription = "/v1/audio/transcriptions";
    public const string AudioVoices = "/v1/audio/voices";
    public const string AudioVoiceClone = "/v1/audio/voices/clone";
    public const string AudioRooms = "/v1/audio/rooms";

    // 工作空间端点
    public const string Workspaces = "/v1/workspaces";

    // OAuth 端点
    public const string OAuthToken = "/api/permission/oauth2/token";
    public const string OAuthDeviceCode = "/api/permission/oauth2/device/code";

    // 模板端点
    public const string TemplateDuplicate = "/v1/templates/{template_id}/duplicate";

    // 变量端点
    public const string Variables = "/v1/variables";

    // 连接器端点
    public const string ConnectorInstall = "v1/connectors/{connector_id}/install";

    // Workflow Chat 端点
    public const string WorkflowChat = "/v1/workflows/chat";

    // 数据集图片端点
    public const string DatasetImages = "v1/datasets/{dataset_id}/images";

    // 商业端点
    public const string CommerceBillTasks = "/v1/commerce/benefit/bill_tasks";
    public const string CommerceBenefitLimitations = "/v1/commerce/benefit/limitations";
}
