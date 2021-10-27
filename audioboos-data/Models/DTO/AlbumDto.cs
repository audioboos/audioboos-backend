using System;
using System.Collections.Generic;
using Mapster;

#nullable enable
namespace AudioBoos.Data.Models.DTO; 

public record AlbumDto {
    public AlbumDto(string artistName) {
        ArtistName = artistName;
    }

    public string? Id { get; set; }
    public string Name { get; set; }
    public string ArtistName { get; set; }
    public string Description { get; set; }
    public string SmallImage { get; set; }
    public string LargeImage { get; set; }

    public DateTime ReleaseDate { get; set; }

    public List<TrackDto>? Tracks { get; set; }
    public string? SiteId { get; set; }

    public static bool IsNullOrIncomplete(AlbumDto? albumDto) {
        return albumDto is null || string.IsNullOrEmpty(albumDto.ArtistName) ||
               string.IsNullOrEmpty(albumDto.Name) ||
               string.IsNullOrEmpty(albumDto.Description) ||
               string.IsNullOrEmpty(albumDto.LargeImage) ||
               string.IsNullOrEmpty(albumDto.SmallImage);
    }
}