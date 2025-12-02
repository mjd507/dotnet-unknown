using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace DotNetUnknown.ApiVersioning;

[ApiController]
[Route("ApiVersioning")]
public sealed class ApiVersioningController : ControllerBase
{
    [HttpGet]
    [Route("v-1-0")]
    [ApiVersion("1.0")]
    public IActionResult RequiredVersionOne()
    {
        return Ok(new { Title = "version 1.0 data" });
    }
}