using System.ComponentModel.DataAnnotations;
using DotNetUnknown.Json;
using DotNetUnknown.Validation;

namespace DotNetUnknown.Tests.Validation;

internal sealed class MxServiceTest
{
    private static readonly Mx009 Mx009 = new()
    {
        UETR = "12345678-1234-1234-1234-000000000000",

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


    private static IEnumerable<BadMx009Request> BadRequestProvider
    {
        get
        {
            var req1 = DeepClone(Mx009);
            req1.MessageId = string.Empty;
            yield return new BadMx009Request(req1, "The MessageId field is required.");

            var req2 = DeepClone(Mx009);
            req2.DebtorAgent = null!;
            yield return new BadMx009Request(req2, "The DebtorAgent field is required.");

            // Validator seems can not validate inner (nested) objects...?
            // var req3 = DeepClone(Mx009);
            // req3.CreditorAgent.BIC = string.Empty;
            // yield return new BadMx009Request(req3, "The BIC field is required.");
        }
    }

    private static Mx009 DeepClone(Mx009 mx009)
    {
        var serializedMx009 = JsonUtils.Serialize(mx009);
        return JsonUtils.Deserialize<Mx009>(serializedMx009) ?? throw new InvalidOperationException();
    }

    [Test]
    public void TestMx009_Success()
    {
        // When
        var mxService = TestProgram.GetScopedService<MxService>();
        // Then
        Assert.That(mxService, Is.Not.Null);
        Assert.DoesNotThrow(() => mxService.Validate(Mx009));
    }

    [TestCaseSource(nameof(BadRequestProvider))]
    public void TestMx009_BadRequest(BadMx009Request badRequest)
    {
        // When
        var mxService = TestProgram.GetScopedService<MxService>();
        // Then
        Assert.That(mxService, Is.Not.Null);
        var validationException = Assert.Throws<ValidationException>(() => mxService.Validate(badRequest.BadMx009));
        Assert.That(validationException.Message, Contains.Substring(badRequest.Reason));
    }

    public record BadMx009Request(Mx009 BadMx009, string Reason);
}