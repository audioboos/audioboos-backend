using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AudioBoos.Data.Store; 

[Index(nameof(ArtistId), nameof(Name), IsUnique = true)]
public record Album : BaseEntity {
    public Album(string name) : base(name) {
    }

    public Album(Artist artist, string name) : base(name) {
        this.Artist = artist;
        this.ArtistId = artist.Id;
    }

    public DateTime? ReleaseDate { get; set; }

    public string? SiteId { get; set; }
    public string? SmallImage { get; set; }
    public string? LargeImage { get; set; }

    public ICollection<Track> Tracks { get; set; }

    [Required] public Guid ArtistId { get; set; }
    [Required] public Artist Artist { get; set; }
}