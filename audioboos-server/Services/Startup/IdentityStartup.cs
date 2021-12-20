using System;
using System.Text;
using System.Threading.Tasks;
using AudioBoos.Data;
using AudioBoos.Data.Store;
using AudioBoos.Server.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AudioBoos.Server.Services.Startup;

public static class IdentityStartup {
    public static IServiceCollection AddAudioBoosIdentity(this IServiceCollection services, IConfiguration config,
        bool isDevelopment) {
        var tokenValidationParameters = new TokenValidationParameters {
            ValidIssuer = config["JWTOptions:Issuer"],
            ValidAudience = config["JWTOptions:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWTOptions:Secret"])),
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        services.AddSingleton(tokenValidationParameters);

        services.AddDefaultIdentity<AppUser>(
                options => {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 4;
                })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AudioBoosContext>();
        services.ConfigureApplicationCookie(options => {
            options.Cookie.Name = Constants.SessionCookie;
        });

        services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => {
                options.TokenValidationParameters = tokenValidationParameters;
                options.Events = new JwtBearerEvents {
                    OnMessageReceived = context => {
                        context.Token = context.Request.Cookies[Constants.AccessTokenCookie];
                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }

    public static IApplicationBuilder UseAudioBoosIdentity(this IApplicationBuilder app) {
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}
