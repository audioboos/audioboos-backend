using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AudioBoos.Data.Models.DTO;
using AudioBoos.Data.Models.Settings;
using AudioBoos.Server.Helpers;
using AudioBoos.Server.Services.Exceptions.AudioLookup;
using MetaBrainz.MusicBrainz;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AudioBoos.Server.Services.AudioLookup;

public class MusicBrainzLookupService : IAudioLookupService {
    private readonly SystemSettings _systemSettings;
    private readonly DiscogsLookupService _discogsLookupService;
    private readonly CoverArtLookupService _coverArtService;
    private readonly ILogger<MusicBrainzLookupService> _logger;
    private readonly SystemSettings _settings;
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
    }


    private Query _buildQuery() => new(
        _settings.DefaultSiteName,
        Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0",
        _settings.ContactEmail);


    public async Task<ArtistInfoLookupDto>
        LookupArtistInfo(string artistName, CancellationToken cancellationToken = default) {
        _logger.LogDebug("Looking up artist info: {ArtistName}", artistName);
        var artistQuery = _buildQuery();
        var remoteInfo =
            (await artistQuery
                .FindArtistsAsync(artistName, 1, simple: false))
            .Results[0].Item;

        //MusicBrainz doesn't allow artist image lookups
        //There's definitely a better way of doing this but this will do for now
        var image = await _discogsLookupService.GetArtistImage(artistName, cancellationToken);

        if (!remoteInfo.Id.Equals(Guid.Empty)) {
            return new ArtistInfoLookupDto(
                artistName,
                remoteInfo.Annotation,
                remoteInfo.Genres is not null && remoteInfo.Genres.Count != 0
                    ? string.Join(',', remoteInfo.Genres?.Select(r => r.Name))
                    : string.Empty, image,
                image,
                remoteInfo.Id.ToString(),
                remoteInfo.Aliases?
                    .Select(r => r.Name)
                    .ToList());
        }

        throw new ArtistNotFoundException($"Artist: {artistName} not found");
    }

    public async Task<AlbumInfoLookupDto> LookupAlbumInfo(string artistName, string albumName, string albumId,
        CancellationToken cancellationToken = default) {
        var albumQuery = _buildQuery();
        var remoteInfo =
            await albumQuery.FindReleasesAsync($"title:{albumName} AND artist:{artistName}");

        if (remoteInfo is null || remoteInfo.Results.Count == 0) {
            throw new AlbumNotFoundException($"Album: {albumName} not found");
        }

        var details = remoteInfo.Results[0];
        try {
            var id = details.Item.Id;
            var art = await _coverArtService.GetCoverart(id.ToString());
            //TODO: probably shouldn't be caching here
            var cacheFile = Path.Combine(_systemSettings.ImagePath, "album", albumId);
            var file = await HttpHelpers.DownloadFile(art, cacheFile);

            return new AlbumInfoLookupDto(
                artistName,
                details.Item.Title,
                details.Item.Annotation,
                details.Item.Genres
                    .Where(r => !string.IsNullOrEmpty(r.Name))
                    .Select(r => r.Name).ToList(),
                file,
                file,
                details.Item.Id.ToString()
            );
        } catch (Exception e) {
            _logger.LogError("Error looking up album\n\t{Error}", e.Message);
            throw new AlbumNotFoundException($"Album: {albumName} not found");
        }
    }
}
