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
using AudioBoos.Server.Services.Email;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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
    private readonly JWT _jwt;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailSender _emailSender;

    public AuthController(
        SignInManager<AppUser> signInManager,
        ILogger<AuthController> logger,
        IOptions<JWT> jwtOptions,
        UserManager<AppUser> userManager,
        IEmailSender emailSender) {
        _signInManager = signInManager;
        _logger = logger;
        _jwt = jwtOptions.Value;
        _userManager = userManager;
        _emailSender = emailSender;
    }

    private async Task _sendRegisterEmail(AppUser user) {
        // if email required
        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var callbackUrl = Url.Page(
            "/Account/ConfirmEmail",
            pageHandler: null,
            values: new {area = "Identity", userId = user.Id, code, returnUrl = "TODO:"},
            protocol: Request.Scheme);

        if (_userManager.Options.SignIn.RequireConfirmedAccount) {
            await _emailSender.SendEmailAsync(
                user.Email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }
    }

    private async Task<JwtSecurityToken> _getUserToken(AppUser user) {
        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim> {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        authClaims.AddRange(userRoles.Select(userRole =>
            new Claim(ClaimTypes.Role, userRole))
        );

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));

        var token = new JwtSecurityToken(
            issuer: _jwt.ValidIssuer,
            audience: _jwt.ValidAudience,
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
        return token;
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

            //TODO: _sendRegisterEmail();

            await _signInManager.SignInAsync(user, isPersistent: false);
            var token = await _getUserToken(user);
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

    [HttpPost("login")]
    public async Task<IActionResult> OnLoginAsync([FromBody] LoginDto request) {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password)) {
            return Unauthorized();
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        var token = await _getUserToken(user);
        return Ok(new {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> OnLogoutAsync() {
        if (HttpContext.Request.Cookies.Count > 0) {
            var siteCookies = HttpContext.Request.Cookies.Where(c => c.Key.Equals("AudioBoos.Auth"));
            foreach (var cookie in siteCookies) {
                Response.Cookies.Delete(cookie.Key);
            }
        }

        await HttpContext.SignOutAsync();
        HttpContext.Session.Clear();
        return Ok();
    }
}