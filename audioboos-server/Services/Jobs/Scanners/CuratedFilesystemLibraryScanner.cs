/*
namespace AudioBoos.Server.Services.Jobs.Scanners {
    /// <summary>
    /// This scanner will work perfectly for me but probably not for other users
    /// Will probably also be super shitty if we move to a SAAS offering
    /// Assumes a rigid <artist>/<album>/<track[]> folder structure 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    // public class CuratedFilesystemLibraryScanner : ILibraryScanner {
    //     private readonly ILogger<CuratedFilesystemLibraryScanner> _logger;
    //     private readonly IUnitOfWork _unitOfWork;
    //     private readonly AudioBoosContext _context;
    //     private readonly IAudioLookupService _lookupService;
    //     private readonly SystemSettings _systemSettings;
    //     private readonly SemaphoreSlim __scanLock = new(1, 1);
    //
    //     public CuratedFilesystemLibraryScanner(ILogger<CuratedFilesystemLibraryScanner> logger,
    //         IUnitOfWork unitOfWork, AudioBoosContext context,
    //         IAudioLookupService lookupService, IOptions<SystemSettings> systemSettings) {
    //         _logger = logger;
    //         _unitOfWork = unitOfWork;
    //         _context = context;
    //         _lookupService = lookupService;
    //         _systemSettings = systemSettings.Value;
    //     }
    //
    //     public async Task<(int, int, int)> ScanLibrary(CancellationToken cancellationToken) {
    //         _logger.LogInformation("Starting library scan");
    //
    //         int artistScans = 0;
    //         int albumScans = 0;
    //         int trackScans = 0;
    //         //First guess at this is...
    //
    //         //1. Find every folder that contains an audio file
    //         //2. Filter these to only include folders that have no child folders
    //         //              (this will filter out random mp3 files kicking about)
    //         //3. Collect their underpants
    //         //4. ????
    //         //5. Profit!!!
    //
    //         //first stab... looks good on white board, pretty sure it will suck!
    //         await __scanLock.WaitAsync(cancellationToken);
    //         try {
    //             var dirList = (await FileSystemHelpers
    //                     .GetDirectoriesAsync(_systemSettings.AudioPath))
    //                 .Where(d => d.Contains("Hauschka"));
    //             //HUGELY naive implementation... 
    //             //get artists
    //             foreach (var dir in dirList) {
    //                 artistScans++;
    //                 try {
    //                     _logger.LogDebug($"Scanning:  {dir}");
    //                     var artistName = dir.GetBaseName();
    //                     _logger.LogDebug($"\tArtist:  {artistName}");
    //
    //                     var artist = await _context.Artists
    //                         .Where(a => a.Name.Equals(artistName))
    //                         .FirstOrDefaultAsync(cancellationToken);
    //
    //                     var artistDto = artist is null ? ArtistDTO.Default(artistName) : artist.Adapt<ArtistDTO>();
    //
    //                     var artistDetails = await _lookupService.LookupArtistInfo(artistName, cancellationToken);
    //
    //                     artist = new Artist(artistDetails?.Name ?? artistName) {
    //                         LargeImage = artistDetails?.LargeImage,
    //                         SmallImage = artistDetails?.SmallImage,
    //                         Description = artistDetails?.Description,
    //                         Aliases = artistDetails?.Aliases,
    //                         DiscogsId = artistDetails?.SiteId
    //                     };
    //                     await _context.Artists
    //                         .Upsert(artist)
    //                         .On(v => v.Name)
    //                         .RunAsync(cancellationToken);
    //
    //                     //get albums
    //                     foreach (var albumFolder in await FileSystemHelpers.GetDirectoriesAsync(dir)) {
    //                         albumScans++;
    //                         // var fileList = folder.GetAllAudioFiles();
    //                         var albumResult =
    //                             await _lookupService.LookupAlbumInfo(artistName, albumFolder.GetBaseName(),
    //                                 string.Empty, cancellationToken);
    //                         var album = new Album(artist, albumResult.Name) {
    //                             Description = albumResult.Description,
    //                             LargeImage = albumResult.LargeImage,
    //                             SmallImage = albumResult.SmallImage
    //                         };
    //                         await _context.Albums
    //                             .Upsert(album)
    //                             .On(v => v.Name)
    //                             .RunAsync(cancellationToken);
    //                     }
    //
    //                     await _unitOfWork.Complete();
    //                 } catch (ArtistNotFoundException e) {
    //                     //TODO: dunno.... guess we should do something??
    //                     _logger.LogError(e.Message);
    //                 } catch (AlbumNotFoundException e) {
    //                     _logger.LogError(e.Message);
    //                 } catch (Exception e) {
    //                     _logger.LogError(e.Message);
    //                 }
    //             }
    //         } finally {
    //             __scanLock.Release();
    //         }
    //
    //         return (artistScans, albumScans, trackScans);
    //     }
    // }
}
*/
