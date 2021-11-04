using Microsoft.AspNetCore.Mvc;

namespace AudioBoos.Server.Controllers; 

[ApiController]
[Route("[controller]")]
public class PingController : ControllerBase {
    [HttpGet]
    public IActionResult Ping() {
        return Ok(new {
            Ping = "Pong"
        });
    }
}