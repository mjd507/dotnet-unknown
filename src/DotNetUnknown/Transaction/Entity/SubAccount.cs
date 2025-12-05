namespace DotNetUnknown.Transaction.Entity;

public class SubAccount
{
    public int Id { get; init; }
    public int ParentId { get; init; }
    public int AccountNumber { get; init; }
    public decimal Balance { get; set; }
}