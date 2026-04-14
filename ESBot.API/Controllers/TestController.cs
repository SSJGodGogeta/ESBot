using Microsoft.AspNetCore.Mvc;

namespace ESBot.API.Controllers;

[ApiController]
[Route("v1/healthcheck")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("ESBot API is running");
    }
}
