using DotNetUnknown.Exception;
using Microsoft.AspNetCore.Mvc;

namespace DotNetUnknown.Tests.Exception;

public sealed class ExceptionTestController : ControllerBase
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