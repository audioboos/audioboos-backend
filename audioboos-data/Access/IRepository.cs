﻿#nullable enable
using AudioBoos.Data.Store;
using Microsoft.EntityFrameworkCore;

namespace AudioBoos.Data.Access;

public interface IRepository<TEntity> where TEntity : BaseEntity {
    public AudioBoosContext Context { get; }
    DbSet<TEntity> GetAll();
    Task<TEntity?> GetById(string id, CancellationToken cancellationToken = default);
    Task<TEntity?> GetById(Guid id, CancellationToken cancellationToken = default);

    Task<TEntity> InsertOrUpdate(TEntity entity, CancellationToken cancellationToken = default);

    Task Delete(Guid id, CancellationToken cancellationToken = default);
}

public interface IAudioRepository<TEntity> : IRepository<TEntity> where TEntity : BaseAudioEntity {
    Task<TEntity?> GetByName(string name, CancellationToken cancellationToken = default);

    Task<TEntity?> GetByName(string name, Func<IQueryable<TEntity>, IQueryable<TEntity>> func,
        CancellationToken cancellationToken = default);

    Task<TEntity?> GetByFile(string fileName, CancellationToken cancellationToken = default);
    Task<TEntity?> GetByAlternativeNames(CancellationToken cancellationToken = default, params string[] names);
}
