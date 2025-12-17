using System.ComponentModel.DataAnnotations;

namespace DotNetUnknown.Validation;

public class Mx009 : MxBase
{
    [MaxLength(35)] public required string InstructionId { get; set; }

    [Required] public required AgentDetail DebtorAgent { get; set; }

    [Required] public required AgentDetail CreditorAgent { get; set; }
}

public class AgentDetail
{
    [Required] [MaxLength(11)] public required string BIC { get; set; }
}