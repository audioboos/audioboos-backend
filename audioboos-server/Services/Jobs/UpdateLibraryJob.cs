using System.Threading.Tasks;
using AudioBoos.Server.Services.Jobs.Scanners;
using Microsoft.Extensions.Logging;
using Quartz;

namespace AudioBoos.Server.Services.Jobs;

[DisallowConcurrentExecution]
public class UpdateLibraryJob : IAudioBoosJob {
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly ILogger<UpdateLibraryJob> _logger;
    private readonly ILibraryScanner _scanner;

    public string JobName => "ScanArtists";

    public UpdateLibraryJob(ISchedulerFactory schedulerFactory, ILibraryScanner scanner,
        ILogger<UpdateLibraryJob> logger) {
        _schedulerFactory = schedulerFactory;
        _logger = logger;
        _scanner = scanner;
    }

    public async Task Execute(IJobExecutionContext context) {
        var scheduler = await _schedulerFactory.GetScheduler(context.CancellationToken);

        await _scanner.ScanLibrary(false, context.CancellationToken);
        await _scanner.UpdateUnscannedArtists(context.CancellationToken);
        await _scanner.UpdateUnscannedAlbums(context.CancellationToken);
        await _scanner.UpdateChecksums(context.CancellationToken);
        await scheduler.TriggerJob(new JobKey("CacheImages", "DEFAULT"), context.CancellationToken);

        _logger.LogInformation("Update Library Job Complete");
    }
}
