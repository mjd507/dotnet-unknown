using System.ComponentModel.DataAnnotations;

namespace DotNetUnknown.Validation;

public class Mx008 : MxBase
{
    [Required] public PartyDetail Debtor { get; set; }

    [Required] public PartyDetail Creditor { get; set; }

    [Required] [MaxLength(4)] public string ChargeBearer { get; set; }

    [MaxLength(140)] public string RemittanceInformation { get; set; }
}

public class PartyDetail
{
    [Required] [MaxLength(140)] public string Name { get; set; }

    [MaxLength(34)] public string AccountIBAN { get; set; }

    public string Address { get; set; }
}