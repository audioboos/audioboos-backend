using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AudioBoos.Data.Extensions;
using AudioBoos.Data.Persistence;
using AudioBoos.Data.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AudioBoos.Data.Access; 

public class ArtistRepository : AbstractRepository<Artist> {
    private readonly ILogger<ArtistRepository> _logger;

    public ArtistRepository(AudioBoosContext context, ILogger<ArtistRepository> logger) : base(context) {
        _logger = logger;
    }

    public override async Task<Artist?> GetByFile(string name, CancellationToken cancellationToken = default) {
        var artist = await this._context.Tracks
            .Where(a => a.PhysicalPath.ToLower().Equals(name.ToLower()))
            .Select(r => r.Album.Artist)
            .FirstOrDefaultAsync(cancellationToken);
        return artist;
    }

    public override async Task<Artist>
        InsertOrUpdate(Artist entity, CancellationToken cancellationToken = default) {
        var existing =
                await _context.Artists
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        a => a.Name.Equals(entity.Name),
                        cancellationToken: cancellationToken)
            ;

        if (existing is not null) {
            entity.Id = existing.Id;
        }

        this._context.Update(entity);
        return entity;
    }
}