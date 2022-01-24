namespace AudioBoos.Data.Models.DTO;

public record ArtistInfoLookupDto(
    string Name,
    string? Description,
    string? Genre,
    string? SmallImage,
    string? LargeImage,
    string? SiteId,
    List<string>? Aliases
) {
    public int? DicogsId { get; set; }
    public Guid? MusicBrainzId { get; set; }
}
