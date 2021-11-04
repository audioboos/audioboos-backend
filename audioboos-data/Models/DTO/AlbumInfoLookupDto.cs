namespace AudioBoos.Data.Models.DTO;

public record AlbumInfoLookupDto(
    string artistName,
    string Name,
    string Description,
    string Genre,
    string SmallImage,
    string LargeImage,
    string SiteId
);
