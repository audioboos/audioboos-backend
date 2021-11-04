using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AudioBoos.Data.Models.AudioLookups;
using AudioBoos.Data.Models.DTO;
using AudioBoos.Server.Services.Exceptions.AudioLookup;
using Microsoft.Extensions.Logging;

namespace AudioBoos.Server.Services.AudioLookup; 
// ReSharper disable once InconsistentNaming

// public class TheAudioDBLookupService : IAudioLookupService {
//     private readonly ILogger<TheAudioDBLookupService> _logger;
//     private readonly IHttpClientFactory _httpClientFactory;
//
//     public TheAudioDBLookupService(ILogger<TheAudioDBLookupService> logger, IHttpClientFactory httpClientFactory) {
//         _logger = logger;
//         _httpClientFactory = httpClientFactory;
//     }
//
//     public async Task<ArtistDTO>
//         LookupArtistInfo(string artistName, CancellationToken cancellationToken = default) {
//         var request = $"search.php?s={Uri.EscapeDataString(artistName)}";
//         using var client = _httpClientFactory.CreateClient("theaudiodb");
//         try {
//             var response = await client.GetAsync(request, cancellationToken);
//             response.EnsureSuccessStatusCode();
//
//             var results = await JsonSerializer.DeserializeAsync<TheAudioDBArtistResult>(
//                 await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
//             if (results?.artists != null && (results.artists.Count != 0)) {
//                 var artist = results.artists.FirstOrDefault();
//                 return new ArtistDTO(
//                     artist?.strArtist ?? "Unknown Artist",
//                     artist?.strBiographyEN,
//                     artist?.strGenre,
//                     artist?.strArtistThumb,
//                     artist?.strArtistLogo,
//                     artist?.strMusicBrainzID
//                 );
//             }
//         } catch (HttpRequestException e) {
//             _logger.LogError($"Error getting info for artist: {artistName}\n\t{e.Message}");
//         }
//
//         throw new ArtistNotFoundException(
//             $"Artist {artistName} not found in theaudiodb\n\tRequest: {client.BaseAddress}/{request}");
//     }
//
//     public async Task<AlbumDTO> LookupAlbumInfo(string artistName, string albumName,
//         CancellationToken cancellationToken = default) {
//         var request = $"searchalbum.php?s={Uri.EscapeDataString(artistName)}&a={Uri.EscapeDataString(albumName)}";
//         using var client = _httpClientFactory.CreateClient("theaudiodb");
//         try {
//             var response = await client.GetAsync(request);
//             response.EnsureSuccessStatusCode();
//
//             var results = await JsonSerializer.DeserializeAsync<TheAudioDBAlbumResult>(
//                 await response.Content.ReadAsStreamAsync()
//             );
//             if (results?.album != null && (results.album.Count != 0)) {
//                 var album = results.album.FirstOrDefault();
//                 return new AlbumDTO(album?.strArtist) {
//                     Name = album?.strAlbum,
//                     Description = album?.strDescriptionEN,
//                     LargeImage = album?.strAlbumCDart,
//                     SmallImage = album?.strAlbumThumb,
//                 };
//             }
//         } catch (HttpRequestException e) {
//             _logger.LogError($"Error getting info for artist: {artistName}\n\t{e.Message}");
//         }
//
//         throw new AlbumNotFoundException(
//             $"Album {albumName} for artist {artistName} not found in theaudiodb\n\tRequest: {client.BaseAddress}/{request}");
//     }
// }