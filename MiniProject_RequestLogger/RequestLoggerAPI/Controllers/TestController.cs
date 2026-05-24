using Microsoft.AspNetCore.Mvc;

namespace RequestLoggerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("fast")]
    public IActionResult GetFast() => Ok(new { Message = "This was fast" });

    [HttpGet("slow")]
    public async Task<IActionResult> GetSlow()
    {
        await Task.Delay(600); // Trigger slow request warning
        return Ok(new { Message = "This was slow" });
    }
}
