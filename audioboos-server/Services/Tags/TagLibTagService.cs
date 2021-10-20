using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AudioBoos.Server.Services;

namespace AudioBoos.Server.Services.Tags {
    public class TagLibTagService : MustInitialize<string>, ITagService {
        private readonly TagLib.File _file;
        private readonly string _filePath;

        public TagLibTagService(string path) : base(path) {
            _filePath = path;
            _file = TagLib.File.Create(path);
        }

        public void Dispose() {
            _file?.Dispose();
        }

        public string GetFilename() {
            return _file.Name;
        }

        public string GetArtistName() {
            return _file.Tag.FirstAlbumArtist ??
                   _file.Tag.FirstPerformer;
        }

        public string GetArtistDescription() {
            return _file.Tag.FirstAlbumArtist ??
                   _file.Tag.FirstPerformer;
        }

        public string GetAlbumName() {
            return _file.Tag.Album;
        }

        public string GetAlbumCatalogueNumber() {
            return string.Empty;
        }

        public string GetTrackName() {
            return _file.Tag.Title;
        }

        public string GetTrackComments() {
            return _file.Tag.Comment;
        }

        public int GetTrackNumber() {
            return Convert.ToInt32(_file.Tag.Track);
        }

        public async Task<string> GetChecksum() {
            using var md5 = MD5.Create();
            await using var stream = System.IO.File.OpenRead(_filePath);
            var hash = await md5.ComputeHashAsync(stream);
            return BitConverter
                .ToString(hash)
                .Replace("-", "")
                .ToLowerInvariant();
        }

        public bool SetArtistName(string artist) {
            throw new NotImplementedException();
        }

        public bool SetAlbumName(string album) {
            throw new NotImplementedException();
        }

        public bool SetTrackTitle(string title) {
            throw new NotImplementedException();
        }
    }
}
