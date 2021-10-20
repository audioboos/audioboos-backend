using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data.Models.DTO;
using AudioBoos.Data.Store;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AudioBoos.Server.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class AlbumsController : ControllerBase {
        private readonly IRepository<Album> _albumsRepository;

        public AlbumsController(IRepository<Album> albumsRepository) {
            _albumsRepository = albumsRepository;
        }

        [HttpGet("{artistName}")]
        public async Task<ActionResult<List<AlbumDto>>> Get(string artistName) {
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
}
