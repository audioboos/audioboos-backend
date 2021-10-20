using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data.Models.Settings;
using AudioBoos.Data.Persistence.Interfaces;
using AudioBoos.Data.Store;
using AudioBoos.Server.Helpers;
using AudioBoos.Server.Services.AudioLookup;
using AudioBoos.Server.Services.Exceptions.AudioLookup;
using AudioBoos.Server.Services.Hubs;
using AudioBoos.Server.Services.Tags;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AudioBoos.Server.Services.Jobs.Scanners {
    internal class FastFilesystemLibraryScanner : LibraryScanner {
        public FastFilesystemLibraryScanner(ILogger<FastFilesystemLibraryScanner> logger,
            IRepository<AudioFile> audioFileRepository,
            IRepository<Artist> artistRepository,
            IRepository<Album> albumRepository,
            IHubContext<JobHub> messageClient,
            IRepository<Track> trackRepository,
            IUnitOfWork unitOfWork,
            IAudioLookupService lookupService,
            IOptions<SystemSettings> systemSettings) : base(logger, audioFileRepository, artistRepository,
            albumRepository, messageClient, trackRepository, unitOfWork, lookupService, systemSettings) {
        }

        public override async Task<(int, int, int)> ScanLibrary(CancellationToken cancellationToken) {
            int artistScans = 0;
            int albumScans = 0;
            int trackScans = 0;

            await __scanLock.WaitAsync(cancellationToken);
            _logger.LogInformation("*** START SCANNING ***");

            await _messageClient.Clients.All.SendAsync("QueueJobMessage", new JobMessage() {
                Message = "Starting scan job",
                Percentage = 0
            }, cancellationToken);

            string scanPath =
                Path.Combine(_systemSettings.AudioPath);
            //Path.Combine(_systemSettings.AudioPath, "Tegan & Sara/If it Was You");;
            var fileList = (await scanPath.GetAllAudioFiles())
                /*.Where(f => f.Contains("Confess")).ToList()*/
                ;

            int fileCount = fileList.Count;
            int currentFile = 0;
            foreach (var file in fileList) {
                try {
                    _logger.LogDebug("Scanning:  {File}", file);
                    await _messageClient.Clients.All.SendAsync("QueueJobMessage", new JobMessage() {
                        Message = $"Scanning: {new FileInfo(file).Name}",
                        Percentage = (int)(++currentFile * 100.0 / fileCount)
                    }, cancellationToken);

                    using var tagger = new TagLibTagService(file);
                    var artistName = tagger.GetArtistName();
                    var albumName = tagger.GetAlbumName();
                    var trackName = tagger.GetTrackName();
                    var trackNumber = tagger.GetTrackNumber();
                    var catalogueId = tagger.GetAlbumCatalogueNumber();

                    if (string.IsNullOrEmpty(artistName) || string.IsNullOrEmpty(albumName) ||
                        string.IsNullOrEmpty(trackName)) {
                        _logger.LogError(
                            "Invalid tag information in file - {File}\n\tArtist: {ArtistName}\n\tAlbum: {AlbumName}\n\tTrack: {TrackName}",
                            file, artistName, albumName, trackName);
                        //TODO: Manage tracks with no tag information
                        continue;
                    }

                    var fileModel = await _audioFileRepository
                                        .GetByFile(file, cancellationToken) ??
                                    new AudioFile(
                                        file,
                                        artistName,
                                        albumName,
                                        trackName);

                    fileModel.LastScanDate = DateTime.UtcNow;
                    await _audioFileRepository.InsertOrUpdate(
                        fileModel,
                        cancellationToken);
                    await _unitOfWork.Complete();

                    var artist = await _processArtistInfo(file, artistName, tagger, cancellationToken);
                    await _artistRepository.InsertOrUpdate(
                        artist,
                        cancellationToken);
                    fileModel.Artist = artist;
                    await _unitOfWork.Complete();

                    var album = await _processAlbumInfo(file, artist, albumName, cancellationToken);
                    await _albumRepository.InsertOrUpdate(
                        album,
                        cancellationToken);
                    fileModel.Album = album;
                    await _unitOfWork.Complete();

                    var track = await _processTrackInfo(file, album, trackName, trackNumber, cancellationToken);
                    await _trackRepository.InsertOrUpdate(
                        track,
                        cancellationToken);
                    fileModel.Track = track;
                    await _unitOfWork.Complete();

                    await _unitOfWork.Complete();

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

            await _unitOfWork.Complete();

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
            var artist = await _artistRepository.GetByFile(file, cancellationToken) ??
                         await _artistRepository.GetByName(artistName, cancellationToken) ??
                         await _artistRepository.GetByAlternativeNames(cancellationToken, artistName) ??
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
            var album = await _albumRepository.GetByFile(file, cancellationToken) ??
                        await _albumRepository.GetByName(albumName, cancellationToken) ??
                        await _albumRepository.GetByAlternativeNames(cancellationToken, albumName) ??
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
            var track = await _trackRepository.GetByFile(file, cancellationToken) ??
                        // don't get by name - as it will return a track from a different album
                        // await _trackRepository.GetByName(trackName, cancellationToken) ??
                        await _trackRepository.GetByAlternativeNames(cancellationToken, trackName) ??
                        new Track(album.Id, trackName, trackNumber, file);

            return track;
        }
    }
}
