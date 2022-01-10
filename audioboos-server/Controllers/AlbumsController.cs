using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data.Interfaces;
using AudioBoos.Data.Models.DTO;
using AudioBoos.Data.Store;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AudioBoos.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AlbumsController : ControllerBase {
    private readonly IAudioRepository<Album> _albumsRepository;
    private readonly IUnitOfWork _unitOfWork;


    public AlbumsController(IAudioRepository<Album> albumsRepository, IUnitOfWork unitOfWork) {
        _albumsRepository = albumsRepository;
        _unitOfWork = unitOfWork;
    }

    //TODO: API methods are a bit unclear here
    //TODO: endpoint is /album{***} but most methods return plural 
    [HttpGet("{artistName}")]
    public async Task<ActionResult<List<AlbumDto>>> GetForArtist(string artistName) {
        var albums = await _albumsRepository
            .GetAll()
            .Include(a => a.Artist)
            .Include(a => a.Tracks)
            .Where(a => a.Artist.Name.Equals(artistName))
            .ToListAsync();

        //TODO: This should be ordered by album release date
        var response = albums
            .OrderBy(a => a.Name)
            .Adapt<List<AlbumDto>>();

        return response;
    }

    [HttpPatch]
    public async Task<ActionResult<AlbumDto>> Patch([FromBody] AlbumDto incomingAlbum) {
        if (!ModelState.IsValid || string.IsNullOrEmpty(incomingAlbum.Id)) {
            return StatusCode(500);
        }

        var album = await _albumsRepository
            .GetAll()
            .Include(a => a.Artist)
            .FirstOrDefaultAsync(a => a.Id.Equals(Guid.Parse(incomingAlbum.Id)));

        if (album is null) {
            return NotFound();
        }

        if (!incomingAlbum.Name.Equals(album.Name)) {
            album.Name = incomingAlbum.Name;
            album.Immutable = true;
            _albumsRepository.InsertOrUpdate(album);
            await _unitOfWork.Complete();
        }

        return Ok(album.Adapt<AlbumDto>());
    }

    [HttpGet("")]
    public async Task<ActionResult<List<AlbumDto>>> GetAss() {
        var albums = await _albumsRepository
            .GetAll()
            .Include(a => a.Artist)
            .Include(a => a.Tracks)
            .ToListAsync();

        //TODO: This should be ordered by album release date
        var response = albums
            .OrderBy(a => a.Name)
            .Adapt<List<AlbumDto>>();

        return response;
    }

    [HttpGet("{artistName}/{albumName}")]
    public async Task<ActionResult<AlbumDto>> GetSingle(string artistName, string albumName) {
        var album = await _albumsRepository
            .GetAll()
            .Include(a => a.Tracks)
            .Include(a => a.Artist)
            .Where(a => a.Artist.Name.Equals(artistName))
            .FirstOrDefaultAsync(a => a.Name.Equals(albumName));

        if (album is null) {
            return NotFound();
        }

        album.Tracks = album.Tracks.OrderBy(c => c.TrackNumber).ToList();

        return Ok(album.Adapt<AlbumDto>());
    }
}
