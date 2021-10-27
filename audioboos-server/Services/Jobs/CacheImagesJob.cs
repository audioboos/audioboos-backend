using System;
using System.IO;
using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data.Models.Settings;
using AudioBoos.Data.Store;
using AudioBoos.Server.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace AudioBoos.Server.Services.Jobs;

[DisallowConcurrentExecution]
public class CacheImagesJob : IAudioBoosJob {
    private readonly IRepository<Artist> _artistRepository;
    private readonly SystemSettings _systemSettings;
    private readonly ILogger<CacheImagesJob> _logger;
    public string JobName => "CacheImages";

    public CacheImagesJob(IRepository<Artist> artistRepository, IOptions<SystemSettings> systemSettings,
        ILogger<CacheImagesJob> logger) {
        _artistRepository = artistRepository;
        _systemSettings = systemSettings.Value;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context) {
        await _cacheArtistImages();
    }

    private async Task _cacheArtistImages() {
        var cachePath = Path.Combine(_systemSettings.ImagePath, "artist");
        if (!Directory.Exists(cachePath)) {
            Directory.CreateDirectory(cachePath);
        }

        foreach (var artist in _artistRepository.GetAll()) {
            var cacheFile = Path.Combine(cachePath, artist.Id.ToString());
            var imageFile = artist.LargeImage;
            try {
                if (File.Exists(cacheFile) || string.IsNullOrEmpty(imageFile)) {
                    continue;
                }

                var file = await HttpHelpers.DownloadFile(imageFile, cacheFile);
                if (!string.IsNullOrEmpty(file) && File.Exists(file)) {
                    _logger.LogInformation("Successfully cached artist image for {Artist}", artist.Name);
                } else {
                    _logger.LogError("Error caching image for {Artist}", artist.Name);
                }
            } catch (Exception e) {
                _logger.LogError("Error caching image for {Artist} - {Image} - {Exception}",
                    artist.Name,
                    imageFile,
                    e.Message);
            }
        }
    }
}
