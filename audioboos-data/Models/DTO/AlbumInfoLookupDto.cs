namespace AudioBoos.Data.Models.DTO;

public record AlbumInfoLookupDTO(
    string artistName,
    string Name,
    string Description,
    string SiteId,
    string SmallImage,
    string LargeImage) {
}
