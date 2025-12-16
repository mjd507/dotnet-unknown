using DotNetUnknown.Validation;

namespace DotNetUnknown.Tests.Validation;

internal sealed class MxServiceTest
{
    private readonly Mx009 _mx009 = new()
    {
        UETR = Guid.NewGuid(),

        MessageId = $"BANK-SETTLE-{DateTime.UtcNow:yyyyMMdd}-999",

        CreatedDateTime = DateTime.UtcNow,

        Settlement = new SettlementDetails
        {
            InterbankSettlementDate = new DateTime(2025, 12, 17),
            Currency = "CNY",
            Amount = 50000.00m
        },

        InstructionId = $"XXXX-{DateTime.UtcNow:yyyyMMdd}-00123",

        DebtorAgent = new AgentDetail
        {
            BIC = "BKCHCNBJXXX"
        },

        CreditorAgent = new AgentDetail
        {
            BIC = "SCBLGBLXXXX"
        }
    };

    [Test]
    public void ValidateTest()
    {
        var mxService = TestProgram.GetScopedService<MxService>();
        Assert.That(mxService, Is.Not.Null);
        Assert.DoesNotThrow(() => mxService.Validate(_mx009));
    }
}