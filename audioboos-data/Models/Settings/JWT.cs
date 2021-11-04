namespace AudioBoos.Data.Models.Settings; 

public class JWT {
    public string ValidAudience { get; set; }
    public string ValidIssuer { get; set; }
    public string Secret { get; set; }
}