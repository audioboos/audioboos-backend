using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AudioBoos.Data.Models.DTO;
using AudioBoos.Data.Models.Settings;
using AudioBoos.Server.Helpers;
using AudioBoos.Server.Services.Exceptions.AudioLookup;
using Hqub.MusicBrainz.API;
using Hqub.MusicBrainz.API.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AudioBoos.Server.Services.AudioLookup;

public class MusicBrainzLookupService : IAudioLookupService {
    private readonly SystemSettings _systemSettings;
    private readonly DiscogsLookupService _discogsLookupService;
    private readonly CoverArtLookupService _coverArtService;
    private readonly ILogger<MusicBrainzLookupService> _logger;
    private readonly SystemSettings _settings;
    private readonly MusicBrainzClient _client;
    public string Name => "MusicBrainz";

    public MusicBrainzLookupService(IOptions<SystemSettings> systemSettings, DiscogsLookupService discogsLookupService,
        IOptions<SystemSettings> settings,
        CoverArtLookupService coverArtService,
        ILogger<MusicBrainzLookupService> logger) {
        _systemSettings = systemSettings.Value;
        _discogsLookupService = discogsLookupService; // for album art lookup (for now)
        _coverArtService = coverArtService;
        _logger = logger;
        _settings = settings.Value;
        _client = new MusicBrainzClient() {
            Cache = new FileRequestCache(Path.Combine(_systemSettings.CachePath, "mb-cache"))
        };
    }


    public async Task<ArtistInfoLookupDto>
        LookupArtistInfo(string artistName, CancellationToken cancellationToken = default) {
        _logger.LogDebug("Looking up artist info: {ArtistName}", artistName);

        var remoteInfo = (await _client.Artists.SearchAsync(artistName.QuoteString()))
            .FirstOrDefault();

        if (remoteInfo is null) {
            throw new ArtistNotFoundException($"Artist: {artistName} not found");
        }

        //MusicBrainz doesn't allow artist image lookups
        //There's definitely a better way of doing this but this will do for now
        var image = await _discogsLookupService.GetArtistImage(artistName, cancellationToken);

        if (string.IsNullOrEmpty(remoteInfo.Id)) {
            throw new ArtistNotFoundException($"Artist: {artistName} not found");
        }

        var artist = new ArtistInfoLookupDto(
            artistName,
            string.Empty, //TODO: find out where the artist description is remoteInfo.Annotation,
            remoteInfo.Genres is not null && remoteInfo.Genres.Count != 0
                ? string.Join(',', remoteInfo.Genres?.Select(r => r.Name))
                : string.Empty, image,
            image,
            remoteInfo.Id.ToString(),
            remoteInfo.Aliases?
                .Select(r => r.Name)
                .ToList()) {
            MusicBrainzId = Guid.Parse(remoteInfo.Id)
        };
        return artist;
    }

    public async Task<AlbumInfoLookupDto> LookupAlbumInfo(string artistName, string albumName, string artistId,
        CancellationToken cancellationToken = default) {
        var artistQuery = string.IsNullOrEmpty(artistId) ? $"artistName:{artistName.Trim()}" : $"arid:{artistId}";
        var query = $"{artistQuery} AND release:{albumName}";
        _logger.LogDebug("Querying for album {Artist} - {Album} using {Query}",
            artistName, albumName, _client.Releases.GetQueryUrl("release", query));
        var remoteInfo = await _client.Releases.SearchAsync(query);
        if (remoteInfo is null || remoteInfo.Count == 0) {
            throw new AlbumNotFoundException($"Album: {albumName} not found");
        }

        Release details;
        string art = string.Empty;
        var results = remoteInfo.ToList();
        //loop through all the releases until we have one with cover art
        foreach (var result in results) {
            art = await _coverArtService.GetCoverart(Guid.Parse(result.Id));
            if (string.IsNullOrEmpty(art)) {
                continue;
            }

            details = result;
            break;
        }

        details = remoteInfo.FirstOrDefault();

        if (details is null) {
            throw new AlbumNotFoundException($"Album: {albumName} not found");
        }

        try {
            //TODO: probably shouldn't be caching here

            var dto = new AlbumInfoLookupDto(
                artistName,
                details.Title,
                string.Empty, //TODO: details.Annotation ?? string.Empty,
                details.Genres?
                    .Where(r => !string.IsNullOrEmpty(r.Name))
                    .Select(r => r.Name).ToList(),
                SmallImage: art,
                LargeImage: art
            );
            dto.MusicBrainzId = Guid.Parse(details.Id);
        } catch (Exception e) {
            _logger.LogError("Error finding cover art\n\t{Error}", e.Message);
            throw new AlbumNotFoundException($"Album: {albumName} not found");
        }

        return null;
    }
}
