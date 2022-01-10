using System.Threading.Tasks;
using AudioBoos.Data.Models.DTO;
using AudioBoos.Data.Store;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AudioBoos.Server.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ProfileController : ControllerBase {
    private readonly UserManager<AppUser> _userManager;

    public ProfileController(UserManager<AppUser> userManager) {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<ProfileDto>> Get() {
        var user = await _userManager.GetUserAsync(this.User);
        if (user is not null) {
            return Ok(user.Adapt<ProfileDto>());
        }

        return Unauthorized();
    }

    [HttpPatch]
    public async Task<ActionResult<ProfileDto>> Patch([FromBody] ProfileDto newProfile) {
        var user = await _userManager.GetUserAsync(this.User);
        if (user is null) {
            return Unauthorized();
        }

        user.FirstName = newProfile.FirstName;
        user.SecondName = newProfile.SecondName;
        user.Photo = newProfile.Photo;
        await _userManager.UpdateAsync(user);
        return Ok(newProfile);
    }
}
