using System.Transactions;
using DotNetUnknown.DbConfig;
using DotNetUnknown.Transaction.Entity;
using Microsoft.EntityFrameworkCore;

namespace DotNetUnknown.Transaction;

public class AuditService(AppDbContext dbCtx)
{
    public void ClearAll()
    {
        dbCtx.AuditTrail.ExecuteDelete();
        dbCtx.SaveChanges();
    }

    public void InsertAudit(string maker, string checker, string comment)
    {
        var auditTrail = new AuditTrail() { Maker = maker, Comment = comment, Checker = checker };
        dbCtx.AuditTrail.Add(auditTrail);
        dbCtx.SaveChanges();
    }

    public void InsertAuditInTx(string maker, string checker, string comment)
    {
        using var scope = new TransactionScope(TransactionScopeOption.RequiresNew);
        InsertAudit(maker, checker, comment);
        scope.Complete();
    }

    public List<AuditTrail> GetAuditTrail()
    {
        return dbCtx.AuditTrail.ToList();
    }
}