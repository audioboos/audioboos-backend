using System.Collections.Generic;

namespace AudioBoos.Data.Models.AudioLookups; 

public class DiscogsAlbumSearchResult {
    public List<Result> results { get; set; }
}

public class UserData {
    public bool in_wantlist { get; set; }
    public bool in_collection { get; set; }
}

public class Community {
    public int want { get; set; }
    public int have { get; set; }
}

public class Format {
    public string name { get; set; }
    public string qty { get; set; }
    public List<string> descriptions { get; set; }
    public string text { get; set; }
}

public class Result {
    public string country { get; set; }
    public string year { get; set; }
    public List<string> format { get; set; }
    public List<string> label { get; set; }
    public string type { get; set; }
    public List<string> genre { get; set; }
    public List<string> style { get; set; }
    public int id { get; set; }
    public List<string> barcode { get; set; }
    public UserData user_data { get; set; }
    public int master_id { get; set; }
    public string master_url { get; set; }
    public string uri { get; set; }
    public string catno { get; set; }
    public string title { get; set; }
    public string thumb { get; set; }
    public string cover_image { get; set; }
    public string resource_url { get; set; }
    public Community community { get; set; }
    public int format_quantity { get; set; }
    public List<Format> formats { get; set; }
}