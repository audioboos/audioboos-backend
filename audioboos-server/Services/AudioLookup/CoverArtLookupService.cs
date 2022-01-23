using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AudioBoos.Data.Models.AudioLookups;
using AudioBoos.Server.Helpers;
using Microsoft.Extensions.Logging;

namespace AudioBoos.Server.Services.AudioLookup;

public class CoverArtLookupService {
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CoverArtLookupService> _logger;

    public CoverArtLookupService(IHttpClientFactory httpClientFactory, ILogger<CoverArtLookupService> logger) {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<string> GetCoverart(string releaseId) {
        _logger.LogDebug("Finding cover art for {Release}", releaseId);
        try {
            using var client = _httpClientFactory.CreateClient("coverart");

            var response = await client.GetAsync($"/release/{releaseId}");
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var json = stream.ToEncodedString(Encoding.UTF8, true);
            _logger.LogInformation("Data from coverart lookup is \n{Json}", json);

            var results =
                await JsonSerializer.DeserializeAsync<CoverArtSearchResult>(await response.Content.ReadAsStreamAsync());
            if (results is not null) {
                return results.images
                    .Where(r => r.front)
                    .Select(r => r.image.Replace("http://", "https://"))
                    .FirstOrDefault();
            }
        } catch (Exception e) {
            _logger.LogError("Error finding cover art for {release}", releaseId);
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);
        }

        return string.Empty;
    }
}
