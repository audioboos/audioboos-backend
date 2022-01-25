using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data.Models.Settings;
using AudioBoos.Data.Store;
using AudioBoos.Server.Services.Images;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace AudioBoos.Server.Services.Jobs;

[DisallowConcurrentExecution]
public class CacheImagesJob : IAudioBoosJob {
    private readonly IAudioRepository<Artist> _artistRepository;
    private readonly IAudioRepository<Album> _albumRepository;
    private readonly SystemSettings _systemSettings;
    private readonly ILogger<CacheImagesJob> _logger;
    public string JobName => "CacheImages";

    public CacheImagesJob(IAudioRepository<Artist> artistRepository, IAudioRepository<Album> albumRepository,
        IOptions<SystemSettings> systemSettings,
        ILogger<CacheImagesJob> logger) {
        _artistRepository = artistRepository;
        _albumRepository = albumRepository;
        _systemSettings = systemSettings.Value;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context) {
        var artistName = context.MergedJobDataMap
            .Where(r => r.Key.Equals("ArtistName"))
            .Select(r => r.Value.ToString())
            .FirstOrDefault();
        var albumName = context.MergedJobDataMap
            .Where(r => r.Key.Equals("AlbumName"))
            .Select(r => r.Value.ToString())
            .FirstOrDefault();

        if (!string.IsNullOrEmpty(artistName)) {
            var result = bool.TryParse(context.MergedJobDataMap
                .Where(r => r.Key.Equals("Overwrite"))
                .Select(r => r.Value.ToString())
                .FirstOrDefault(), out var newImage);
            if (!result) {
                newImage = false;
            }

            var cachePath = Path.Combine(_systemSettings.ImagePath, "artist");
            if (!Directory.Exists(cachePath)) {
                Directory.CreateDirectory(cachePath);
            }

            var artist = await _artistRepository.GetByName(artistName);
            if (artist is not null) {
                await _cacheArtistImage(artist, cachePath, newImage);
            }
        } else if (!string.IsNullOrEmpty(albumName)) {
            var cachePath = Path.Combine(_systemSettings.ImagePath, "album");
            if (!Directory.Exists(cachePath)) {
                Directory.CreateDirectory(cachePath);
            }

            var album = await _albumRepository.GetByName(albumName);
            if (album is not null) {
                await _cacheAlbumImage(album, cachePath);
            }
        } else {
            await _cacheArtistImages();
            await _cacheAlbumImages();
        }
    }

    private async Task _cacheAlbumImages() {
        var cachePath = Path.Combine(_systemSettings.ImagePath, "album");
        if (!Directory.Exists(cachePath)) {
            Directory.CreateDirectory(cachePath);
        }

        _logger.LogInformation("**Finished caching album images**");
        var albums = _albumRepository
            .GetAll()
            .Where(a => !string.IsNullOrEmpty(a.LargeImage)); //ignore local album art

        foreach (var album in albums) {
            await _cacheAlbumImage(album, cachePath);
        }
    }

    private async Task _cacheAlbumImage(Album album, string cachePath) {
        var cacheFile = Path.Combine(cachePath, album.Id.ToString());
        var imageFile = album.LargeImage;
        try {
            var result = await ImageCacher.CacheImage(cacheFile, imageFile);
            if (result) {
                _logger.LogInformation("Successfully cached album image for {Album}", album.Name);
            }
        } catch (Exception e) {
            _logger.LogError("Error caching image for {Album} - {Image} - {Exception}",
                album.Name,
                imageFile,
                e.Message);
        }
    }

    private async Task _cacheArtistImages() {
        var cachePath = Path.Combine(_systemSettings.ImagePath, "artist");
        if (!Directory.Exists(cachePath)) {
            Directory.CreateDirectory(cachePath);
        }

        foreach (var artist in _artistRepository.GetAll()) {
            //TODO: We need a better method of detecting of the artist images have changed
            //TODO: We shouldn't be passing true here
            await _cacheArtistImage(artist, cachePath, true);
        }

        _logger.LogInformation("**Finished caching artist images**");
    }


    private async Task _cacheArtistImage(Artist artist, string cachePath, bool overwriteExisting) {
        var cacheFile = Path.Combine(cachePath, artist.GetImageFile());
        var imageFile = artist.LargeImage;
        if (File.Exists(cacheFile)) {
            if (overwriteExisting) {
                File.Delete(cacheFile);
            } else {
                return;
            }
        }

        try {
            if (string.IsNullOrEmpty(imageFile)) {
                _logger.LogInformation("Creating placeholder image for {Artist}", artist.Name);
                var placeholder = await TextImageGenerator.CreateArtistAvatarImage(artist.Name);
                await File.WriteAllBytesAsync(cacheFile, placeholder);
            } else {
                var result = await ImageCacher.CacheImage(cacheFile, imageFile);
                if (result) {
                    _logger.LogInformation("Successfully cached artist image for {Artist}", artist.Name);
                }
            }
        } catch (Exception e) {
            _logger.LogError("Error caching image for {Artist} - {Image} - {Exception}",
                artist.Name,
                imageFile,
                e.Message);
        }
    }
}
