namespace AudioBoos.Data.Models.DTO;

public record AlbumInfoLookupDto(
    string artistName,
    string Name,
    string Description,
    List<string>? Genres,
    string SmallImage,
    string LargeImage
) {
    public int? DiscogsId { get; set; }
    public Guid? MusicBrainzId { get; set; }
}
