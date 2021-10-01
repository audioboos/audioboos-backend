using System.Threading.Tasks;
using AudioBoos.Server.Services.Jobs.Scanners;
using Microsoft.Extensions.Logging;
using Quartz;

namespace AudioBoos.Server.Services.Jobs {
    [DisallowConcurrentExecution]
    public class UpdateLibraryJob : IAudioBoosJob {
        private readonly ILogger<UpdateLibraryJob> _logger;
        private readonly ILibraryScanner _scanner;

        public string JobName => "UpdateLibrary";

        public UpdateLibraryJob(ILogger<UpdateLibraryJob> logger, ILibraryScanner scanner) {
            _logger = logger;
            _scanner = scanner;
        }

        public Task Execute(IJobExecutionContext context) {
            return _scanner.ScanLibrary(context.CancellationToken);
        }
    }
}
