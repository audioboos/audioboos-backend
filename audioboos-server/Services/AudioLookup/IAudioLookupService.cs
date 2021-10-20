using System.Threading;
using System.Threading.Tasks;
using AudioBoos.Data.Models.DTO;

namespace AudioBoos.Server.Services.AudioLookup {
    public interface IAudioLookupService {
        public Task<ArtistInfoLookupDto> LookupArtistInfo(string artistName, CancellationToken cancellationToken);

        public Task<AlbumInfoLookupDTO> LookupAlbumInfo(string artistName, string albumName, CancellationToken cancellationToken);
    }
}
