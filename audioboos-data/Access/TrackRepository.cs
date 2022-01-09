using AudioBoos.Data.Store;
using Microsoft.EntityFrameworkCore;

namespace AudioBoos.Data.Access;

public class TrackRepository : AbstractAudioRepository<Track> {
    public TrackRepository(AudioBoosContext context) : base(context) { }

    public override async Task<Track?> GetByFile(string fileName, CancellationToken cancellationToken = default) {
        return await _context.Tracks
            .AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(
                t => t.PhysicalPath.Equals(fileName),
                cancellationToken);
    }

    public override async Task<Track> InsertOrUpdate(Track entity, CancellationToken cancellationToken = default) {
        var existing = await _context
            .Tracks
            .AsNoTracking()
            .FirstOrDefaultAsync(
                e => e.PhysicalPath.Equals(entity.PhysicalPath),
                cancellationToken);
        if (existing is not null) {
            entity.Id = existing.Id;
        }

        this._context.Update(entity);
        return entity;
    }
}
