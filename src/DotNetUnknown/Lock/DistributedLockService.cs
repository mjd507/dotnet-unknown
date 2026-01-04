using Medallion.Threading;

namespace DotNetUnknown.Lock;

public sealed class DistributedLockService(
    IDistributedLockProvider distributedLockProvider,
    ILogger<DistributedLockService> logger)
{
    private readonly IDistributedLock _distributedLock =
        distributedLockProvider.CreateLock("MY_DISTRIBUTED_LOCK_FOR_ACCOUNT_BALANCE");

    private decimal _balance = 100;

    public void Debit(decimal amount)
    {
        using (_distributedLock.Acquire())
        {
            _balance -= amount;
            logger.LogInformation($"Debit from {_balance + amount} to {_balance}");
        }
    }

    public decimal GetBalance()
    {
        using (_distributedLock.Acquire())
        {
            return _balance;
        }
    }
}