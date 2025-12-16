using System.ComponentModel.DataAnnotations;

namespace DotNetUnknown.Validation;

public class Mx009 : MxBase
{
    [MaxLength(35)] public string InstructionId { get; set; }

    [Required] public AgentDetail DebtorAgent { get; set; }

    [Required] public AgentDetail CreditorAgent { get; set; }
}

public class AgentDetail
{
    [Required] [MaxLength(11)] public string BIC { get; set; }
}