using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AudioBoos.Data.Extensions;
using AudioBoos.Data.Persistence;
using AudioBoos.Data.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AudioBoos.Data.Access; 

public class AudioFileRepository : AbstractRepository<AudioFile> {
    private readonly ILogger<AudioFileRepository> _logger;

    public AudioFileRepository(AudioBoosContext context, ILogger<AudioFileRepository> logger) : base(context) {
        _logger = logger;
    }

    public override async Task<AudioFile?> GetByFile(string name, CancellationToken cancellationToken = default) {
        var file = await this._context.AudioFiles
            .AsNoTrackingWithIdentityResolution()
            .Where(a => a.PhysicalPath.ToLower().Equals(name.ToLower()))
            .FirstOrDefaultAsync(cancellationToken);
        return file;
    }

    public override async Task<AudioFile> InsertOrUpdate(AudioFile entity,
        CancellationToken cancellationToken = default) {
        var existing =
            await _context.AudioFiles
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    f => f.PhysicalPath.Equals(entity.PhysicalPath),
                    cancellationToken: cancellationToken);

        if (existing is not null) {
            _logger.LogDebug("Entity: {Existing} found, updating existing", existing.Id);
            entity.Id = existing.Id;
        }

        this._context.Update(entity);
        return entity;
    }
}