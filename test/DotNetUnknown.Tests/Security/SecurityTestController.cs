using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetUnknown.Tests.Security;

[ApiController]
[Route("security")]
public sealed class SecurityTestController : ControllerBase
{
    [HttpGet]
    [Route("admin")]
    [Authorize(Roles = "Admin")]
    public IActionResult AdminData()
    {
        return Ok("this is admin user data");
    }
}