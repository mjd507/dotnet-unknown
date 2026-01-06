namespace DotNetUnknown.Scheduling.Executor;

public sealed class CrontabOptions : IEquatable<CrontabOptions>
{
    public static CrontabOptions Invalid { get; } = new();

    public string? Crontab { get; init; }

    /// <summary>波动量（毫秒），下次执行时间最多增加不超过此值的量，可防止同一时间执行</summary>
    public int Jitter { get; init; } = 1000;

    /// <summary>
    /// 是否允许分布式执行（需要配置分布式锁）
    /// </summary>
    public bool AllowConcurrentExecution { get; init; }

    /// <summary>
    /// 是否允许本地并行执行（上一次未执行完毕则不执行）
    /// </summary>
    public bool AllowLocalConcurrentExecution { get; init; }

    public bool Equals(CrontabOptions? other) =>
        other is not null && (
            ReferenceEquals(this, other)
            || Crontab == other.Crontab
            && Jitter == other.Jitter
            && AllowConcurrentExecution == other.AllowConcurrentExecution
            && AllowLocalConcurrentExecution == other.AllowLocalConcurrentExecution
        );

    public override bool Equals(object? obj) => Equals(obj as CrontabOptions);

    public override int GetHashCode() =>
        HashCode.Combine(Crontab, Jitter, AllowConcurrentExecution, AllowLocalConcurrentExecution);
}