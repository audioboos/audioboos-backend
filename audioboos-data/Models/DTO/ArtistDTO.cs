#nullable enable

namespace AudioBoos.Data.Models.DTO {
    public record ArtistDTO(
        string Name,
        string? Description,
        string? Genre,
        string? SmallImage,
        string? LargeImage,
        string? SiteId,
        List<string>? Aliases = null,
        string? Id = null
    ) {
        public static bool IsNullOrIncomplete(ArtistDTO? artistDto) {
            return artistDto is null || string.IsNullOrEmpty(artistDto.Name) ||
                   string.IsNullOrEmpty(artistDto.Description) ||
                   string.IsNullOrEmpty(artistDto.LargeImage) ||
                   string.IsNullOrEmpty(artistDto.SmallImage);
        }
    }
}
