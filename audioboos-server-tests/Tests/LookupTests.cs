using System;
using System.Threading.Tasks;
using AudioBoos.Data.Models.Settings;
using AudioBoos.Server.Services.AudioLookup;
using AudioBoos.Server.Tests.Fixtures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AudioBoos.Server.Tests;

public class LookupTests : IClassFixture<LookupFixture> {
    private readonly ServiceProvider _serviceProvider;

    public LookupTests(LookupFixture fixture) {
        _serviceProvider = fixture.ServiceProvider;
        var options = fixture.Config.Get<SystemSettings>();
        Console.WriteLine(options.DefaultSiteName);
    }

    [Fact]
    public async Task Test_Lookup_Artist_Info() {
        using var scope = _serviceProvider.CreateScope();
        var lookupService = scope.ServiceProvider.GetRequiredService<IAudioLookupService>();

        var artistNames = new[] {
            "Adam & The Ants",
            "Air"
        };
        foreach (var artistName in artistNames) {
            var result = await lookupService.LookupArtistInfo(artistName);
            Assert.Equal(artistName, result.Name);
            //Assert.NotEmpty(result.Description);
            Assert.NotEmpty(result.LargeImage);
            Assert.NotEmpty(result.SmallImage);
        }
    }
}
