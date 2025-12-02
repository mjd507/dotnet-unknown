using Microsoft.AspNetCore.Mvc;

namespace DotNetUnknown.Exception;

public sealed class ExceptionController : ControllerBase
{
    [HttpGet("/business_exception")]
    public IActionResult BusinessException()
    {
        throw new BusinessException("this is a business exception");
    }

    [HttpGet("/system_exception")]
    public IActionResult SystemException()
    {
        throw new System.Exception("this is a system exception");
    }
}