using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetUnknown.Security;

[ApiController]
[Route("security")]
public sealed class SecurityController : ControllerBase
{
    [HttpGet]
    [Route("admin")]
    [Authorize(Roles = "Admin")]
    public IActionResult AdminData()
    {
        return Ok("this is admin user data");
    }
}