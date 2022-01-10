using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data.Interfaces;
using AudioBoos.Data.Models.DTO;
using AudioBoos.Data.Store;
using Microsoft.AspNetCore.Mvc;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace AudioBoos.Server.Controllers;

[ApiController]
// [Authorize]
[Route("[controller]")]
public class ArtistsController : ControllerBase {
    private readonly IAudioRepository<Artist> _artistRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ArtistsController(IAudioRepository<Artist> artistRepository, IUnitOfWork unitOfWork) {
        _artistRepository = artistRepository;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<List<ArtistDto>>> Get() {
        var artists = await _artistRepository
            .GetAll()
            .AsNoTracking()
            .Include(a => a.Albums)
            .ThenInclude(a => a.Tracks)
            .OrderBy(r => r.Name)
            .ToListAsync();
        var results = artists.Adapt<List<ArtistDto>>();
        return Ok(results);
    }

    [HttpGet("{artistName}")]
    public async Task<ActionResult<ArtistDto>> Get(string artistName) {
        var artist = await _artistRepository
            .GetAll()
            .AsNoTracking()
            .Include(a => a.Albums)
            .ThenInclude(a => a.Tracks.OrderBy(t => t.TrackNumber))
            .FirstOrDefaultAsync(r => r.Name.Equals(artistName));
        return artist != null ? Ok(artist.Adapt<ArtistDto>()) : NotFound();
    }

    [HttpPatch]
    public async Task<ActionResult<ArtistDto>> Patch([FromBody] ArtistDto incomingArtist) {
        if (!ModelState.IsValid || string.IsNullOrEmpty(incomingArtist.Id)) {
            return StatusCode(500);
        }

        var artist = await _artistRepository
            .GetById(incomingArtist.Id);
        if (artist is null) {
            return NotFound();
        }

        if (!incomingArtist.Name.Equals(artist.Name)) {
            artist.Name = incomingArtist.Name;
            artist.Immutable = true;
            await _artistRepository.InsertOrUpdate(artist);
            await _unitOfWork.Complete();
        }

        return Ok(artist.Adapt<ArtistDto>());
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<ArtistDto>>> Search([FromQuery] string term) {
        if (string.IsNullOrEmpty(term)) {
            return NoContent();
        }

        var artists = await _artistRepository
            .GetAll()
            .Include(a => a.Albums)
            .ThenInclude(a => a.Tracks.OrderBy(t => t.TrackNumber))
            .Where(r => r.Name.ToLower().Contains(term.ToLower()))
            .ToListAsync();
        return artists != null ? Ok(artists.Adapt<List<ArtistDto>>()) : NoContent();
    }
}
