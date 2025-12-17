using Microsoft.AspNetCore.Mvc;

namespace DotNetUnknown.Validation;

[ApiController]
[Route("mx")]
public class MxController : ControllerBase
{
    [HttpPost]
    [Route("008")]
    public IActionResult Mx008(Mx008 mx008)
    {
        return Ok(mx008);
    }
}