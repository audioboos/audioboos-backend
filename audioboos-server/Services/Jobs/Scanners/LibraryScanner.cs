using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data.Models.Settings;
using AudioBoos.Data.Persistence.Interfaces;
using AudioBoos.Data.Store;
using AudioBoos.Server.Services.AudioLookup;
using AudioBoos.Server.Services.Exceptions.AudioLookup;
using AudioBoos.Server.Services.Hubs;
using AudioBoos.Server.Services.Tags;
using Mapster;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AudioBoos.Server.Services.Jobs.Scanners;

internal abstract class LibraryScanner : ILibraryScanner {
    protected readonly ILogger _logger;
    protected readonly IRepository<AudioFile> _audioFileRepository;
    protected readonly IRepository<Artist> _artistRepository;
    protected readonly IRepository<Album> _albumRepository;
    protected readonly IHubContext<JobHub> _messageClient;
    protected readonly IRepository<Track> _trackRepository;
    protected readonly IUnitOfWork _unitOfWork;
    protected readonly IAudioLookupService _lookupService;
    protected readonly SystemSettings _systemSettings;

    protected readonly SemaphoreSlim __scanLock = new(1, 1);

    protected LibraryScanner(ILogger<LibraryScanner> logger,
        IRepository<AudioFile> audioFileRepository,
        IRepository<Artist> artistRepository,
        IRepository<Album> albumRepository,
        IHubContext<JobHub> messageClient,
        IRepository<Track> trackRepository,
        IUnitOfWork unitOfWork,
        IAudioLookupService lookupService,
        IOptions<SystemSettings> systemSettings
    ) {
        _logger = logger;
        _audioFileRepository = audioFileRepository;
        _artistRepository = artistRepository;
        _albumRepository = albumRepository;
        _messageClient = messageClient;
        _trackRepository = trackRepository;
        _unitOfWork = unitOfWork;
        _lookupService = lookupService;
        _systemSettings = systemSettings.Value;
    }

    public abstract Task<(int, int, int)> ScanLibrary(CancellationToken cancellationToken);

    public async Task UpdateUnscannedArtists(CancellationToken cancellationToken) {
        var unscannedArtists = await _artistRepository
            .GetAll()
            .Where(a => string.IsNullOrEmpty(a.SmallImage) || string.IsNullOrEmpty(a.LargeImage) ||
                        string.IsNullOrEmpty(a.Description))
            .Where(a => a.TaggingStatus != TaggingStatus.ManualUpdate) //never auto scan manually updated artists
            .Where(a => a.Name.Equals("Bleep & Booster"))
            .ToListAsync(cancellationToken);

        foreach (var artist in unscannedArtists) {
            _logger.LogDebug("Looking up info for {Artist}", artist.Name);
            try {
                var remoteArtistInfo = await _lookupService.LookupArtistInfo(
                    artist.Name,
                    cancellationToken);
                if (remoteArtistInfo is null) {
                    continue;
                }

                var updated = remoteArtistInfo.Adapt(artist);
                updated.TaggingStatus = TaggingStatus.RemoteLookup;

                await _artistRepository.InsertOrUpdate(updated, cancellationToken);
                await _unitOfWork.Complete();
            } catch (ArtistNotFoundException) {
                _logger.LogWarning("Artist {Artist} not found in {Scanner}", artist.Name, _lookupService.Name);
            } catch (Exception e) {
                _logger.LogError("Failure finding artist {Artist} in {Scanner}", artist.Name, _lookupService.Name);
                _logger.LogError("{Error}", e.Message);
            }
        }

        _logger.LogInformation("Finished processing artists");
    }

    public async Task UpdateChecksums(CancellationToken cancellationToken) {
        var unscannedFiles = await _audioFileRepository
            .GetAll()
            .Where(a => string.IsNullOrEmpty(a.Checksum))
            .ToListAsync(cancellationToken);

        var options = new ParallelOptions {MaxDegreeOfParallelism = 10, CancellationToken = cancellationToken};
        foreach (var audioFile in unscannedFiles) {
            _logger.LogDebug("Calculating checksum for {File}", audioFile.PhysicalPath);
            using var tagger = new TagLibTagService(audioFile.PhysicalPath);
            var checksum = await tagger.GetChecksum();
            audioFile.Checksum = checksum;
            await _audioFileRepository.InsertOrUpdate(audioFile, cancellationToken);
        }

        await _unitOfWork.Complete();
    }
}
