using System.Threading.Tasks;
using AudioBoos.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AudioBoos.Data;

public class UnitOfWork : IUnitOfWork {
    private readonly AudioBoosContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private int _lastResultCount;

    public int LastResultCount => _lastResultCount;

    public UnitOfWork(AudioBoosContext context, ILogger<UnitOfWork> logger) {
        _logger = logger;
        _context = context;
    }

    public DbContextId __contextId() {
        return _context.ContextId;
    }

    public async Task<bool> Complete() {
        try {
            _lastResultCount = await _context.SaveChangesAsync();
            _logger.LogDebug("Completed unit of work with {Results} changes", _lastResultCount);
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
