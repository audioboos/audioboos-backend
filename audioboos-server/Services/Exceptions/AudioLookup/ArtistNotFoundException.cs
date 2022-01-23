using System;

namespace AudioBoos.Server.Services.Exceptions.AudioLookup;

public class CoverArtNotFoundException : Exception {
    public CoverArtNotFoundException(string message) : base(message) {
    }
}

public class AlbumNotFoundException : Exception {
    public AlbumNotFoundException(string message) : base(message) {
    }
}

public class ArtistNotFoundException : Exception {
    public ArtistNotFoundException(string message) : base(message) {
    }
}
