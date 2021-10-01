using System.Threading.Tasks;
using AudioBoos.Data.Persistence.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AudioBoos.Data.Persistence {
    public class UnitOfWork : IUnitOfWork {
        private readonly AudioBoosContext _context;
        private readonly ILogger<UnitOfWork> _logger;

        public UnitOfWork(AudioBoosContext context, ILogger<UnitOfWork> logger) {
            _logger = logger;
            _context = context;
        }

        public async Task<bool> Complete() {
            try {
                var results = await _context.SaveChangesAsync();
                _logger.LogDebug("Completed unit of work with {Results} changes", results);
                _context.ChangeTracker.Clear();
                return true;
            } catch (DbUpdateException e) {
                _logger.LogError("Error completing unit of work: {Message}\n{InnerMessage}",
                    e.Message,
                    e?.InnerException?.Message
                );
                throw;
            }
        }
    }
}
