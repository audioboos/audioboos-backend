#nullable enable
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AudioBoos.Data.Models.AudioLookups;
using AudioBoos.Data.Models.DTO;
using AudioBoos.Server.Helpers;
using AudioBoos.Server.Services.Exceptions.AudioLookup;
using Microsoft.Extensions.Logging;

namespace AudioBoos.Server.Services.AudioLookup {
    public class DiscogsLookupService : IAudioLookupService {
        private readonly ILogger<DiscogsLookupService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        private string[] COUNTRY_PRIORITIES = {
            "Europe", "UK", "UK & Europe", "US"
        };

        public DiscogsLookupService(ILogger<DiscogsLookupService> logger, IHttpClientFactory httpClientFactory) {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ArtistDTO>
            LookupArtistInfo(string artistName, CancellationToken cancellationToken = default) {
            var request = $"/database/search?q={Uri.EscapeDataString(artistName)}&type=artist";
            try {
                using var client = _httpClientFactory.CreateClient("discogs");

                try {
                    var response = await client.GetAsync(request, cancellationToken);
                    response.EnsureSuccessStatusCode();

                    var results = await JsonSerializer.DeserializeAsync<DiscogsArtistSearchResult>(
                        await response.Content.ReadAsStreamAsync(cancellationToken),
                        cancellationToken: cancellationToken);
                    if (results?.results != null && (results.results.Count != 0)) {
                        //lookup the full details in the api 
                        var result = results.results.FirstOrDefault();
                        if (result == null) {
                            throw new ArtistNotFoundException(
                                $"Artist {artistName} not found in discogs\n\tRequest: {client.BaseAddress}/{request}");
                        }

                        request = $"/artists/{result.id}";
                        var artistResult = await client.GetAsync(request);
                        var artist = await JsonSerializer.DeserializeAsync<DiscogsArtistResult>(
                            await artistResult.Content.ReadAsStreamAsync(cancellationToken),
                            cancellationToken: cancellationToken);

                        var parsed = new ArtistDTO(
                            //Fix discogs putting (2) or (3) at the end of artist name
                            Regex.Replace(
                                artist?.name ?? "Unknown artist",
                                @"\(\d+\)",
                                string.Empty).Trim(),
                            artist?.profile ?? string.Empty,
                            string.Empty, //TODO: Find genre info from Discogs
                            artist?.images.FirstOrDefault(r => r.type.Equals("secondary"))
                                ?.resource_url ?? string.Empty,
                            artist?.images.FirstOrDefault(r => r.type.Equals("primary"))
                                ?.resource_url ?? string.Empty,
                            artist?.id.ToString() ?? string.Empty,
                            artist?
                                .aliases?
                                .Select(r => r.name)
                                .ToList() ?? Array.Empty<string>().ToList()
                        );
                        return parsed;
                    }
                } catch (HttpRequestException e) {
                    _logger.LogError($"Error getting info for artist: {artistName}\n\t{e.Message}");
                }

                throw new ArtistNotFoundException(
                    $"Artist {artistName} not found in discogs\n\tRequest: {client.BaseAddress}/{request}");
            } catch (Exception e) {
                _logger.LogError($"Unable to create discogs http client {e.Message}");
                throw;
            }
        }

        public async Task<AlbumDTO> LookupAlbumInfo(string artistName, string albumName,
            CancellationToken cancellationToken = default) {
            var request =
                $"/database/search?artist={Uri.EscapeDataString(artistName)}&title={Uri.EscapeDataString(albumName)}&type=album";
            try {
                using var client = _httpClientFactory.CreateClient("discogs");

                try {
                    var response = await client.GetAsync(request, cancellationToken);
                    response.EnsureSuccessStatusCode();
                    var results = await JsonSerializer.DeserializeAsync<DiscogsAlbumSearchResult>(
                        await response.Content.ReadAsStreamAsync(cancellationToken),
                        cancellationToken: cancellationToken);
                    if (results is null) {
                        throw new AlbumNotFoundException(
                            $"Album {artistName} - {albumName} not found in discogs\n\tRequest: ${client.BaseAddress}/{request}");
                    }


                    //this looks kinda weird but it's a descending list of selection criteria
                    var album = results.results
                                    .FirstOrDefault(a =>
                                        COUNTRY_PRIORITIES.Contains(a.country) && !string.IsNullOrEmpty(a.year)) ??
                                results.results
                                    .FirstOrDefault(a => COUNTRY_PRIORITIES.Contains(a.country)) ??
                                results.results.FirstOrDefault(a => !string.IsNullOrEmpty(a.year)) ??
                                results.results
                                    .FirstOrDefault();
                    if (album is null) {
                        throw new AlbumNotFoundException(
                            $"Album {artistName} - {albumName} not found in discogs\n\tRequest: ${client.BaseAddress}/{request}");
                    }

                    var regex = @$"{artistName} ?\(?\d?\)? ?[–,-] ?"; //Fuck discogs weird ass hyphens
                    try {
                        var parsed = new AlbumDTO(artistName) {
                            Name = Regex.Replace(
                                album.title,
                                regex,
                                string.Empty,
                                RegexOptions.IgnoreCase).ToTitleCase(),
                            Description = album.style.Count != 0 ? string.Join("\n", album.style) : string.Empty,
                            SiteId = $"{album.id}",
                            LargeImage = album.cover_image,
                            SmallImage = album.cover_image
                        };
                        return parsed;
                    } catch (Exception parsedException) {
                        _logger.LogError(
                            "Error getting info for album: {ArtistName} - {AlbumName}\n\t{Message}",
                            artistName, albumName, parsedException.Message);
                        throw;
                    }
                } catch (HttpRequestException e) {
                    _logger.LogError("Error getting info for album: {ArtistName} - {AlbumName}\n\t{Message}",
                        artistName, albumName, e.Message);
                }

                throw new AlbumNotFoundException(
                    $"Album {artistName} - {albumName} not found in discogs\n\tRequest: ${client.BaseAddress}/{request}");
            } catch (Exception e) {
                _logger.LogError("Unable to create discogs http client {Message}",
                    e.Message);
                throw;
            }
        }
    }
}
