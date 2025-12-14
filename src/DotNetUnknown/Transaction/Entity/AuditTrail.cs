using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetUnknown.Transaction.Entity;

[Table("AUDIT_TRAIL")]
public class AuditTrail
{
    [Key] [Column("ID")] public int Id { get; init; }
    [Column("MAKER")] public required string Maker { get; init; }
    [Column("CHECKER")] public required string Checker { get; init; }
    [Column("COMMENT")] public required string Comment { get; init; }
    [Column("CREATED_AT")] public DateTime CreatedAt { get; init; }
}