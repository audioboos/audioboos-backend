﻿using System.Threading;
using System.Threading.Tasks;
using AudioBoos.Data.Models.DTO;

namespace AudioBoos.Server.Services.AudioLookup;

public interface IAudioLookupService {
    string Name { get; }
    Task<ArtistInfoLookupDto> LookupArtistInfo(string artistName, CancellationToken cancellationToken = default);

    Task<AlbumInfoLookupDto> LookupAlbumInfo(string artistName, string albumName, string artistId,
        CancellationToken cancellationToken);
}
