namespace DotNetUnknown.Transaction.Model;

public class MoneyTransferReq
{
    public int MasterAccNum { get; set; }
    public int SubAccNum { get; set; }
    public decimal Amount { get; set; }
    public bool FlagException { get; set; }
}