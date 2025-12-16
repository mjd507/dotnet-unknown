using System.ComponentModel.DataAnnotations;

namespace DotNetUnknown.Validation;

public abstract class MxBase
{
    [Required] public Guid UETR { get; set; }

    [Required] [MaxLength(35)] public string MessageId { get; set; }

    [Required] public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;

    [Required] public SettlementDetails Settlement { get; set; }
}

public class SettlementDetails
{
    [Required] public DateTime InterbankSettlementDate { get; set; }

    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; }

    [Required]
    [Range(0.01, 999999999999.99)]
    public decimal Amount { get; set; }
}