using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data.Interfaces;
using AudioBoos.Data.Store;
using Microsoft.AspNetCore.Mvc;

namespace AudioBoos.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class DebugController : ControllerBase {
    [HttpGet]
    [Produces("text/plain")]
    public async Task<IActionResult> Ping() {
        return await Task.FromResult(Ok("Pong"));
    }
}
