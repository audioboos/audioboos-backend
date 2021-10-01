using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AudioBoos.Data.Models.DTO;
using AudioBoos.Data.Models.Settings;
using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.CoverArt;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AudioBoos.Server.Services.AudioLookup {
    public class MusicBrainzLookupService : IAudioLookupService {
        private readonly ILogger<MusicBrainzLookupService> _logger;
        private readonly SystemSettings _settings;

        public MusicBrainzLookupService(IOptions<SystemSettings> settings, ILogger<MusicBrainzLookupService> logger) {
            _logger = logger;
            _settings = settings.Value;
        }


        private Query _buildQuery() => new Query(
            _settings.Hostname,
            Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0",
            _settings.ContactEmail);

        private CoverArt _buildCoverArt() => new CoverArt(
            _settings.Hostname,
            Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0",
            _settings.ContactEmail);

        public Task<ArtistDTO> LookupArtistInfo(string artistName, CancellationToken cancellationToken = default) {
            _logger.LogDebug("Looking up artist info: {ArtistName}", artistName);
            throw new NotImplementedException();
            /*
            var artistQuery = _buildQuery();
            var coverArt = _buildCoverArt();

            var remoteInfo = currentInfo.SiteId.Equals(Guid.Empty)
                ? (await artistQuery.FindArtistsAsync(artistName, 1)).Results[0].Item
                : (await artistQuery.LookupArtistAsync(Guid.Parse(currentInfo.SiteId)));

            if (string.IsNullOrEmpty(currentInfo.LargeImage) ||
                string.IsNullOrEmpty(currentInfo.SmallImage) && !remoteInfo.Id.Equals(Guid.Empty)) {
                try {
                    var art = await coverArt.FetchGroupFrontAsync(remoteInfo.Id);
                    //TODO: Fix this...
                } catch (Exception e) {
                    _logger.LogError($"Error looking up cover art for {artistName}\n\t{e.Message}");
                }
            }

            if (!remoteInfo.Id.Equals(Guid.Empty)) {
                return new ArtistDTO(
                    Guid.NewGuid(),
                    artistName,
                    SiteId: remoteInfo.Id.ToString(),
                    Aliases: remoteInfo.Aliases?
                        .Select(r => r.Name)
                        .ToList());
            }

            throw new ArtistNotFoundException($"Artist: {artistName} not found");
            */
        }

        public Task<AlbumDTO> LookupAlbumInfo(string artistName, string albumName,
            CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }
    }
}
