using System.ComponentModel.DataAnnotations;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AudioBoos.Data.Store;

[Index(nameof(ArtistId), nameof(Name), IsUnique = true)]
public record Album : BaseAudioEntity {
    public Album(string name) : base(name) {
    }

    public Album(Artist artist, string name) : base(name) {
        this.Artist = artist;
        this.ArtistId = artist.Id;
    }

    public DateTime? ReleaseDate { get; set; }

    public string? SiteId { get; set; }
    [AdaptIgnore] public string? SmallImage { get; set; }
    [AdaptIgnore] public string? LargeImage { get; set; }

    public List<Track> Tracks { get; set; }
    public List<string>? Genres { get; set; }

    [Required] public Guid ArtistId { get; set; }
    [Required] public Artist Artist { get; set; }

    public int? DiscogsId { get; set; }
    public Guid? MusicBrainzId { get; set; }

    /// <summary>
    /// Album has been edited well, we don't want it in the scans anymore
    /// </summary>
    public bool Immutable { get; set; }

    public static bool IsIncomplete(BaseAudioEntity audioEntity) =>
        !BaseAudioEntity.IsIncomplete(audioEntity) ||
        !string.IsNullOrEmpty((audioEntity as Album)?.SmallImage) ||
        !string.IsNullOrEmpty((audioEntity as Album)?.LargeImage);
}
