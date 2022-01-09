namespace AudioBoos.Data.Models.DTO;

public record TrackPlayDto {
    public string TrackName { get; set; }
    public DateTime DatePlayed { get; set; }
    public string PlayedByUser { get; set; }
    public string PlayedByIp { get; set; }
}
