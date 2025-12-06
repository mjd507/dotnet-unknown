using DotNetUnknown.Transaction.Model;
using Microsoft.AspNetCore.Mvc;

namespace DotNetUnknown.Transaction;

[Route("transaction")]
[ApiController]
public class TransactionController(TransactionService transactionService, AuditService auditService) : ControllerBase
{
    [HttpPost]
    [Route("resetAccount")]
    public ActionResult<string> ResetAccount(ResetAccountReq resetAccountReq)
    {
        transactionService.ResetAccount(resetAccountReq.MasterAccNum, resetAccountReq.SubAccNum);
        auditService.ClearAll();
        return Ok();
    }

    [HttpPost]
    [Route("moneyTransfer")]
    public ActionResult<string> MoneyTransfer(MoneyTransferReq transferReq)
    {
        transactionService.MoneyTransfer(transferReq);
        return Ok();
    }

    [HttpPost]
    [Route("accountBalance")]
    public ActionResult<AccountBalanceResp> AccountBalance(AccountBalanceReq accountBalanceReq)
    {
        var (masterAccBalance, subAccBalance) =
            transactionService.FindBalance(accountBalanceReq.MasterAccNum, accountBalanceReq.SubAccNum);
        var accountBalanceResp = new AccountBalanceResp
        {
            MasterAccBalance = masterAccBalance, SubAccBalance = subAccBalance,
            AuditTrail = auditService.GetAuditTrail()
        };
        return Ok(accountBalanceResp);
    }
}