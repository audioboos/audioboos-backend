using System.Net;

namespace AudioBoos.Data.Store;

public record AudioPlay : BaseEntity {
    public virtual AppUser? User { get; set; }
    public virtual AudioFile File { get; set; }

    public IPAddress IPAddress { get; set; }
}
