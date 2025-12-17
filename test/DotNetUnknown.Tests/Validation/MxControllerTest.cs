using System.Net;
using System.Net.Http.Json;
using DotNetUnknown.Json;
using DotNetUnknown.Validation;

namespace DotNetUnknown.Tests.Validation;

internal sealed class MxControllerTest
{
    private static readonly Mx008 Mx008 = new()
    {
        UETR = "12345678-1234-1234-1234-000000000000",

        MessageId = $"XXXX-{DateTime.UtcNow:yyyyMMdd}-00123",

        CreatedDateTime = DateTime.UtcNow,

        Settlement = new SettlementDetails
        {
            InterbankSettlementDate = new DateTime(2025, 12, 17),
            Currency = "CNY",
            Amount = 50000.00m
        },

        Debtor = new PartyDetail
        {
            Name = "Alice Zhang",
            AccountIBAN = "6222021234567890",
            Address = "123 Nanjing Road, Shanghai, CN"
        },

        Creditor = new PartyDetail
        {
            Name = "TechCorp Solutions",
            AccountIBAN = "GB29WEST12345678901234",
            Address = "45 Canary Wharf, London, UK"
        },

        ChargeBearer = "SHAR",
        RemittanceInformation = "Invoice #INV-2025-001"
    };

    private static IEnumerable<BadMx008Request> BadRequestProvider
    {
        get
        {
            var req1 = DeepClone(Mx008);
            req1.UETR = string.Empty;
            yield return new BadMx008Request(req1, "The UETR field is required.");

            var req2 = DeepClone(Mx008);
            req2.Settlement.Currency = "CNYY";
            yield return new BadMx008Request(req2,
                "The field Currency must be a string with a minimum length of 3 and a maximum length of 3.");

            var req3 = DeepClone(Mx008);
            req3.Settlement.Amount = 0;
            yield return new BadMx008Request(req3, "The field Amount must be between 0.01 and 999999999999.99.");

            var req4 = DeepClone(Mx008);
            req4.Debtor.Name = string.Empty;
            yield return new BadMx008Request(req4, "The Name field is required.");
        }
    }

    private static Mx008 DeepClone(Mx008 mx008)
    {
        var serializedMx008 = JsonUtils.Serialize(mx008);
        return JsonUtils.Deserialize<Mx008>(serializedMx008) ?? throw new InvalidOperationException();
    }

    [Test]
    public async Task TestMx008_Success()
    {
        // Give
        var mx008 = DeepClone(Mx008);
        // When
        var response = await TestProgram.HttpClient.PostAsJsonAsync("mx/008", mx008);
        // Then
        var statusCode = response.StatusCode;
        var body = await response.Content.ReadAsStringAsync();
        await TestContext.Out.WriteLineAsync(body);

        var responseJson = await response.Content.ReadFromJsonAsync<Mx008>();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseJson, Is.Not.Null);
        }
    }

    [TestCaseSource(nameof(BadRequestProvider))]
    public async Task TestMx008_BadRequest(BadMx008Request badRequest)
    {
        // When
        var response = await TestProgram.HttpClient.PostAsJsonAsync("mx/008", badRequest.BadMx008);
        // Then
        var statusCode = response.StatusCode;
        var body = await response.Content.ReadAsStringAsync();
        await TestContext.Out.WriteLineAsync(body);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(body, Contains.Substring(badRequest.Reason));
        }
    }

    public record BadMx008Request(Mx008 BadMx008, string Reason);
}