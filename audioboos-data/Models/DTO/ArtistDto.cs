#nullable enable

namespace AudioBoos.Data.Models.DTO;

public record ArtistDto() {
    public string? Id { get; set; }
    public string Name { get; set; }
    public string? NormalisedName { get; set; }
    public string? Description { get; set; }
    public string? Genre { get; set; }
    public string? SmallImage { get; set; }
    public string? LargeImage { get; set; }

    string? SiteId { get; set; }
    public DateTime FirstSeen { get; set; }

    private List<string>? Aliases { get; set; }
    public HashSet<AlbumDto>? Albums { get; set; }

    public string? DiscogsId { get; set; } = "";
    public Guid? MusicBrainzId { get; set; } = null;

    public static bool IsNullOrIncomplete(ArtistDto? artistDto) {
        return artistDto is null || string.IsNullOrEmpty(artistDto.Name) ||
               string.IsNullOrEmpty(artistDto.Description) ||
               string.IsNullOrEmpty(artistDto.LargeImage) ||
               string.IsNullOrEmpty(artistDto.SmallImage);
    }
}
