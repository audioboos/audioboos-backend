#nullable enable
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AudioBoos.Data;
using AudioBoos.Data.Store;
using Microsoft.EntityFrameworkCore;

namespace AudioBoos.Data.Access;

public abstract class AbstractRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity {
    protected readonly AudioBoosContext _context;
    private DbSet<TEntity> _entities;

    public AudioBoosContext Context { get => _context; }

    protected AbstractRepository(AudioBoosContext context) {
        this._context = context;
        _entities = context.Set<TEntity>();
    }

    public DbSet<TEntity> GetAll() {
        return _entities;
    }

    public async Task<TEntity?> GetById(string id, CancellationToken cancellationToken = default) {
        return await GetById(Guid.Parse(id), cancellationToken);
    }

    public async Task<TEntity?> GetById(Guid id, CancellationToken cancellationToken = default) {
        return await _entities
            .SingleOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public abstract Task<TEntity> InsertOrUpdate(TEntity entity,
        CancellationToken cancellationToken = default);

    public async Task Delete(Guid id, CancellationToken cancellationToken = default) {
        if (id.Equals(Guid.Empty)) throw new ArgumentNullException(nameof(id));
        var entity = await _entities.SingleOrDefaultAsync(s => s.Id == id, cancellationToken);
        if (entity != null) {
            _entities.Remove(entity);
        }
    }
}

public abstract class AbstractAudioRepository<TEntity> : AbstractRepository<TEntity>, IAudioRepository<TEntity>
    where TEntity : BaseAudioEntity {
    public AbstractAudioRepository(AudioBoosContext context) : base(context) {
    }

    public async Task<TEntity?> GetByName(string name, CancellationToken cancellationToken = default) {
        return await this._context.Set<TEntity>()
            .FirstOrDefaultAsync(e =>
                    e.Name.ToUpper().Equals(name.ToUpper()),
                cancellationToken);
    }

    public async Task<TEntity?> GetByName(string name, Func<IQueryable<TEntity>, IQueryable<TEntity>> func,
        CancellationToken cancellationToken = default) {
        DbSet<TEntity> result = this._context.Set<TEntity>();
        return await func(result)
            .AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(e =>
                e.Name.ToUpper().Equals(name.ToUpper()), cancellationToken);
    }

    public abstract Task<TEntity?> GetByFile(string fileName, CancellationToken cancellationToken = default);

    public async Task<TEntity?> GetByAlternativeNames(CancellationToken cancellationToken = default,
        params string[] names) {
        var results = (await this._context.Set<TEntity>()
                .AsNoTrackingWithIdentityResolution()
                .ToListAsync(cancellationToken))
            .FirstOrDefault(t => t
                .AlternativeNames
                .Select(c => c.ToString())
                .Intersect(names).Any());

        return results;
    }
}
