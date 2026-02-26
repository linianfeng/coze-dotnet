using System.Runtime.CompilerServices;

namespace Coze.Sdk.Authentication;

/// <summary>
/// 支持自动 Token 刷新的线程安全 Token 认证。
/// 适用于需要定期刷新 Token 的 OAuth 场景。
/// </summary>
public class RefreshableTokenAuth : Auth, IDisposable
{
    private readonly Func<CancellationToken, Task<(string Token, int ExpiresInSeconds)>> _refreshFunc;
    private readonly TimeSpan _refreshBuffer;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private string _accessToken;
    private DateTimeOffset _expiresAt;
    private bool _disposed;

    /// <summary>
    /// 初始化 <see cref="RefreshableTokenAuth"/> 类的新实例。
    /// </summary>
    /// <param name="initialToken">初始访问 Token（可选，如果为 null 则立即刷新）。</param>
    /// <param name="refreshFunc">刷新 Token 的函数，返回 (Token, ExpiresInSeconds)。</param>
    /// <param name="expiresInSeconds">初始 Token 过期时间（秒）（默认：0，触发立即刷新）。</param>
    /// <param name="refreshBuffer">过期前触发刷新的时间（默认：5 分钟）。</param>
    public RefreshableTokenAuth(
        string? initialToken,
        Func<CancellationToken, Task<(string Token, int ExpiresInSeconds)>> refreshFunc,
        int expiresInSeconds = 0,
        TimeSpan? refreshBuffer = null)
    {
        _accessToken = initialToken ?? string.Empty;
        _refreshFunc = refreshFunc ?? throw new ArgumentNullException(nameof(refreshFunc));
        _refreshBuffer = refreshBuffer ?? TimeSpan.FromMinutes(5);
        _expiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds);
    }

    /// <summary>
    /// 初始化一个在首次使用时获取 Token 的新实例。
    /// </summary>
    /// <param name="refreshFunc">刷新 Token 的函数，返回 (Token, ExpiresInSeconds)。</param>
    /// <param name="refreshBuffer">过期前触发刷新的时间（默认：5 分钟）。</param>
    public RefreshableTokenAuth(
        Func<CancellationToken, Task<(string Token, int ExpiresInSeconds)>> refreshFunc,
        TimeSpan? refreshBuffer = null)
        : this(null, refreshFunc, 0, refreshBuffer)
    {
    }

    /// <summary>
    /// 从 OAuth Token 和简单刷新函数创建 RefreshableTokenAuth。
    /// </summary>
    public static RefreshableTokenAuth Create(
        OAuthToken initialToken,
        Func<string, CancellationToken, Task<OAuthToken>> refreshWithToken,
        TimeSpan? refreshBuffer = null)
    {
        var refreshToken = initialToken.RefreshToken
            ?? throw new ArgumentException("Initial token must have a refresh token", nameof(initialToken));

        return new RefreshableTokenAuth(
            initialToken.AccessToken!,
            async ct =>
            {
                var newToken = await refreshWithToken(refreshToken, ct);
                return (newToken.AccessToken!, newToken.ExpiresIn ?? 3600);
            },
            initialToken.ExpiresIn ?? 3600,
            refreshBuffer);
    }

    /// <inheritdoc/>
    public override string TokenType => "Bearer";

    /// <summary>
    /// 获取当前访问 Token，必要时刷新。线程安全。
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override string GetToken()
    {
        if (!NeedsRefresh()) return _accessToken;
        return GetTokenAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// 异步获取当前访问 Token，必要时刷新。线程安全。
    /// </summary>
    public async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        if (!NeedsRefresh()) return _accessToken;

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            if (!NeedsRefresh()) return _accessToken;

            var (newToken, expiresIn) = await _refreshFunc(cancellationToken);
            _accessToken = newToken;
            _expiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresIn);

            return _accessToken;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// 更新 Token 和过期时间。线程安全。
    /// </summary>
    public async Task UpdateTokenAsync(string token, int expiresInSeconds)
    {
        await _semaphore.WaitAsync();
        try
        {
            _accessToken = token;
            _expiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private bool NeedsRefresh()
    {
        return DateTimeOffset.UtcNow >= _expiresAt.Subtract(_refreshBuffer);
    }

    /// <summary>
    /// 获取一个值，指示 Token 是否已过期。
    /// </summary>
    public bool IsExpired => DateTimeOffset.UtcNow >= _expiresAt;

    /// <summary>
    /// 获取 Token 过期时间。
    /// </summary>
    public DateTimeOffset ExpiresAt => _expiresAt;

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_disposed)
        {
            _semaphore.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
