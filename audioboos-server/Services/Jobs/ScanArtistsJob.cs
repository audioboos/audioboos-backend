using System.Threading.Tasks;
using AudioBoos.Server.Services.Jobs.Scanners;
using Microsoft.Extensions.Logging;
using Quartz;

namespace AudioBoos.Server.Services.Jobs;

[DisallowConcurrentExecution]
public class ScanArtistsJob : IAudioBoosJob {
    private readonly ILogger<ScanArtistsJob> _logger;
    private readonly ILibraryScanner _scanner;

    public string JobName => "UpdateLibrary";

    public ScanArtistsJob(ILogger<ScanArtistsJob> logger, ILibraryScanner scanner) {
        _logger = logger;
        _scanner = scanner;
    }

    public async Task Execute(IJobExecutionContext context) {
        _logger.LogInformation("Starting artists scan");
        await _scanner.UpdateUnscannedArtists(context.CancellationToken);
    }
}
