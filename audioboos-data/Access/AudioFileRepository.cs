using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AudioBoos.Data.Extensions;
using AudioBoos.Data.Persistence;
using AudioBoos.Data.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AudioBoos.Data.Access {
    public class AudioFileRepository : AbstractRepository<AudioFile> {
        private readonly ILogger<AudioFileRepository> _logger;

        public AudioFileRepository(AudioBoosContext context, ILogger<AudioFileRepository> logger) : base(context) {
            _logger = logger;
        }

        public override async Task<AudioFile> GetByFile(string name, CancellationToken cancellationToken = default) {
            var file = await this._context.AudioFiles
                .Where(a => a.PhysicalPath.ToLower().Equals(name.ToLower()))
                .FirstOrDefaultAsync(cancellationToken);
            return file;
        }

        public override async Task<AudioFile> InsertOrUpdate(AudioFile entity,
            CancellationToken cancellationToken = default) {
            return await _context.AudioFiles.AddOrUpdate(
                _context,
                model =>
                    model.PhysicalPath.Equals(entity.PhysicalPath) ||
                    model.Id.Equals(entity.Id),
                entity,
                _logger);
        }
    }
}
