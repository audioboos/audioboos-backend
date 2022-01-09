using System.Net;

namespace AudioBoos.Data.Store;

public sealed record TrackPlayLog : BaseEntity {
    public AppUser? User { get; set; }
    public Track Track { get; set; }

    public IPAddress IPAddress { get; set; }
}
