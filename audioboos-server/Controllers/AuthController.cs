using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AudioBoos.Data.Models.DTO;
using AudioBoos.Data.Models.Settings;
using AudioBoos.Data.Store;
using AudioBoos.Server.Helpers;
using AudioBoos.Server.Services.Email;
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
    private readonly TokenValidationParameters _tokenValidationParameters;

    public AuthController(
        SignInManager<AppUser> signInManager,
        ILogger<AuthController> logger,
        IOptions<JWTOptions> jwtOptions,
        UserManager<AppUser> userManager,
        IEmailSender emailSender,
        TokenValidationParameters tokenValidationParameters) {
        _signInManager = signInManager;
        _logger = logger;
        _jwtOptions = jwtOptions.Value;
        _userManager = userManager;
        _emailSender = emailSender;
        _tokenValidationParameters = tokenValidationParameters;
    }

    [HttpPost("login")]
    public async Task<IActionResult> OnLoginAsync([FromBody] LoginDto request) {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password)) {
            return Unauthorized();
        }

        await _signInManager.SignInAsync(user, false);
        var token = await AuthHelpers.GetUserToken(user, _jwtOptions);
        var tokenText = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = AuthHelpers.GetRefreshToken(HttpContext.Connection.RemoteIpAddress.ToString());

        AuthHelpers.SetCookies(Response, tokenText, user.UserName, refreshToken.Token);

        return Ok();
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<ProfileDto>> GetProfile() {
        var user = await _userManager.GetUserAsync(this.User);
        if (user is not null) {
            return Ok(new ProfileDto(user.UserName));
        }

        return Unauthorized();
    }

    [Authorize]
    [HttpGet("p")]
    public async Task<ActionResult<AuthPingDto>> OnPingAsync() {
        return await Task.FromResult(Ok(new AuthPingDto {
            Success = true,
            Message = "pong"
        }));
    }

    [HttpPost("register")]
    public async Task<IActionResult> OnRegisterAsync([FromBody] RegisterDto request) {
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

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> OnLogoutAsync() {
        await HttpContext.SignOutAsync();
        AuthHelpers.RemoveCookies(HttpContext.Response);
        return Ok();
    }
}
