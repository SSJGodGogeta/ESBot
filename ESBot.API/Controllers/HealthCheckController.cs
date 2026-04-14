using Microsoft.AspNetCore.Mvc;

namespace ESBot.API.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("ESBot API is running");
    }
}
