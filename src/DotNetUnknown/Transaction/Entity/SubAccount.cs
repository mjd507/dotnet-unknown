using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetUnknown.Transaction.Entity;

[Table("SUB_ACCOUNT")]
public class SubAccount
{
    [Column("ID")] public int Id { get; init; }
    [Column("PARENT_ID")] public int ParentId { get; init; }
    [Column("ACCOUNT_NUMBER")] public int AccountNumber { get; init; }
    [Column("BALANCE")] public decimal Balance { get; set; }
}