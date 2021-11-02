using System.Threading;
using System.Threading.Tasks;
using AudioBoos.Data.Models.DTO;

namespace AudioBoos.Server.Services.AudioLookup;

public interface IAudioLookupService {
    string Name { get; }
    Task<ArtistInfoLookupDto> LookupArtistInfo(string artistName, CancellationToken cancellationToken);

    Task<AlbumInfoLookupDto> LookupAlbumInfo(string artistName, string albumName, string albumId,
        CancellationToken cancellationToken);
}
