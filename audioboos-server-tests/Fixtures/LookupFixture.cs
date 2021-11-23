using AudioBoos.Server.Services.AudioLookup;
using AudioBoos.Server.Services.Startup;
using Microsoft.Extensions.DependencyInjection;

namespace AudioBoos.Server.Tests.Fixtures;

public class LookupFixture : Fixture {
    public LookupFixture() {
        _serviceCollection.AddAudioBoosHttpClients();
        _serviceCollection.AddScoped<IAudioLookupService, MusicBrainzLookupService>();

        ServiceProvider = _serviceCollection.BuildServiceProvider();
    }
}
