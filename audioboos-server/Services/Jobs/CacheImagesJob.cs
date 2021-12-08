using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data.Models.Settings;
using AudioBoos.Data.Store;
using AudioBoos.Server.Helpers;
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
        await _cacheArtistImages();
        await _cacheAlbumImages();
    }

    private async Task _cacheAlbumImages() {
        var cachePath = Path.Combine(_systemSettings.ImagePath, "album");
        if (!Directory.Exists(cachePath)) {
            Directory.CreateDirectory(cachePath);
        }

        var albums = _albumRepository
            .GetAll()
            .Where(a => a.LargeImage.StartsWith("http")); //ignore local album art

        foreach (var album in albums) {
            var cacheFile = Path.Combine(cachePath, album.Id.ToString());
            var imageFile = album.LargeImage;
            try {
                var result = await ImageCacher.CacheImage(cacheFile, imageFile);
                if (result) {
                    _logger.LogInformation("Successfully cached artist image for {Album}", album.Name);
                }
            } catch (Exception e) {
                _logger.LogError("Error caching image for {Album} - {Image} - {Exception}",
                    album.Name,
                    imageFile,
                    e.Message);
            }
        }

        _logger.LogInformation("**Finished caching album images**");
    }

    private async Task _cacheArtistImages() {
        var cachePath = Path.Combine(_systemSettings.ImagePath, "artist");
        if (!Directory.Exists(cachePath)) {
            Directory.CreateDirectory(cachePath);
        }

        foreach (var artist in _artistRepository.GetAll()) {
            var cacheFile = Path.Combine(cachePath, artist.GetImageFile());
            var imageFile = artist.LargeImage;
            if (File.Exists(cacheFile)) continue;
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

        _logger.LogInformation("**Finished caching artist images**");
    }
}
