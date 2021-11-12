using System.Threading;
using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data.Models.Settings;
using AudioBoos.Data.Persistence;
using AudioBoos.Data.Persistence.Interfaces;
using AudioBoos.Data.Store;
using AudioBoos.Server.Services.AudioLookup;
using AudioBoos.Server.Services.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AudioBoos.Server.Services.Jobs.Scanners;

/// <summary>
/// This provides a better scanner for "unstructured" audio libraries
/// Best guess would be to index all audio files and work backwards from there 
/// </summary>
internal class UnstructuredLibraryScanner : LibraryScanner {
    public UnstructuredLibraryScanner(ILogger<UnstructuredLibraryScanner> logger,
        AudioBoosContext context,
        IRepository<AudioFile> audioFileRepository,
        IRepository<Artist> artistRepository,
        IRepository<Album> albumRepository,
        IHubContext<JobHub> messageClient,
        IRepository<Track> trackRepository,
        IUnitOfWork unitOfWork,
        IAudioLookupService lookupService,
        IOptions<SystemSettings> systemSettings) : base(logger, context, audioFileRepository, artistRepository,
        albumRepository, messageClient, trackRepository, unitOfWork, lookupService, systemSettings) {
    }

    public override async Task<(int, int, int)> ScanLibrary(bool deepScan, CancellationToken cancellationToken) {
        _logger.LogInformation("Starting unstructured library scan");
        return await Task.FromResult((0, 0, 0));
    }
}
