using Microsoft.AspNetCore.Mvc;

namespace BudgetTracker.Host.Controllers // ← namespace must match your Host project
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping() => Ok(new { message = "API is alive!", time = DateTime.UtcNow });
    }
}