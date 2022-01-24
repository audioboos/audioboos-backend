namespace AudioBoos.Data.Models.DTO;

public record TrackPlayDto {
    public string ArtistName { get; set; }
    public string AlbumName { get; set; }
    public string TrackName { get; set; }
    public int TotalPlays { get; set; }
    public DateTime DatePlayed { get; set; }
    public string PlayedByUser { get; set; }
    public string PlayedByIp { get; set; }
}
