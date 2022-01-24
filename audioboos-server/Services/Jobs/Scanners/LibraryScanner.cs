using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AudioBoos.Data;
using AudioBoos.Data.Access;
using AudioBoos.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using AudioBoos.Data.Store;
using AudioBoos.Server.Helpers;
using AudioBoos.Server.Services.AudioLookup;
using AudioBoos.Server.Services.Exceptions.AudioLookup;
using AudioBoos.Server.Services.Hubs;
using AudioBoos.Server.Services.Tags;
using Mapster;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace AudioBoos.Server.Services.Jobs.Scanners;

internal abstract class LibraryScanner : ILibraryScanner {
    protected readonly ILogger _logger;
    protected readonly IHubContext<JobHub> _messageClient;
    protected readonly IAudioLookupService _lookupService;
    private readonly ISchedulerFactory _schedulerFactory;
    protected readonly SemaphoreSlim __scanLock = new(1, 1);

    protected readonly IServiceProvider _serviceProvider;

    protected LibraryScanner(ILogger<LibraryScanner> logger,
        IHubContext<JobHub> messageClient,
        IAudioLookupService lookupService,
        ISchedulerFactory schedulerFactory,
        IServiceProvider serviceProvider) {
        _logger = logger;
        _messageClient = messageClient;
        _lookupService = lookupService;
        _schedulerFactory = schedulerFactory;
        _serviceProvider = serviceProvider;
    }

    protected async Task<string> _libraryPath() {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetService<AudioBoosContext>();

        return Constants.DebugMode && false
            ? "/mnt/frasier/audio/MuziQ/Pedestrian/Store"
            : await context
                .Settings
                .Where(r => r.Key.ToLower().Equals("librarypath"))
                .Select(s => s.Value)
                .FirstOrDefaultAsync();
    }


    public abstract Task<(int, int, int)> ScanLibrary(bool deepScan, string childFolder,
        CancellationToken cancellationToken);

    public async Task UpdateUnscannedAlbums(CancellationToken cancellationToken) {
        using var scope = _serviceProvider.CreateScope();
        var albumRepository = scope.ServiceProvider.GetService<IAudioRepository<Album>>();
        _logger.LogInformation("Scanning unscanned albums");

        var unscannedAlbums = albumRepository
                .GetAll()
                .AsNoTracking()
                .Include(a => a.Artist)
                .Where(a => a.TaggingStatus != TaggingStatus.ManualUpdate)
                .Where(a => !a.Immutable)
                .ToList()
                .Where(a => Album.IsIncomplete(a) || a.TaggingStatus.Equals(TaggingStatus.MP3TagsOnly))
            // .Where(a => a.Artist.Name.Equals("Bauhaus"))
            ;

        foreach (var album in unscannedAlbums) {
            await UpdateAlbum(album, cancellationToken);
        }

        _logger.LogInformation("Finished scanning albums");
    }

    public async Task UpdateAlbum(Album album, CancellationToken cancellationToken) {
        try {
            if (album.Immutable) {
                _logger.LogWarning("Album {Album} has been marked as immutable, will not scan", album.Name);
            }

            using var scope = _serviceProvider.CreateScope();
            var albumRepository = scope.ServiceProvider.GetService<IAudioRepository<Album>>();
            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            _logger.LogInformation("Scanning {Album}", album.Name);
            var remoteAlbumInfo = await _lookupService.LookupAlbumInfo(
                album.Artist.Name,
                album.Name,
                album.Artist.MusicBrainzId.ToString(),
                cancellationToken);

            if (remoteAlbumInfo is null) {
                return;
            }

            var updated = remoteAlbumInfo.Adapt(album);
            updated.TaggingStatus = TaggingStatus.RemoteLookup;
            updated.LastScanDate = DateTime.Now;
            await albumRepository.InsertOrUpdate(updated, cancellationToken);
            await unitOfWork.Complete();
            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            await scheduler.TriggerJob(
                new JobKey("CacheImages", "DEFAULT"),
                new JobDataMap(
                    new Dictionary<string, string> {
                        {"AlbumName", album.Name}
                    }
                ),
                cancellationToken);
        } catch (AlbumNotFoundException) {
            _logger.LogWarning("Unable to find album info for {Artist} - {Album}", album.Artist.Name, album.Name);
        } catch (Exception e) {
            _logger.LogError("Exception updating info for {Artist} - {Album} - {Error}", album.Artist.Name, album.Name,
                e.Message);
        }
    }


    public async Task UpdateUnscannedArtists(CancellationToken cancellationToken) {
        using var scope = _serviceProvider.CreateScope();
        var artistRepository = scope.ServiceProvider.GetService<IAudioRepository<Artist>>();
        var unscannedArtists = artistRepository
            .GetAll()
            .AsNoTracking()
            .Where(a => a.TaggingStatus != TaggingStatus.ManualUpdate) //never auto scan manually updated artists
            .Where(a => !a.Immutable)
            .ToList()
            .Where(a => Artist.IsIncomplete(a) || a.TaggingStatus.Equals(TaggingStatus.MP3TagsOnly));

        foreach (var artist in unscannedArtists) {
            await UpdateArtist(artist.Name, cancellationToken);
        }
    }

    public async Task UpdateArtist(string artistName, CancellationToken cancellationToken) {
        _logger.LogDebug("Looking up info for {Artist}", artistName);
        using var scope = _serviceProvider.CreateScope();
        var artistRepository = scope.ServiceProvider.GetService<IAudioRepository<Artist>>();
        var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
        if (artistRepository is null || unitOfWork is null) {
            _logger.LogError("Unable to scope unit of work & repositories");
            return;
        }

        var artist = await artistRepository
            .GetAll()
            .Include(a => a.Albums)
            .Where(a => a.Name.Equals(artistName))
            .FirstOrDefaultAsync(cancellationToken);

        if (artist is null) {
            _logger.LogError("Artist {Artist} is not in repository", artistName);
            return;
        }

        if (artist.Immutable) {
            _logger.LogWarning("Artist {Artist} has been marked as immutable, will not scan", artist.Name);
            return;
        }

        try {
            var remoteArtistInfo = await _lookupService.LookupArtistInfo(
                artist.Name,
                cancellationToken);
            if (remoteArtistInfo is null) {
                return;
            }

            bool newImage = !remoteArtistInfo.LargeImage.Equals(artist.LargeImage) ||
                            !remoteArtistInfo.SmallImage.Equals(artist.SmallImage);

            var updated = remoteArtistInfo.Adapt(artist);
            updated.TaggingStatus = TaggingStatus.RemoteLookup;
            updated.LastScanDate = DateTime.Now;

            await artistRepository.InsertOrUpdate(updated, cancellationToken);
            await unitOfWork.Complete();
            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            await scheduler.TriggerJob(
                new JobKey("CacheImages", "DEFAULT"),
                new JobDataMap(
                    new Dictionary<string, string> {
                        {"ArtistName", artist.Name},
                        //TODO: keep this at true for now, until all the stale images are cached
                        {"Overwrite", newImage.ToString()}
                    }
                ),
                cancellationToken);

            _logger.LogDebug("Scanning albums for {Artist}", artistName);
            foreach (var album in artist.Albums) {
                await UpdateAlbum(album, cancellationToken);
            }
        } catch (ArtistNotFoundException) {
            _logger.LogWarning("Artist {Artist} not found in {Scanner}", artist.Name, _lookupService.Name);
        } catch (Exception e) {
            _logger.LogError("Failure finding artist {Artist} in {Scanner}", artist.Name, _lookupService.Name);
            _logger.LogError("{Error}", e.Message);
        }

        _logger.LogInformation("Finished processing artists");
    }

    public async Task UpdateChecksums(CancellationToken cancellationToken) {
        using var scope = _serviceProvider.CreateScope();
        var audioFileRepository = scope.ServiceProvider.GetService<IAudioRepository<AudioFile>>();
        var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
        var unscannedFiles = await audioFileRepository
            .GetAll()
            .Where(a => string.IsNullOrEmpty(a.Checksum))
            .ToListAsync(cancellationToken);

        foreach (var audioFile in unscannedFiles) {
            _logger.LogDebug("Calculating checksum for {File}", audioFile.PhysicalPath);
            using var tagger = new TagLibTagService(audioFile.PhysicalPath);
            var checksum = await tagger.GetChecksum();
            audioFile.Checksum = checksum;
            await audioFileRepository.InsertOrUpdate(audioFile, cancellationToken);
        }

        await unitOfWork.Complete();
    }
}
