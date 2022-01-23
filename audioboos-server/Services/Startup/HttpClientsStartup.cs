using System;
using System.Net;
using System.Net.Http;
using AudioBoos.Server.Services.AudioLookup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AudioBoos.Server.Services.Startup;

public static class HttpClientsStartup {
    // private const string USER_AGENT = "AudioBoos-API/0.1 +https://audioboos.com";
    private const string USER_AGENT = "AudioBoos-API/0.1";

    public static IServiceCollection AddAudioBoosHttpClients(this IServiceCollection services, IConfiguration config) {
        // services.AddTransient<IAudioLookupService, TheAudioDBLookupService>();
        services.AddTransient<CoverArtLookupService>();

        switch (config?.GetSection("System").GetValue<string>("AudioLookupService")) {
            case "discogs":
                services.AddTransient<IAudioLookupService, DiscogsLookupService>();
                break;
            default:
                services.AddTransient<IAudioLookupService, MusicBrainzLookupService>();
                break;
        }

        services.AddTransient<DiscogsLookupService>();

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

        // services.AddHttpClient("coverart", c => {
        //         c.BaseAddress = new Uri("https://coverartarchive.org/");
        //     })
        //     .ConfigureHttpClient(c => {
        //         c.DefaultRequestHeaders.Add("Accept", "application/json");
        //         c.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
        //     }).ConfigurePrimaryHttpMessageHandler(x => new HttpClientHandler {
        //         AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
        //         ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        //     });
        return services;
    }
}
