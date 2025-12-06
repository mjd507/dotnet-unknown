using DotNetUnknown.Transaction.Entity;

namespace DotNetUnknown.Transaction.Model;

public struct AccountBalanceResp
{
    public decimal MasterAccBalance { get; init; }
    public decimal SubAccBalance { get; init; }
    public List<AuditTrail> AuditTrail { get; init; }
}