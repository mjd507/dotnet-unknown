using System.ComponentModel.DataAnnotations;

namespace DotNetUnknown.Validation;

public abstract class MxBase
{
    [Required] public required string UETR { get; set; }

    [Required] [MaxLength(35)] public required string MessageId { get; set; }

    [Required] public required DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;

    [Required] public required SettlementDetails Settlement { get; set; }
}

public class SettlementDetails
{
    [Required] public required DateTime InterbankSettlementDate { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public required string Currency { get; set; }

    [Required]
    [Range(0.01, 999999999999.99)]
    public required decimal Amount { get; set; }
}