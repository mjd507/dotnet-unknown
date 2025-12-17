using System.ComponentModel.DataAnnotations;

namespace DotNetUnknown.Validation;

public class Mx008 : MxBase
{
    [Required] public required PartyDetail Debtor { get; set; }

    [Required] public required PartyDetail Creditor { get; set; }

    [Required] [MaxLength(4)] public required string ChargeBearer { get; set; }

    [MaxLength(140)] public string? RemittanceInformation { get; set; }
}

public class PartyDetail
{
    [Required] [MaxLength(140)] public required string Name { get; set; }

    [Required] [MaxLength(34)] public required string AccountIBAN { get; set; }

    public string? Address { get; set; }
}