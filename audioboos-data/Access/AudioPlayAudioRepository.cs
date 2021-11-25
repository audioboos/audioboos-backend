using AudioBoos.Data.Store;

namespace AudioBoos.Data.Access;

public class AudioPlayAudioRepository : AbstractRepository<TrackPlayLog> {
    public AudioPlayAudioRepository(AudioBoosContext context) : base(context) {
    }

    public override async Task<TrackPlayLog> InsertOrUpdate(TrackPlayLog entity,
        CancellationToken cancellationToken = default) {
        this._context.TrackPlayLogs.Add(entity);
        return await Task.FromResult(entity);
    }
}
