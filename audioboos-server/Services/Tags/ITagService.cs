using System;
using System.Threading.Tasks;

namespace AudioBoos.Server.Services.Tags {
    interface ITagService : IDisposable {
        string GetFilename();
        string GetArtistName();
        string GetAlbumCatalogueNumber();
        string GetAlbumName();
        string GetTrackName();
        string GetTrackComments();
        int GetTrackNumber();
        Task<string> GetChecksum();

        bool SetTrackTitle(string title);
        bool SetArtistName(string artist);
        bool SetAlbumName(string album);
    }
}
