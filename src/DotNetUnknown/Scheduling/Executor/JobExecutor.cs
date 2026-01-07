using System.Diagnostics.CodeAnalysis;
using Medallion.Threading;
using Microsoft.Extensions.Options;
using NCrontab;

namespace DotNetUnknown.Scheduling.Executor;

public abstract partial class JobExecutor : BackgroundService
{
    private static readonly CrontabSchedule ErrorSchedule = CrontabSchedule.Parse("* * * * *");
    private readonly IDisposable? _disposable;
    private readonly ILogger<JobExecutor> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private State _state = new();

    protected JobExecutor(ILogger<JobExecutor> logger, IServiceScopeFactory scopeFactory,
        IOptionsMonitor<Dictionary<string, CrontabOptions>> optionsMonitor)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _disposable = optionsMonitor.OnChange(CrontabChanged);

        CrontabChanged(optionsMonitor.CurrentValue);
    }

    protected abstract string Name { get; }

    private void CrontabChanged(Dictionary<string, CrontabOptions> optionsMonitor)
    {
        if (!optionsMonitor.TryGetValue(Name, out var crontab))
        {
            MissingCrontab(_logger, Name);

            if (_state.HasSchedule()) _state = new State();

            return;
        }

        if (_state.HasSchedule() && _state.Options.Equals(crontab)) return;

        if (string.IsNullOrWhiteSpace(crontab.Crontab))
        {
            CrontabError(_logger, Name, crontab.Crontab ?? string.Empty);

            if (_state.HasSchedule()) _state = new State();

            return;
        }

        var schedule = CrontabSchedule.TryParse(crontab.Crontab, new() { IncludingSeconds = true },
            crontabSchedule => crontabSchedule,
            ex =>
            {
                CrontabError(_logger, Name, crontab.Crontab ?? string.Empty, ex());

                return ErrorSchedule;
            });

        if (schedule != ErrorSchedule)
        {
            var state = Interlocked.Exchange(ref _state, new State(schedule, crontab));

            if (!state.HasSchedule()) state.TaskCompletionSource.TrySetResult();
        }
        else if (_state.HasSchedule()) _state = new State();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using (stoppingToken.AsCompletedTask(out var task))
            while (!stoppingToken.IsCancellationRequested)
            {
                var nextOccurrence = GetNextOccurrence();
                if (await Task.WhenAny(nextOccurrence, task) == task) return;

                if (await nextOccurrence.ConfigureAwait(false) is { } options)
                    await ExecuteOnceAsync(options, stoppingToken);
            }
    }

    private async Task ExecuteOnceAsync(CrontabOptions options, CancellationToken cancellationToken)
    {
        var scope = _scopeFactory.CreateAsyncScope();

        await using (scope.ConfigureAwait(false))
            await ExecuteOnceAsync(options, scope.ServiceProvider, cancellationToken).ConfigureAwait(false);
    }

    public abstract Task<bool?> ExecuteOnceAsync(CrontabOptions options, IServiceProvider provider,
        CancellationToken cancellationToken);

    private async Task<CrontabOptions?> GetNextOccurrence()
    {
        var state = _state;
        if (!state.HasSchedule())
        {
            await state.TaskCompletionSource.Task.ConfigureAwait(false);

            return null;
        }

        var now = DateTime.Now;

        var time = state.Schedule.GetNextOccurrence(now);
        if (state.Options.Jitter > 0)
            time = time.AddMilliseconds(Random.Shared.Next(state.Options.Jitter));

        NextExecutionTime(_logger, Name, time);

        var span = time - now;

        await (span.TotalSeconds < 1 ? Task.Delay(1000) : Task.Delay(span)).ConfigureAwait(false);

        return state.Options;
    }

    public override void Dispose()
    {
        _disposable?.Dispose();

        base.Dispose();
    }

    [LoggerMessage(1, LogLevel.Information, "下次执行 {Job} 时间为：{Time:yyyy-MM-dd HH:mm:ss.fff}")]
    protected static partial void NextExecutionTime(ILogger logger, string job, DateTime time);

    [LoggerMessage(2, LogLevel.Error, "执行 {Job} 失败")]
    protected static partial void ExecuteFailed(ILogger logger, string job, System.Exception ex);

    [LoggerMessage(3, LogLevel.Information, "{Job} 正在其它节点执行")]
    protected static partial void ConcurrentExecution(ILogger logger, string job);

    [LoggerMessage(4, LogLevel.Error, "没有读取到 {Job} 的 Crontab 配置")]
    protected static partial void MissingCrontab(ILogger logger, string job);

    [LoggerMessage(5, LogLevel.Error, "{Job} 的 Crontab 表达式 '{Crontab}' 不支持")]
    protected static partial void CrontabError(ILogger logger, string job, string crontab, System.Exception? ex = null);

    [LoggerMessage(6, LogLevel.Information, "{Job} 正在本地执行, skipping")]
    protected static partial void LocalConcurrentExecution(ILogger logger, string job);

    protected sealed class State
    {
        public State(CrontabSchedule schedule, CrontabOptions options)
        {
            Schedule = schedule;
            Options = options;
        }

        public State() => TaskCompletionSource = new TaskCompletionSource();

        public CrontabSchedule? Schedule { get; }

        public CrontabOptions? Options { get; }

        public TaskCompletionSource? TaskCompletionSource { get; }

        [MemberNotNullWhen(true, nameof(Schedule), nameof(Options))]
        [MemberNotNullWhen(false, nameof(TaskCompletionSource))]
        public bool HasSchedule() => Schedule != null;
    }
}

public sealed class JobExecutor<T>(
    ILogger<JobExecutor> logger,
    IServiceScopeFactory scopeFactory,
    IOptionsMonitor<Dictionary<string, CrontabOptions>> optionsMonitor)
    : JobExecutor(logger, scopeFactory, optionsMonitor) where T : IJob
{
    private readonly ILogger<JobExecutor> _logger = logger;

    private int _isRunning;

    protected override string Name => typeof(T).Name;

    public override async Task<bool?> ExecuteOnceAsync(CrontabOptions options, IServiceProvider provider,
        CancellationToken cancellationToken)
    {
        if (!options.AllowLocalConcurrentExecution && Interlocked.CompareExchange(ref _isRunning, 1, 0) != 0)
        {
            LocalConcurrentExecution(_logger, Name);
            return null;
        }

        IDistributedSynchronizationHandle? handle = default;
        if (!options.AllowConcurrentExecution)
        {
            try
            {
                handle = await provider.GetRequiredService<IDistributedLockProvider>().CreateLock(Name)
                    .TryAcquireAsync(cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (System.Exception ex)
            {
                Interlocked.Exchange(ref _isRunning, 0);

                ExecuteFailed(_logger, Name, ex);

                return false;
            }

            if (handle == null)
            {
                ConcurrentExecution(_logger, Name);

                return null;
            }
        }

        try
        {
            await provider.GetRequiredService<T>().ExecuteAsync(cancellationToken).ConfigureAwait(false);

            Interlocked.Exchange(ref _isRunning, 0);
        }
        catch (System.Exception ex)
        {
            Interlocked.Exchange(ref _isRunning, 0);

            ExecuteFailed(_logger, Name, ex);

            return false;
        }
        finally
        {
            if (handle != null) await handle.DisposeAsync().ConfigureAwait(false);
        }

        return true;
    }
}