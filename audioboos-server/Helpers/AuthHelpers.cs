using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AudioBoos.Data.Models.Settings;
using AudioBoos.Data.Store;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace AudioBoos.Server.Helpers;

public static class AuthHelpers {
    public static ClaimsPrincipal GetPrincipalFromToken(string token,
        TokenValidationParameters tokenValidationParameters) {
        var tokenHandler = new JwtSecurityTokenHandler();
        try {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
            return !_jwtHasValidSecurityAlgorithm(validatedToken) ? null : principal;
        } catch (Exception e) {
            return null;
        }
    }

    private static bool _jwtHasValidSecurityAlgorithm(SecurityToken token) =>
        (token is JwtSecurityToken jwtSecurityToken) &&
        jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
            StringComparison.InvariantCultureIgnoreCase);

    public static async Task<JwtSecurityToken> GetUserToken(AppUser user, JWTOptions jwtOptions) {
        var authClaims = new[] {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            jwtOptions.Issuer,
            jwtOptions.Audience,
            expires: DateTime.UtcNow.Add(jwtOptions.TokenLifetime),
            claims: authClaims,
            signingCredentials: credentials
        );
        return token;
    }

    public static async Task<JwtSecurityToken> GetRefreshToken(AppUser user, JWTOptions jwtOptions) {
        var authClaims = new[] {
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            jwtOptions.Issuer,
            jwtOptions.Audience,
            expires: DateTime.UtcNow.Add(jwtOptions.RefreshTokenLifetime),
            claims: authClaims,
            signingCredentials: credentials
        );
        return token;
    }

    public static void SetCookies(HttpResponse response, string userName, string token, string refreshToken) {
        response.Cookies.Append(Constants.AccessTokenCookie, token,
            new CookieOptions {HttpOnly = true, SameSite = SameSiteMode.Strict});
        response.Cookies.Append(Constants.UsernameCookie, userName,
            new CookieOptions {HttpOnly = true, SameSite = SameSiteMode.Strict});
        response.Cookies.Append(Constants.RefreshTokenCookie, refreshToken,
            new CookieOptions {HttpOnly = true, SameSite = SameSiteMode.Strict});
    }

    public static void RemoveCookies(HttpResponse response) {
        response.Cookies.Append(Constants.SessionCookie, string.Empty,
            new CookieOptions
                {HttpOnly = true, SameSite = SameSiteMode.Strict, Expires = DateTime.UtcNow.AddSeconds(5)});
        response.Cookies.Append(Constants.AccessTokenCookie, string.Empty,
            new CookieOptions
                {HttpOnly = true, SameSite = SameSiteMode.Strict, Expires = DateTime.UtcNow.AddSeconds(5)});
        response.Cookies.Append(Constants.UsernameCookie, string.Empty,
            new CookieOptions
                {HttpOnly = true, SameSite = SameSiteMode.Strict, Expires = DateTime.UtcNow.AddSeconds(5)});
        response.Cookies.Append(Constants.RefreshTokenCookie, string.Empty,
            new CookieOptions
                {HttpOnly = true, SameSite = SameSiteMode.Strict, Expires = DateTime.UtcNow.AddSeconds(5)});
    }
}
