﻿using System;
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
    private readonly IAudioRepository<Artist> _artistRepository;
    private readonly IAudioRepository<Album> _albumRepository;
    private readonly SystemSettings _systemSettings;

    public ImageController(IOptions<SystemSettings> systemSettings, IAudioRepository<Artist> artistRepository,
        IAudioRepository<Album> albumRepository) {
        _artistRepository = artistRepository;
        _albumRepository = albumRepository;
        _systemSettings = systemSettings.Value;
    }

    [HttpGet("artist/{artistId}")]
    public async Task<IActionResult> GetArtistImage(string artistId, [FromQuery] string type = "small") {
        var cacheFile = Path.Combine(_systemSettings.ImagePath, "artist", artistId);
        if (System.IO.File.Exists(cacheFile)) {
            return _getFileDirect(cacheFile);
        }

        var artist = await _artistRepository.GetById(artistId);
        if (artist is null) return NotFound();

        var image = type.Equals("small") ? artist.SmallImage : artist.LargeImage;

        if (!string.IsNullOrEmpty(image) && image.StartsWith("http")) {
            await ImageCacher.CacheImage(cacheFile, image);
            if (System.IO.File.Exists(cacheFile)) {
                return _getFileDirect(cacheFile);
            }
        }

        //TODO: switch??
        return string.IsNullOrEmpty(image)
            ? type.Equals("small") ?
                File(await TextImageGenerator.CreateArtistAvatarImage(artist.Name), "image/png") :
                File(await TextImageGenerator.CreateArtistImage(artist.Name), "image/png")
            : Ok(image);
    }

    [HttpGet("album/{albumId}")]
    public async Task<IActionResult> GetAlbumImage(string albumId, [FromQuery] string type) {
        var cacheFile = Path.Combine(_systemSettings.ImagePath, "album", albumId);
        if (System.IO.File.Exists(cacheFile)) {
            return _getFileDirect(cacheFile);
        }

        var album = await _albumRepository.GetAll()
            .Where(a => a.Id.Equals(Guid.Parse(albumId)))
            .Include(a => a.Artist)
            .FirstOrDefaultAsync();
        if (album is null) return NotFound();

        var image = type.Equals("small") ? album.SmallImage : album.LargeImage;

        return string.IsNullOrEmpty(image)
            ? File(await TextImageGenerator.CreateAlbumImage(album.Artist.Name, album.Name), "image/png")
            : _getFileDirect(image);
    }

    private IActionResult _getFileDirect(string path) {
        if (!System.IO.File.Exists(path)) {
            return NotFound();
        }

        var res = File(System.IO.File.OpenRead(path), "image/jpeg");
        res.EnableRangeProcessing = true;
        return res;
    }
}
