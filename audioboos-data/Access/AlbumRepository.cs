using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AudioBoos.Data.Extensions;
using AudioBoos.Data.Persistence;
using AudioBoos.Data.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AudioBoos.Data.Access {
    public class AlbumRepository : AbstractRepository<Album> {
        private readonly ILogger<AlbumRepository> _logger;

        public AlbumRepository(AudioBoosContext context, ILogger<AlbumRepository> logger) : base(context) {
            _logger = logger;
        }

        public override async Task<Album?> GetByFile(string name, CancellationToken cancellationToken = default) {
            var album = await this._context.Tracks
                .Where(a => a.PhysicalPath.ToLower().Equals(name.ToLower()))
                .Include(r => r.Album.Artist)
                .Select(r => r.Album)
                .FirstOrDefaultAsync(cancellationToken);
            return album;
        }

        public override async Task<Album> InsertOrUpdate(Album entity, CancellationToken cancellationToken = default) {
            var existing =
                await _context.Albums
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        a => a.Name.Equals(entity.Name) && a.Artist.Id.Equals(entity.ArtistId),
                        cancellationToken: cancellationToken);

            if (existing is not null) {
                entity.Id = existing.Id;
            }

            this._context.Update(entity);
            return entity;
        }
    }
}
