using Coze.Sdk.Authentication;
using Coze.Sdk.Clients;
using Coze.Sdk.Http;

namespace Coze.Sdk;

/// <summary>
/// Coze SDK 的主入口接口。
/// 对应 Java SDK 中的 CozeAPI.java。
/// </summary>
public interface ICozeClient : IDisposable
{
    /// <summary>
    /// 获取聊天客户端。
    /// </summary>
    IChatClient Chat { get; }

    /// <summary>
    /// 获取机器人客户端。
    /// </summary>
    IBotsClient Bots { get; }

    /// <summary>
    /// 获取工作流客户端。
    /// </summary>
    IWorkflowClient Workflows { get; }

    /// <summary>
    /// 获取文件客户端。
    /// </summary>
    IFileClient Files { get; }

    /// <summary>
    /// 获取会话客户端。
    /// </summary>
    IConversationClient Conversations { get; }

    /// <summary>
    /// 获取数据集客户端。
    /// </summary>
    IDatasetClient Datasets { get; }

    /// <summary>
    /// 获取音频客户端。
    /// </summary>
    IAudioClient Audio { get; }

    /// <summary>
    /// 获取工作空间客户端。
    /// </summary>
    IWorkspaceClient Workspaces { get; }

    /// <summary>
    /// 获取模板客户端。
    /// </summary>
    ITemplateClient Templates { get; }

    /// <summary>
    /// 获取变量客户端。
    /// </summary>
    IVariablesClient Variables { get; }

    /// <summary>
    /// 获取连接器客户端。
    /// </summary>
    IConnectorClient Connectors { get; }

    /// <summary>
    /// 获取商业客户端。
    /// </summary>
    ICommerceClient Commerce { get; }
}

/// <summary>
/// Coze SDK 客户端的主要实现。
/// </summary>
public class CozeClient : ICozeClient
{
    private readonly CozeHttpClient _httpClient;
    private readonly CozeOptions _options;
    private bool _disposed;

    // 延迟初始化的服务客户端
    private readonly Lazy<IChatClient> _chat;
    private readonly Lazy<IBotsClient> _bots;
    private readonly Lazy<IWorkflowClient> _workflows;
    private readonly Lazy<IFileClient> _files;
    private readonly Lazy<IConversationClient> _conversations;
    private readonly Lazy<IDatasetClient> _datasets;
    private readonly Lazy<IAudioClient> _audio;
    private readonly Lazy<IWorkspaceClient> _workspaces;
    private readonly Lazy<ITemplateClient> _templates;
    private readonly Lazy<IVariablesClient> _variables;
    private readonly Lazy<IConnectorClient> _connectors;
    private readonly Lazy<ICommerceClient> _commerce;

    /// <summary>
    /// 初始化 <see cref="CozeClient"/> 类的新实例。
    /// </summary>
    /// <param name="options">SDK 配置选项。</param>
    public CozeClient(CozeOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _httpClient = new CozeHttpClient(options);

        _chat = new Lazy<IChatClient>(() => new ChatClient(_httpClient));
        _bots = new Lazy<IBotsClient>(() => new BotsClient(_httpClient));
        _workflows = new Lazy<IWorkflowClient>(() => new WorkflowClient(_httpClient));
        _files = new Lazy<IFileClient>(() => new FileClient(_httpClient));
        _conversations = new Lazy<IConversationClient>(() => new ConversationClient(_httpClient));
        _datasets = new Lazy<IDatasetClient>(() => new DatasetClient(_httpClient));
        _audio = new Lazy<IAudioClient>(() => new AudioClient(_httpClient));
        _workspaces = new Lazy<IWorkspaceClient>(() => new WorkspaceClient(_httpClient));
        _templates = new Lazy<ITemplateClient>(() => new TemplateClient(_httpClient));
        _variables = new Lazy<IVariablesClient>(() => new VariablesClient(_httpClient));
        _connectors = new Lazy<IConnectorClient>(() => new ConnectorClient(_httpClient));
        _commerce = new Lazy<ICommerceClient>(() => new CommerceClient(_httpClient));
    }

    /// <summary>
    /// 使用 Token 初始化 <see cref="CozeClient"/> 类的新实例。
    /// </summary>
    /// <param name="token">API Token。</param>
    /// <param name="baseUrl">可选的基础 URL（默认为 https://api.coze.cn）。</param>
    public CozeClient(string token, string? baseUrl = null)
        : this(new CozeOptions
        {
            Auth = new TokenAuth(token),
            BaseUrl = baseUrl ?? "https://api.coze.cn"
        })
    {
    }

    /// <inheritdoc/>
    public IChatClient Chat => _chat.Value;

    /// <inheritdoc/>
    public IBotsClient Bots => _bots.Value;

    /// <inheritdoc/>
    public IWorkflowClient Workflows => _workflows.Value;

    /// <inheritdoc/>
    public IFileClient Files => _files.Value;

    /// <inheritdoc/>
    public IConversationClient Conversations => _conversations.Value;

    /// <inheritdoc/>
    public IDatasetClient Datasets => _datasets.Value;

    /// <inheritdoc/>
    public IAudioClient Audio => _audio.Value;

    /// <inheritdoc/>
    public IWorkspaceClient Workspaces => _workspaces.Value;

    /// <inheritdoc/>
    public ITemplateClient Templates => _templates.Value;

    /// <inheritdoc/>
    public IVariablesClient Variables => _variables.Value;

    /// <inheritdoc/>
    public IConnectorClient Connectors => _connectors.Value;

    /// <inheritdoc/>
    public ICommerceClient Commerce => _commerce.Value;

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient.Dispose();
            _disposed = true;
        }
    }
}
