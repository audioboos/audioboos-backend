using System.Collections.Generic;

namespace AudioBoos.Data.Models.AudioLookups; 

public class _DiscogsArtistSearchResultInner {
    public int id { get; set; }
}

public class DiscogsArtistSearchResult {
    public List<_DiscogsArtistSearchResultInner> results { get; set; }
}

public class Image {
    public string type { get; set; }
    public string uri { get; set; }
    public string resource_url { get; set; }
    public string uri150 { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}

public class Alias {
    public int id { get; set; }
    public string name { get; set; }
    public string resource_url { get; set; }
    public string thumbnail_url { get; set; }
}

public class DiscogsArtistResult {
    public string name { get; set; }
    public int id { get; set; }
    public string resource_url { get; set; }
    public string uri { get; set; }
    public string releases_url { get; set; }
    public List<Image> images { get; set; }
    public string realname { get; set; }
    public string profile { get; set; }
    public List<string> urls { get; set; }
    public List<string> namevariations { get; set; }
    public string data_quality { get; set; }
    public List<Alias>? aliases { get; set; }
}