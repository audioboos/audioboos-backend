#nullable enable
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AudioBoos.Data.Store;

[Index(nameof(Name), IsUnique = true)]
public record Artist(string Name) : BaseAudioEntity(Name) {
    public string? Style { get; set; }
    public string? Genre { get; set; }
    public string? SmallImage { get; set; }
    public string? LargeImage { get; set; }
    public string? HeaderImage { get; set; }

    public string? Fanart { get; set; }
    public string? MusicBrainzId { get; set; }
    public string? DiscogsId { get; set; }
    public List<string>? Aliases { get; set; }

    public ICollection<Album>? Albums { get; set; }

    public static bool IsIncomplete(BaseAudioEntity audioEntity) =>
        !BaseAudioEntity.IsIncomplete(audioEntity) ||
        !string.IsNullOrEmpty((audioEntity as Artist)?.SmallImage) ||
        !string.IsNullOrEmpty((audioEntity as Artist)?.LargeImage);

    public string GetNormalisedName() => (Name.ToLower().StartsWith("the") ? Name.Remove(0, 3) : Name).Trim();

    public string GetImageFile() => $"{Id}.jpg";
}
