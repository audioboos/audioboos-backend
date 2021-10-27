using AudioBoos.Data.Models.Settings;
using AudioBoos.Server.Services.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AudioBoos.Server.Services.Startup; 

public static class OptionsBinder {
    public static IServiceCollection AddAudioBoosOptions(this IServiceCollection services, IConfiguration config) {
        services.AddOptions();
        services.Configure<SystemSettings>(config.GetSection("System"));
        services.Configure<EmailOptions>(config.GetSection("EmailOptions"));
        services.Configure<JobOptions>(config.GetSection("JobOptions"));
        services.Configure<JWT>(config.GetSection("JWT"));

        return services;
    }
}