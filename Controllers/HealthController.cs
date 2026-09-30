using Microsoft.AspNetCore.Mvc;

namespace Alumni.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { status = "ok" });
    }

    [HttpPost]
    public IActionResult Post()
    {
        return Ok(new { status = "ok" });
    }
}
