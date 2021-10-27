using System;
using AudioBoos.Server.Services.AudioLookup;
using Microsoft.Extensions.DependencyInjection;

namespace AudioBoos.Server.Services.Startup; 

public static class HttpClientsStartup {
    // private const string USER_AGENT = "AudioBoos-API/0.1 +https://audioboos.com";
    private const string USER_AGENT = "AudioBoos-API/0.1";

    public static IServiceCollection AddAudioBoosHttpClients(this IServiceCollection services) {
        // services.AddTransient<IAudioLookupService, TheAudioDBLookupService>();
        services.AddTransient<IAudioLookupService, DiscogsLookupService>();

        services.AddHttpClient("discogs", c => {
                c.BaseAddress = new Uri("https://api.discogs.com/");
            })
            .ConfigureHttpClient(c => {
                c.DefaultRequestHeaders.Add("Authorization",
                    "Discogs token=wdMKyNInHfhXHKshwIieKtzpLAGdAiqHYtehovKL");
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
            });
        services.AddHttpClient("theaudiodb", c => {
                c.BaseAddress = new Uri("https://theaudiodb.com/api/v1/json/523532/");
            })
            .ConfigureHttpClient(c => {
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
            });
        return services;
    }
}