using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetUnknown.Transaction.Entity;

[Table("MASTER_ACCOUNT")]
public class MasterAccount
{
    [Key] [Column("ID")] public int Id { get; init; }

    [Column("ACCOUNT_NUMBER")] public int AccountNumber { get; init; }

    [Column("BALANCE")] public decimal Balance { get; set; }
}