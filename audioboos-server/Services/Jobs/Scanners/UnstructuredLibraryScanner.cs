using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AudioBoos.Server.Services.Jobs.Scanners {
    /// <summary>
    /// This provides a better scanner for "unstructured" audio libraries
    /// Best guess would be to index all audio files and work backwards from there 
    /// </summary>
    public class UnstructuredLibraryScanner : ILibraryScanner {
        private readonly ILogger<UnstructuredLibraryScanner> _logger;

        public UnstructuredLibraryScanner(ILogger<UnstructuredLibraryScanner> logger) {
            _logger = logger;
        }

        public async Task<(int, int, int)> ScanLibrary(CancellationToken cancellationToken) {
            _logger.LogInformation("Starting unstructured library scan");
            return await Task.FromResult((0, 0, 0));
        }
    }
}
