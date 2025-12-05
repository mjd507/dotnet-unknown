using DotNetUnknown.Transaction.Model;
using Microsoft.AspNetCore.Mvc;

namespace DotNetUnknown.Transaction;

[Route("transaction")]
[ApiController]
public class TransactionController(TransactionService transactionService) : ControllerBase
{
    [HttpPost]
    [Route("resetAccount")]
    public ActionResult<string> ResetAccount(ResetAccountReq resetAccountReq)
    {
        transactionService.ResetAccount(resetAccountReq.MasterAccNum, resetAccountReq.SubAccNum);
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
    public ActionResult<string> AccountBalance(AccountBalanceReq accountBalanceReq)
    {
        var (masterAccBalance, subAccBalance) =
            transactionService.FindBalance(accountBalanceReq.MasterAccNum, accountBalanceReq.SubAccNum);
        return Ok("masterAccBalance:" + masterAccBalance + ", subAccBalance:" + subAccBalance);
    }
}