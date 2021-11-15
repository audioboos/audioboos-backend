﻿using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AudioBoos.Data.Models.AudioLookups;
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
        _logger.LogDebug("Finding cover art for {release}", releaseId);
        using var client = _httpClientFactory.CreateClient("coverart");

        var response = await client.GetAsync($"/release/{releaseId}");
        response.EnsureSuccessStatusCode();

        var results = await JsonSerializer.DeserializeAsync<CoverArtSearchResult>(
            await response.Content.ReadAsStreamAsync());

        return results.images
            .Select(r => r.image.Replace("http://", "https://"))
            .FirstOrDefault();
    }
}
