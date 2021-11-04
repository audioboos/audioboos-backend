using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AudioBoos.Data.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AudioBoos.Data.Extensions; 

public static class DbSetExtensions {
    // public static async Task<TEntity> AddOrUpdate<TEntity>(this DbSet<TEntity> dbSet, DbContext context,
    //     Expression<Func<TEntity, bool>> predicate, TEntity entity, ILogger logger) where TEntity : BaseEntity {
    //     try {
    //         TEntity? result = await dbSet
    //             .AsNoTracking()
    //             .FirstOrDefaultAsync(predicate);
    //         if (result != null) {
    //             entity.Id = result.Id;
    //             dbSet.Update(entity);
    //             return entity;
    //         }
    //
    //         dbSet.Add(entity);
    //         return entity;
    //     } catch (Exception e) {
    //         logger.LogError("Error adding entity {Message}", e.Message);
    //         throw;
    //     }
    // }
}