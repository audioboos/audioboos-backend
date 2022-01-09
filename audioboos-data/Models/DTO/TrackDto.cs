#nullable enable
namespace AudioBoos.Data.Models.DTO;

public class TrackDto {
    public TrackDto(string artistName) {
        AlbumName = artistName;
    }

    public string? Id { get; set; }
    public string Name { get; set; }
    public string AlbumName { get; set; }
    public int TrackNumber { get; set; }
    public int Duration { get; set; }
    public string AudioUrl { get; set; }
    public string SiteId { get; set; }

    public static bool IsNullOrIncomplete(TrackDto? trackDto) {
        return trackDto is null || string.IsNullOrEmpty(trackDto.Name);
    }
}
