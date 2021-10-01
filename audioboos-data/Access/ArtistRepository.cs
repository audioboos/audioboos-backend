using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AudioBoos.Data.Extensions;
using AudioBoos.Data.Persistence;
using AudioBoos.Data.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AudioBoos.Data.Access {
    public class ArtistRepository : AbstractRepository<Artist> {
        private readonly ILogger<ArtistRepository> _logger;

        public ArtistRepository(AudioBoosContext context, ILogger<ArtistRepository> logger) : base(context) {
            _logger = logger;
        }

        public override async Task<Artist?> GetByFile(string name, CancellationToken cancellationToken = default) {
            var artist = await this._context.Tracks
                .Where(a => a.PhysicalPath.ToLower().Equals(name.ToLower()))
                .Select(r => r.Album.Artist)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
            return artist;
        }

        public override async Task<Artist>
            InsertOrUpdate(Artist entity, CancellationToken cancellationToken = default) {
            await _context.Artists.AddOrUpdate(
                _context,
                model => model.Id.Equals(entity.Id) ||
                         model.Name.Equals(entity.Name),
                entity,
                _logger);

            return entity;
        }
    }
}
