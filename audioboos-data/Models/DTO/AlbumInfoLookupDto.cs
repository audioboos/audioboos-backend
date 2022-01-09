namespace AudioBoos.Data.Models.DTO;

public record AlbumInfoLookupDto(
    string artistName,
    string Name,
    string Description,
    List<string>? Genres,
    string SmallImage,
    string LargeImage,
    string SiteId
);
