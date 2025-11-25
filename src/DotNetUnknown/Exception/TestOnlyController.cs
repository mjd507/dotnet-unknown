using Microsoft.AspNetCore.Mvc;

namespace DotNetUnknown.Exception;

public class TestOnlyController : ControllerBase
{
    [HttpGet("/business_exception")]
    public IActionResult BusinessException()
    {
        throw new BusinessException("this is a business exception");
    }
}