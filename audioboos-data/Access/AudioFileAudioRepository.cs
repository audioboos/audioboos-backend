using AudioBoos.Data;
using AudioBoos.Data.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AudioBoos.Data.Access;

public class AudioFileAudioRepository : AbstractAudioRepository<AudioFile> {
    private readonly ILogger<AudioFileAudioRepository> _logger;

    public AudioFileAudioRepository(AudioBoosContext context, ILogger<AudioFileAudioRepository> logger) : base(context) {
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
