public class Thumbnails {
    public string large { get; set; }
    public string small { get; set; }
}

public class CoverArtImage {
    public bool approved { get; set; }
    public bool back { get; set; }
    public string comment { get; set; }
    public long edit { get; set; }
    public bool front { get; set; }
    public string id { get; set; }
    public string image { get; set; }
    public Thumbnails thumbnails { get; set; }
    public List<string> types { get; set; }
}


public class CoverArtSearchResult {
    public List<CoverArtImage> images { get; set; }
    public string release { get; set; }
}
