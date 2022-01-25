using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AudioBoos.Data.Models.AudioLookups;
using AudioBoos.Server.Helpers;
using AudioBoos.Server.Services.Exceptions.AudioLookup;
using MetaBrainz.MusicBrainz.CoverArt;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;

namespace AudioBoos.Server.Services.AudioLookup;

public class CoverArtLookupService {
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CoverArtLookupService> _logger;

    public CoverArtLookupService(IHttpClientFactory httpClientFactory, ILogger<CoverArtLookupService> logger) {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<string> GetCoverart(Guid releaseId) {
        _logger.LogDebug("Finding cover art for {Release}", releaseId);
        try {
            var covertArt = new CoverArt(
                ProductHeaderValue.Parse("AudioBoos"),
                "https://audioboos.info"
            );
            var release = await covertArt.FetchReleaseAsync(releaseId);
            if (release?.Images?.Count > 0) {
                var image = release.Images.Where(i => i.Front);
                return image.Select(i => i.Location).FirstOrDefault().ToString();
            }
        } catch (JsonException je) {
            _logger.LogError("No cover art available for {release}", releaseId);
        } catch (Exception e) {
            _logger.LogError("Error finding cover art for {release}", releaseId);
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);
        }

        return string.Empty;
    }
}
