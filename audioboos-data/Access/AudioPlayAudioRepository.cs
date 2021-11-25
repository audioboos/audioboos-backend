using AudioBoos.Data.Store;

namespace AudioBoos.Data.Access;

public class AudioPlayAudioRepository : AbstractRepository<AudioPlay> {
    public AudioPlayAudioRepository(AudioBoosContext context) : base(context) {
    }

    public override async Task<AudioPlay> InsertOrUpdate(AudioPlay entity,
        CancellationToken cancellationToken = default) {
        this._context.AudioPlays.Add(entity);
        return await Task.FromResult(entity);
    }
}
