namespace DotNetUnknown.Transaction.Entity;

public class MasterAccount
{
    public int Id { get; init; }
    public int AccountNumber { get; init; }
    public decimal Balance { get; set; }
}