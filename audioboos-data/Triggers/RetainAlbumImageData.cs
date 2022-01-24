using AudioBoos.Data.Store;
using EntityFrameworkCore.Triggered;

namespace AudioBoos.Data.Triggers;

public class RetainAlbumImageData : IBeforeSaveTrigger<Album> {
    public Task BeforeSave(ITriggerContext<Album> context, CancellationToken cancellationToken) {
        //prevent perfectly good images being overwritten when the remote lookup doesn't get any images
        if (context.ChangeType == ChangeType.Modified) {
            if (string.IsNullOrEmpty(context.Entity.LargeImage) &&
                !string.IsNullOrEmpty(context.UnmodifiedEntity?.LargeImage)) {
                context.Entity.LargeImage = context.UnmodifiedEntity.LargeImage;
            }

            if (string.IsNullOrEmpty(context.Entity.SmallImage) &&
                !string.IsNullOrEmpty(context.UnmodifiedEntity?.SmallImage)) {
                context.Entity.SmallImage = context.UnmodifiedEntity.SmallImage;
            }
        }

        return Task.CompletedTask;
    }
}
