namespace AudioBoos.Data.Models.Settings;

public class SystemSettings {
    public string WebClientUrl { get; set; }
    public string DefaultSiteName { get; set; }
    public string CachePath { get; set; }
    public string ContactEmail { get; set; }

    public string ImagePath => Path.Combine(CachePath, "images");
}
