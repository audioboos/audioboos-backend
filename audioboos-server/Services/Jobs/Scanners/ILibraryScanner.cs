﻿using System.Threading;
using System.Threading.Tasks;

namespace AudioBoos.Server.Services.Jobs.Scanners;

public interface ILibraryScanner {
    Task<(int, int, int)> ScanLibrary(bool deepScan, string childFolder, CancellationToken cancellationToken);
    Task UpdateArtist(string artistName, CancellationToken cancellationToken);

    Task UpdateUnscannedArtists(CancellationToken cancellationToken);
    Task UpdateUnscannedAlbums(CancellationToken cancellationToken);
    Task UpdateChecksums(CancellationToken cancellationToken);
}
