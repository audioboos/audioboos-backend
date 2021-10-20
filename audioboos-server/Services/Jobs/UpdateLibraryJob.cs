using System.Threading.Tasks;
using AudioBoos.Server.Services.Jobs.Scanners;
using Microsoft.Extensions.Logging;
using Quartz;

namespace AudioBoos.Server.Services.Jobs {
    [DisallowConcurrentExecution]
    public class UpdateLibraryJob : IAudioBoosJob {
        private readonly ILogger<UpdateLibraryJob> _logger;
        private readonly ILibraryScanner _scanner;

        public string JobName => "ScanArtists";

        public UpdateLibraryJob(ILogger<UpdateLibraryJob> logger, ILibraryScanner scanner) {
            _logger = logger;
            _scanner = scanner;
        }

        public async Task Execute(IJobExecutionContext context) {
            await _scanner.ScanLibrary(context.CancellationToken);
            await _scanner.UpdateUnscannedArtists(context.CancellationToken);
            await _scanner.UpdateChecksums(context.CancellationToken);
            _logger.LogInformation("Update Library Job Complete");
        }
    }
}
