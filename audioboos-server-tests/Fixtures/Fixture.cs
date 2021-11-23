using System.IO;
using AudioBoos.Server.Services.Startup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AudioBoos.Server.Tests.Fixtures;

public class Fixture {
    protected readonly ServiceCollection _serviceCollection;
    public IConfiguration Config { get; }
    public ServiceProvider ServiceProvider { get; protected init; }

    protected Fixture() {
        _serviceCollection = new ServiceCollection();
        Config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(
                "appsettings.Development.json",
                false,
                true)
            .Build();
        _serviceCollection.AddAudioBoosOptions(Config);
    }
}
