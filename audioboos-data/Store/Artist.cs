#nullable enable
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AudioBoos.Data.Store {
    [Index(nameof(Name), IsUnique = true)]
    public record Artist(string Name) : BaseEntity(Name) {
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

        public string GetNormalisedName() => (Name.ToLower().StartsWith("the") ? Name.Remove(0, 3) : Name).Trim();
    }
}
