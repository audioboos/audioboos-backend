﻿using System;
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
    public static RefreshToken GetRefreshToken(string ipAddress) {
        using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
        var randomBytes = new byte[64];
        rngCryptoServiceProvider.GetBytes(randomBytes);
        return new RefreshToken {
            Token = Convert.ToBase64String(randomBytes),
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };
    }

    private static ClaimsPrincipal GetPrincipalFromToken(string token,
        TokenValidationParameters tokenValidationParameters) {
        var tokenHandler = new JwtSecurityTokenHandler();
        try {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
            return !_jwtHasValidSecurityAlgorithm(validatedToken) ? null : principal;
        } catch (Exception) {
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
            expires: DateTime.UtcNow.Add(jwtOptions.Lifetime),
            claims: authClaims,
            signingCredentials: credentials
        );
        return token;
    }

    public static void SetCookies(HttpResponse response, string token, string userName, string refreshToken) {
        response.Cookies.Append("X-Access-Token", token,
            new CookieOptions {HttpOnly = true, SameSite = SameSiteMode.Strict});
        response.Cookies.Append("X-Username", userName,
            new CookieOptions {HttpOnly = true, SameSite = SameSiteMode.Strict});
        response.Cookies.Append("X-Refresh-Token", refreshToken,
            new CookieOptions {HttpOnly = true, SameSite = SameSiteMode.Strict});
    }
}