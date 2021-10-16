using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AudioBoos.Data.Store {
    [Index(nameof(PhysicalPath), IsUnique = true)]
    public record AudioFile(
        [Required] string PhysicalPath,
        string ID3ArtistName,
        string ID3AlbumName,
        string ID3TrackName
    ) : BaseEntity(ID3AlbumName) {
        public string Checksum { get; set; }

        public Artist? Artist { get; set; }
        public Album? Album { get; set; }
        public Track? Track { get; set; }
    }
}
