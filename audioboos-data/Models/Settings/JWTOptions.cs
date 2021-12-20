namespace AudioBoos.Data.Models.Settings;

public class JWTOptions {
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public string Secret { get; set; }
    public TimeSpan Lifetime { get; set; }
}
