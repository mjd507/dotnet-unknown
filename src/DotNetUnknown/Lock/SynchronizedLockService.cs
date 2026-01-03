namespace DotNetUnknown.Lock;

public sealed class SynchronizedLockService(ILogger<SynchronizedLockService> logger)
{
    private readonly System.Threading.Lock _balanceLock = new();
    private decimal _balance = 100;

    public void Debit(decimal amount)
    {
        lock (_balanceLock)
        {
            _balance -= amount;
            logger.LogInformation($"Debit from {_balance + amount} to {_balance}");
        }
    }

    public decimal GetBalance()
    {
        lock (_balanceLock)
        {
            return _balance;
        }
    }
}