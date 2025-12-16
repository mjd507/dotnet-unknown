using System.Net;
using System.Net.Http.Json;
using DotNetUnknown.Validation;

namespace DotNetUnknown.Tests.Validation;

internal sealed class MxControllerTest
{
    private readonly Mx008 _mx008 = new()
    {
        UETR = Guid.NewGuid(),

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

    [Test]
    public async Task TestMx008()
    {
        // When
        var responseMessage = await TestProgram.HttpClient.PostAsJsonAsync("mx/008", _mx008);
        // Then
        var statusCode = responseMessage.StatusCode;
        await TestContext.Out.WriteLineAsync(await responseMessage.Content.ReadAsStringAsync());

        var responseJson = await responseMessage.Content.ReadFromJsonAsync<Mx008>();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(statusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseJson, Is.Not.Null);
        }
    }
}