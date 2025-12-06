using DotNetUnknown.Tests.Support;
using DotNetUnknown.Transaction;
using DotNetUnknown.Transaction.Model;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetUnknown.Tests.Transaction;

[TestFixture]
public class TransactionServiceTests : DbTestSupport
{
    private const int MasterAccNum = 10000;
    private const int SubAccNum = 10001;

    [TestCase(true)]
    [TestCase(false)]
    public void TestMoneyTransfer(bool hasException)
    {
        // Data Reset
        using var scope = WebAppFactory.Services.CreateScope();
        var transactionService = scope.ServiceProvider.GetRequiredService<TransactionService>();
        var auditService = scope.ServiceProvider.GetRequiredService<AuditService>();
        transactionService.ResetAccount(MasterAccNum, SubAccNum);
        auditService.ClearAll();
        // Given
        var transferReq = new MoneyTransferReq
            { Amount = 60, MasterAccNum = MasterAccNum, SubAccNum = SubAccNum, FlagException = hasException };
        // When
        try
        {
            transactionService.MoneyTransfer(transferReq);
        }
        catch (System.Exception)
        {
            // ignored
        }

        // Then (use a new scope)
        using var scope2 = WebAppFactory.Services.CreateScope();
        var transactionService2 = scope2.ServiceProvider.GetRequiredService<TransactionService>();
        var auditService2 = scope2.ServiceProvider.GetRequiredService<AuditService>();
        var (masterAccBalance, subAccBalance) =
            transactionService2.FindBalance(transferReq.MasterAccNum, transferReq.SubAccNum);
        var auditTrails = auditService2.GetAuditTrail();
        var expectedMasterAccBalance = hasException ? 100 : 40;
        var expectedSubAccBalance = hasException ? 0 : 60;
        var expectedAuditTrailsCnt = hasException ? 0 : 2;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(masterAccBalance, Is.EqualTo(expectedMasterAccBalance));
            Assert.That(subAccBalance, Is.EqualTo(expectedSubAccBalance));
            Assert.That(auditTrails, Has.Count.EqualTo(expectedAuditTrailsCnt));
        }
    }
}