using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AudioBoos.Data;
using AudioBoos.Data.Access;
using AudioBoos.Data.Models.DTO;
using AudioBoos.Data.Models.Settings;
using AudioBoos.Data.Store;
using AudioBoos.Server.Helpers;
using AudioBoos.Server.Services.Email;
using Mapster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace AudioBoos.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase {
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ILogger<AuthController> _logger;
    private readonly JWTOptions _jwtOptions;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailSender _emailSender;
    private readonly AudioBoosContext _context;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public AuthController(
        SignInManager<AppUser> signInManager,
        ILogger<AuthController> logger,
        IOptions<JWTOptions> jwtOptions,
        UserManager<AppUser> userManager,
        IEmailSender emailSender,
        AudioBoosContext context,
        TokenValidationParameters tokenValidationParameters) {
        _signInManager = signInManager;
        _logger = logger;
        _jwtOptions = jwtOptions.Value;
        _userManager = userManager;
        _emailSender = emailSender;
        _context = context;
        _tokenValidationParameters = tokenValidationParameters;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResultDto>> OnLogin([FromBody] LoginDto request) {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password)) {
            return Unauthorized();
        }

        await _signInManager.SignInAsync(user, false);
        var token = new JwtSecurityTokenHandler().WriteToken(await AuthHelpers.GetUserToken(user, _jwtOptions));
        var refreshToken =
            new JwtSecurityTokenHandler().WriteToken(await AuthHelpers.GetRefreshToken(user, _jwtOptions));

        AuthHelpers.SetCookies(Response, user.UserName, token, refreshToken);
        user.RefreshTokens.Add(new RefreshToken {
            Token = refreshToken,
            JwtToken = token
        });
        await _context.SaveChangesAsync();

        return Ok(new AuthResultDto(token, refreshToken));
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<IActionResult> OnLogout() {
        await HttpContext.SignOutAsync();
        AuthHelpers.RemoveCookies(HttpContext.Response);
        return Ok();
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResultDto>> OnRefreshTokenAsync() {
        var refreshToken = Request.Cookies[Constants.RefreshTokenCookie];

        var validatedToken = AuthHelpers.GetPrincipalFromToken(refreshToken, _tokenValidationParameters);
        if (validatedToken == null) {
            return Unauthorized();
        }

        var userId = validatedToken.Claims.Single(r => r.Type == ClaimTypes.NameIdentifier).Value;
        var user = await _userManager.FindByIdAsync(userId);
        var storedRefreshToken = _context.RefreshTokens
            .SingleOrDefault(t => t.Token.Equals(refreshToken) && !t.Revoked && t.User.Id == userId);
        if (storedRefreshToken is null) {
            return Unauthorized();
        }

        //get the new token
        var newToken = new JwtSecurityTokenHandler()
            .WriteToken(await AuthHelpers.GetUserToken(user, _jwtOptions));
        var newRefreshToken = new JwtSecurityTokenHandler()
            .WriteToken(await AuthHelpers.GetRefreshToken(user, _jwtOptions));

        AuthHelpers.SetCookies(Response, user.UserName, newToken, newRefreshToken);

        user.RefreshTokens.Add(new RefreshToken {
            Token = newRefreshToken,
            JwtToken = newToken
        });
        storedRefreshToken.Revoked = true;
        _context.Update(storedRefreshToken);
        await _context.SaveChangesAsync();

        return Ok(new AuthResultDto(newToken, newRefreshToken));
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<ProfileDto>> GetProfile() {
        var user = await _userManager.GetUserAsync(this.User);
        if (user is not null) {
            return Ok(user.Adapt<ProfileDto>());
        }

        return Unauthorized();
    }

    [Authorize]
    [HttpGet("p")]
    public async Task<ActionResult<AuthPingDto>> OnPing() {
        return await Task.FromResult(Ok(new AuthPingDto {
            Success = true,
            Message = "pong"
        }));
    }

    [HttpPost("register")]
    public async Task<IActionResult> OnRegister([FromBody] RegisterDto request) {
        if (!ModelState.IsValid) {
            return StatusCode(500);
        }

        var user = new AppUser {UserName = request.Email, Email = request.Email};
        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded) {
            _logger.LogInformation("User created a new account with password");
            await _signInManager.SignInAsync(user, false);
            var token = await AuthHelpers.GetUserToken(user, _jwtOptions);
            return Ok(new {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        foreach (var error in result.Errors) {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return StatusCode(500);
    }
}
