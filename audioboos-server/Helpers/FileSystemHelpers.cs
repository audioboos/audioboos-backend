using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AudioBoos.Server.Helpers {
    public static class FileSystemHelpers {
        static string[] audioFileExtensions = {
            ".WAV", ".MID", ".MIDI", ".WMA", ".MP3", ".OGG", ".RMA"
        };

        public static Task<string[]> GetDirectoriesAsync(string path) {
            var results = Directory.GetDirectories(path);

            return Task.FromResult(results);
        }

        public static Task<List<string>> GetFilesAsync(this string path, string filter = "*.*",
            SearchOption searchOptions = SearchOption.AllDirectories) {
            var results = filter.Equals("*.*")
                ? Directory.GetFiles(path)
                : Directory.GetFiles(path, filter, searchOptions);
            return Task.FromResult(results.ToList());
        }

        public static string GetBaseName(this string path) {
            return Path.GetFileName(path);
        }

        public static Task<List<string>> GetAllAudioFiles(this string path) {
            return path.GetFilesAsync("*.mp3");
        }

        public static bool IsAudioFile(this string path) {
            return -1 != Array.IndexOf(
                audioFileExtensions,
                Path.GetExtension(path).ToUpperInvariant()
            );
        }

        public static string GetAlbumArt(this string path, string type = "Large") {
            string[] checks = {
                $"AlbumArt_*_{type}.jpg",
                $"AlbumArt_*_{type}.jpeg",
                $"AlbumArt{type}.jpg",
                $"AlbumArt{type}.jpeg",
                "folder.jpg",
                "folder.jpeg"
            };
            foreach (var check in checks) {
                var files = Directory.GetFiles(path, check, SearchOption.TopDirectoryOnly);
                if (files.Length != 0) {
                    return files[0];
                }
            }

            return string.Empty;
        }

        public static string GetAbsolutePath(this string path) {
            return new FileInfo(path).FullName;
        }
    }
}
