namespace DotNetUnknown.Transaction;

public static class TransactionExtension
{
    extension(IServiceCollection serviceCollection)
    {
        public void RegisterTransactionServices()
        {
            serviceCollection.AddScoped<TransactionService>();
        }
    }
}