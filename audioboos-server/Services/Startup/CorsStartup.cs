using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AudioBoos.Server.Services.Startup;

public static class CorsStartup {
    private static readonly string policyName = "AudioBoosCors";

    public static IServiceCollection AddAudioBoosCors(this IServiceCollection services, IConfiguration config,
        bool isDevelopment) {
        services.AddCors(options => {
            options.AddPolicy(name: policyName,
                builder => {
                    builder.AllowCredentials();
                    builder.AllowAnyHeader();
                    builder.WithOrigins(
                        config.GetValue<string>("System:WebClientUrl"));
                });
        });

        return services;
    }

    public static IApplicationBuilder UseAudioBoosCors(this IApplicationBuilder app) {
        app.UseCors(policyName);
        return app;
    }
}
