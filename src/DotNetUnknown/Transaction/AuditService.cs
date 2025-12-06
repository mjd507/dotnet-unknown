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
        // if this inner transaction be called inside an outer transaction, will throw below exception:
        // The connection is already in a transaction and cannot participate in another transaction.
        using var transaction = dbCtx.Database.BeginTransaction();
        try
        {
            var auditTrail = new AuditTrail() { Maker = maker, Comment = comment, Checker = checker };
            dbCtx.AuditTrail.Add(auditTrail);
            dbCtx.SaveChanges();
            transaction.Commit();
        }
        catch (System.Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    public List<AuditTrail> GetAuditTrail()
    {
        return dbCtx.AuditTrail.ToList();
    }
}