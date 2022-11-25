using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data.Interfaces;
using AudioBoos.Data.Store;
using AudioBoos.Server.Helpers;
using AudioBoos.Server.Services.AudioLookup;
using AudioBoos.Server.Services.Exceptions;
using AudioBoos.Server.Services.Exceptions.AudioLookup;
using AudioBoos.Server.Services.Hubs;
using AudioBoos.Server.Services.Tags;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace AudioBoos.Server.Services.Jobs.Scanners;

internal class FastFilesystemLibraryScanner : LibraryScanner {
    public FastFilesystemLibraryScanner(ILogger<FastFilesystemLibraryScanner> logger,
        IHubContext<JobHub> messageClient,
        IAudioLookupService lookupService,
        ISchedulerFactory schedulerFactory,
        IServiceProvider serviceProvider) : base(
        logger, messageClient, lookupService,
        schedulerFactory, serviceProvider) {
    }

    public override async Task<(int, int, int)> ScanLibrary(bool deepScan, string childFolder,
        CancellationToken cancellationToken) {
        int artistScans = 0;
        int albumScans = 0;
        int trackScans = 0;

        await __scanLock.WaitAsync(cancellationToken);
        _logger.LogInformation("*** START SCANNING ***");
        using var scope = _serviceProvider.CreateScope();
        var trackRepository = scope.ServiceProvider.GetService<IAudioRepository<Track>>();
        var audioFileRepository = scope.ServiceProvider.GetService<IAudioRepository<AudioFile>>();
        var artistRepository = scope.ServiceProvider.GetService<IAudioRepository<Artist>>();
        var albumRepository = scope.ServiceProvider.GetService<IAudioRepository<Album>>();
        var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();

        if (trackRepository is null) {
            throw new NullReferenceException(nameof(trackRepository));
        }

        await _messageClient.Clients.All.SendAsync("QueueJobMessage", new JobMessage() {
            Message = "Starting scan job",
            Percentage = 0
        }, cancellationToken);

        var libraryPath = Path.Combine(await _libraryPath(), childFolder);
        if (string.IsNullOrEmpty(libraryPath) || !Directory.Exists(libraryPath)) {
            _logger.LogError("Library path ({LibraryPath}) does not exist", libraryPath);
            throw new LibraryNotFoundException($"Library path ({libraryPath}) does not exist");
        }

        var fileList = (await libraryPath.GetAllAudioFiles());
        if (!deepScan) {
            var scannedList = await trackRepository.GetAll()
                .Select(r => r.PhysicalPath)
                .ToListAsync(cancellationToken);
            fileList = fileList
                .Except(scannedList)
                .ToList();
        }

        int fileCount = fileList.Count;
        int currentFile = 0;
        foreach (var file in fileList) {
            try {
                _logger.LogInformation("Scanning:  {File}", file);
                await _messageClient.Clients.All.SendAsync("QueueJobMessage", new JobMessage() {
                    Message = $"Scanning: {new FileInfo(file).Name}",
                    Percentage = (int)(++currentFile * 100.0 / fileCount)
                }, cancellationToken);

                using var tagger = new TagLibTagService(file);
                var artistName = tagger.GetArtistName();
                var albumName = tagger.GetAlbumName();
                var trackName = tagger.GetTrackName();
                var trackNumber = tagger.GetTrackNumber();
                var duration = tagger.GetDuration();

                if (string.IsNullOrEmpty(artistName) || string.IsNullOrEmpty(albumName) ||
                    string.IsNullOrEmpty(trackName)) {
                    _logger.LogError(
                        "Invalid tag information in file - {File}\n\tArtist: {ArtistName}\n\tAlbum: {AlbumName}\n\tTrack: {TrackName}",
                        file, artistName, albumName, trackName);
                    //TODO: Manage tracks with no tag information
                    continue;
                }

                var fileModel = await audioFileRepository
                                    .GetByFile(file, cancellationToken) ??
                                new AudioFile(
                                    file,
                                    artistName,
                                    albumName,
                                    trackName);

                fileModel.LastScanDate = DateTime.UtcNow;
                await audioFileRepository.InsertOrUpdate(
                    fileModel,
                    cancellationToken);
                await unitOfWork.Complete();

                var artist = await _processArtistInfo(file, artistName, tagger, cancellationToken);
                await artistRepository.InsertOrUpdate(
                    artist,
                    cancellationToken);
                fileModel.Artist = artist;
                await unitOfWork.Complete();

                var album = await _processAlbumInfo(file, artist, albumName, cancellationToken);
                await albumRepository.InsertOrUpdate(
                    album,
                    cancellationToken);
                fileModel.Album = album;
                await unitOfWork.Complete();

                var track = await _processTrackInfo(file, album, trackName, trackNumber, cancellationToken);
                track.Duration = duration;
                await trackRepository.InsertOrUpdate(
                    track,
                    cancellationToken);
                fileModel.Track = track;

                await unitOfWork.Complete();

                trackScans++;
            } catch (ArtistNotFoundException e) {
                //TODO: dunno.... guess we should do something??
                _logger.LogError("Error scanning {File}", e.Message);
                _logger.LogError("Error finding artist {Error}", e.Message);
            } catch (AlbumNotFoundException e) {
                _logger.LogError("Error scanning {File}", e.Message);
                _logger.LogError("Error finding album {Error}", e.Message);
            } catch (DbUpdateException e) {
                _logger.LogError("Error scanning {File}", e.Message);
                _logger.LogError("Database error {Error}", e.InnerException.Message);
            } catch (Exception e) {
                _logger.LogError("Error scanning {File}", file);
                _logger.LogError("File scan error {Error}", e.Message);
            }
        }

        await unitOfWork.Complete();

        await _messageClient.Clients.All.SendAsync("QueueJobMessage", new JobMessage() {
            Message = "",
            Percentage = 0
        }, cancellationToken);

        _logger.LogInformation("*** FINISHED SCANNING ***");
        __scanLock.Release();
        return (artistScans, albumScans, trackScans);
    }

    private async Task<Artist> _processArtistInfo(string file, string artistName,
        TagLibTagService tagger, CancellationToken cancellationToken) {
        using var scope = _serviceProvider.CreateScope();
        var artistRepository = scope.ServiceProvider.GetService<IAudioRepository<Artist>>();
        if (artistRepository is null) { throw new NullReferenceException(nameof(artistRepository)); }

        //TODO: Use LinqExtension.MostCommon to find the most common artist name in the folder
        var artist = await artistRepository.GetByFile(file, cancellationToken) ??
                     await artistRepository.GetByName(artistName, cancellationToken) ??
                     await artistRepository.GetByAlternativeNames(cancellationToken, artistName) ??
                     new Artist(artistName);

        if (!string.IsNullOrEmpty(artist.LargeImage) && !string.IsNullOrEmpty(artist.SmallImage)) {
            return artist;
        }

        try {
            artist.Description = tagger.GetArtistDescription();
            artist.TaggingStatus = TaggingStatus.MP3TagsOnly;
        } catch (ArtistNotFoundException) {
        }

        return artist;
    }

    private async Task<Album> _processAlbumInfo(string file, Artist artist, string albumName,
        CancellationToken cancellationToken) {
        using var scope = _serviceProvider.CreateScope();
        var albumRepository = scope.ServiceProvider.GetService<IAudioRepository<Album>>();
        var album = await albumRepository.GetByFile(file, cancellationToken) ??
                    await albumRepository.GetByName(albumName, cancellationToken) ??
                    await albumRepository.GetByAlternativeNames(cancellationToken, albumName) ??
                    new Album(artist, albumName);

        if (string.IsNullOrEmpty(album.LargeImage)) {
            album.LargeImage = AlbumArtHelper.GetLargeAlbumImagePath(Path.GetDirectoryName(file));
        }

        if (string.IsNullOrEmpty(album.SmallImage)) {
            album.SmallImage = AlbumArtHelper.GetSmallAlbumImagePath(Path.GetDirectoryName(file));
        }

        try {
            try {
                album.ArtistId = artist.Id;
                album.TaggingStatus = TaggingStatus.RemoteLookup;
                // album.LastScanDate = DateTime.UtcNow;
            } catch (Exception e) {
                _logger.LogError("{Error}", e.Message);
            }
        } catch (AlbumNotFoundException) {
        }

        return album;
    }

    private async Task<Track> _processTrackInfo(string file, Album album, string trackName,
        int trackNumber, CancellationToken cancellationToken) {
        using var scope = _serviceProvider.CreateScope();
        var trackRepository = scope.ServiceProvider.GetService<IAudioRepository<Track>>();
        var track = await trackRepository.GetByFile(file, cancellationToken) ??
                    // don't get by name - as it will return a track from a different album
                    // await _trackRepository.GetByName(trackName, cancellationToken) ??
                    await trackRepository.GetByAlternativeNames(cancellationToken, trackName) ??
                    new Track(album.Id, trackName, trackNumber, file);

        return track;
    }
}
