namespace AudioBoos.Data.Models.DTO {
    // public record SettingsDTO {
    //     public string SiteName { get; }
    //
    //     public SettingsDTO(string? siteName) => (SiteName) = (siteName);
    // }
    public class SettingsDTO {
        public SettingsDTO(string siteName) {
            this.SiteName = siteName;
        }

        public string SiteName { get; set; }
    }
}
