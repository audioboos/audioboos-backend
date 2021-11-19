using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data.Models.Settings;
using AudioBoos.Data.Store;
using AudioBoos.Server.Services.Images;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AudioBoos.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase {
    private readonly IRepository<Artist> _artistRepository;
    private readonly IRepository<Album> _albumRepository;
    private readonly SystemSettings _systemSettings;

    public ImageController(IOptions<SystemSettings> systemSettings, IRepository<Artist> artistRepository,
        IRepository<Album> albumRepository) {
        _artistRepository = artistRepository;
        _albumRepository = albumRepository;
        _systemSettings = systemSettings.Value;
    }

    [HttpGet("artist/{artistId}")]
    public async Task<IActionResult> GetArtistImage(string artistId, [FromQuery] string type = "small") {
        var artist = await _artistRepository.GetById(artistId);
        return File(await TextImageGenerator.CreateArtistImage(artist.Name), "image/png");
        /*
        var cacheFile = Path.Combine(_systemSettings.ImagePath, "artist", artistId);
        if (System.IO.File.Exists(cacheFile)) {
            return GetFileDirect(cacheFile);
        }

        var artist = await _artistRepository.GetById(artistId);
        if (artist is null) return NotFound();

        var image = type.Equals("small") ? artist.SmallImage : artist.LargeImage;

        //TODO: switch??
        return string.IsNullOrEmpty(image)
            ? type.Equals("small") ?
                File(await TextImageGenerator.CreateArtistAvatarImage(artist.Name), "image/png") :
                File(await TextImageGenerator.CreateArtistImage(artist.Name), "image/png")
            : Ok(image);*/
    }

    [HttpGet("album/{albumId}")]
    public async Task<IActionResult> GetAlbumImage(string albumId, [FromQuery] string type) {
        var cacheFile = Path.Combine(_systemSettings.ImagePath, "album", albumId);
        if (System.IO.File.Exists(cacheFile)) {
            return GetFileDirect(cacheFile);
        }

        var album = await _albumRepository.GetAll()
            .Where(a => a.Id.Equals(Guid.Parse(albumId)))
            .Include(a => a.Artist)
            .FirstOrDefaultAsync();
        if (album is null) return NotFound();

        var image = type.Equals("small") ? album.SmallImage : album.LargeImage;

        return string.IsNullOrEmpty(image)
            ? File(await TextImageGenerator.CreateAlbumImage(album.Artist.Name, album.Name), "image/png")
            : GetFileDirect(image);
    }

    public async Task<IActionResult> GeneratePlaceholderImage(string artistName, string albumName) {
        var image = await TextImageGenerator.CreateAlbumImage(artistName, albumName);
        return File(image, "image/png");
    }

    public IActionResult GetFileDirect(string path) {
        if (!System.IO.File.Exists(path)) {
            return NotFound();
        }

        var res = File(System.IO.File.OpenRead(path), "image/jpeg");
        res.EnableRangeProcessing = true;
        return res;
    }
}
