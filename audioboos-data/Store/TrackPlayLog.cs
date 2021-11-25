using System.Net;

namespace AudioBoos.Data.Store;

public record TrackPlayLog : BaseEntity {
    public virtual AppUser? User { get; set; }
    public virtual Track Track { get; set; }

    public IPAddress IPAddress { get; set; }
}
