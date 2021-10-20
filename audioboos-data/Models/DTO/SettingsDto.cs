namespace AudioBoos.Data.Models.DTO {
    public class SettingsDto {
        public SettingsDto(string siteName) {
            this.SiteName = siteName;
        }

        public string SiteName { get; set; }
    }
}
