using DotNetUnknown.DbConfig;
using DotNetUnknown.Transaction.Entity;
using DotNetUnknown.Transaction.Model;

namespace DotNetUnknown.Transaction;

public class TransactionService(AppDbContext dbCtx)
{
    public void MoneyTransfer(MoneyTransferReq transferReq)
    {
        var masterAccount = dbCtx.MasterAccount.Single(acc => acc.AccountNumber.Equals(transferReq.MasterAccNum));
        masterAccount.Balance -= transferReq.Amount;
        var subAccount = dbCtx.SubAccount.Single(acc => acc.AccountNumber.Equals(transferReq.SubAccNum));
        subAccount.Balance += transferReq.Amount;
        dbCtx.SaveChanges();
    }

    /**
     * suppose total balance is 100.
     * master account number is 10000, initial balance is 100,
     * subaccount number is 10001,  initial balance is 0.
     */
    public void ResetAccount(int masterAccNum, int subAccNum)
    {
        var masterAccount = dbCtx.MasterAccount.SingleOrDefault(account => account.AccountNumber.Equals(masterAccNum));
        if (masterAccount == null)
        {
            masterAccount = new MasterAccount { Balance = 100, AccountNumber = masterAccNum };
            dbCtx.MasterAccount.Attach(masterAccount);
        }
        else
        {
            masterAccount.Balance = 100;
        }

        var subAccount = dbCtx.SubAccount.SingleOrDefault(account => account.AccountNumber.Equals(subAccNum));
        if (subAccount == null)
        {
            subAccount = new SubAccount { AccountNumber = subAccNum, ParentId = masterAccNum, Balance = 0 };
            dbCtx.SubAccount.Attach(subAccount);
        }
        else
        {
            subAccount.Balance = 0;
        }

        dbCtx.SaveChanges();
    }

    public (decimal, decimal) FindBalance(int masterAccNum, int subAccNum)
    {
        var masterAccount = dbCtx.MasterAccount.Single(acc => acc.AccountNumber.Equals(masterAccNum));
        var subAccount = dbCtx.SubAccount.Single(acc => acc.AccountNumber.Equals(subAccNum));
        return (masterAccount.Balance, subAccount.Balance);
    }
}