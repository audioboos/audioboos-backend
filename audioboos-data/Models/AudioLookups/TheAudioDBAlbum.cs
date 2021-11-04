using System.Collections.Generic;

namespace AudioBoos.Data.Models.AudioLookups; 

public class TheAudioDbAlbum {
    public string idAlbum { get; set; }
    public string idArtist { get; set; }
    public string idLabel { get; set; }
    public string strAlbum { get; set; }
    public string strAlbumStripped { get; set; }
    public string strArtist { get; set; }
    public string strArtistStripped { get; set; }
    public string intYearReleased { get; set; }
    public string strStyle { get; set; }
    public string strGenre { get; set; }
    public string strLabel { get; set; }
    public string strReleaseFormat { get; set; }
    public string intSales { get; set; }
    public string strAlbumThumb { get; set; }
    public string strAlbumThumbHQ { get; set; }
    public string strAlbumThumbBack { get; set; }
    public string strAlbumCDart { get; set; }
    public string strAlbumSpine { get; set; }
    public string strAlbum3DCase { get; set; }
    public string strAlbum3DFlat { get; set; }
    public string strAlbum3DFace { get; set; }
    public string strAlbum3DThumb { get; set; }
    public string strDescriptionEN { get; set; }
    public string strDescriptionDE { get; set; }
    public string strDescriptionFR { get; set; }
    public string strDescriptionCN { get; set; }
    public string strDescriptionIT { get; set; }
    public string strDescriptionJP { get; set; }
    public string strDescriptionRU { get; set; }
    public string strDescriptionES { get; set; }
    public string strDescriptionPT { get; set; }
    public string strDescriptionSE { get; set; }
    public string strDescriptionNL { get; set; }
    public string strDescriptionHU { get; set; }
    public string strDescriptionNO { get; set; }
    public string strDescriptionIL { get; set; }
    public string strDescriptionPL { get; set; }
    public string intLoved { get; set; }
    public string intScore { get; set; }
    public string intScoreVotes { get; set; }
    public string strReview { get; set; }
    public string strMood { get; set; }
    public string strTheme { get; set; }
    public string strSpeed { get; set; }
    public string strLocation { get; set; }
    public string strMusicBrainzID { get; set; }
    public string strMusicBrainzArtistID { get; set; }
    public string strAllMusicID { get; set; }
    public string strBBCReviewID { get; set; }
    public string strRateYourMusicID { get; set; }
    public string strDiscogsID { get; set; }
    public string strWikidataID { get; set; }
    public string strWikipediaID { get; set; }
    public string strGeniusID { get; set; }
    public string strLyricWikiID { get; set; }
    public string strMusicMozID { get; set; }
    public string strItunesID { get; set; }
    public string strAmazonID { get; set; }
    public string strLocked { get; set; }
}

public class TheAudioDBAlbumResult {
    public List<TheAudioDbAlbum> album { get; set; }
}