#nullable enable

namespace AudioBoos.Data.Models.DTO; 

public record ArtistDto(
    string Name,
    string? Description,
    string? Genre,
    string? SmallImage,
    string? LargeImage,
    string? SiteId,
    List<string>? Aliases = null,
    List<AlbumDto>? Albums = null,
    string? Id = null
) {
    public static bool IsNullOrIncomplete(ArtistDto? artistDto) {
        return artistDto is null || string.IsNullOrEmpty(artistDto.Name) ||
               string.IsNullOrEmpty(artistDto.Description) ||
               string.IsNullOrEmpty(artistDto.LargeImage) ||
               string.IsNullOrEmpty(artistDto.SmallImage);
    }
}