using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioBoos.Data.Access;
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

    public ArtistsController(IAudioRepository<Artist> artistRepository) {
        _artistRepository = artistRepository;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<ArtistDto>>> Get() {
        var artists = await _artistRepository
            .GetAll()
            .Include(a => a.Albums)
            .OrderBy(r => r.Name)
            .ToListAsync();
        var results = artists.Adapt<List<ArtistDto>>();
        return Ok(results);
    }

    [HttpGet("{artistName}")]
    public async Task<ActionResult<ArtistDto>> Get(string artistName) {
        var artist = await _artistRepository
            .GetAll()
            .Include(a => a.Albums)
            .ThenInclude(a => a.Tracks.OrderBy(t => t.TrackNumber))
            .FirstOrDefaultAsync(r => r.Name.Equals(artistName));
        return artist != null ? Ok(artist.Adapt<ArtistDto>()) : NotFound();
    }
}
