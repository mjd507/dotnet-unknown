namespace DotNetUnknown.Transaction.Entity;

public class AuditTrail
{
    public int Id { get; init; }
    public required string Maker { get; init; }
    public required string Checker { get; init; }
    public required string Comment { get; init; }
}