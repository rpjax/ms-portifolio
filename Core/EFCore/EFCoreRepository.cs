﻿using Microsoft.EntityFrameworkCore;
using ModularSystem.Core;
using ModularSystem.Core.Linq;
using ModularSystem.EntityFramework.Linq;
using System.Linq.Expressions;

namespace ModularSystem.EntityFramework.Repositories;

/// <summary>
/// Represents a repository implementation using Entity Framework Core.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
public class EFCoreRepository<T> : IRepository<T> where T : class, IEFEntity
{
    /// <summary>
    /// Gets the underlying <see cref="DbContext"/> instance used by the repository.
    /// </summary>
    protected DbContext DbContext { get; }

    /// <summary>
    /// Gets the <see cref="DbSet{TEntity}"/> instance used by the repository.
    /// </summary>
    protected DbSet<T> DbSet { get; }

    /// <summary>
    /// Gets the settings object for the repository.
    /// </summary>
    protected SettingsObject Settings { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EFCoreRepository{T}"/> class.
    /// </summary>
    /// <param name="dbContext">The <see cref="DbContext"/> instance.</param>
    /// <param name="settings">The optional settings object.</param>
    public EFCoreRepository(DbContext dbContext, SettingsObject? settings = null)
    {
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        DbContext = dbContext;
        DbSet = dbContext.Set<T>();
        Settings = settings ?? new SettingsObject();
    }

    /// <summary>
    /// Disposes the <see cref="DbContext"/> instance.
    /// </summary>
    public virtual void Dispose()
    {
        DbContext.Dispose();
    }

    /// <summary>
    /// Returns an <see cref="IAsyncQueryable{T}"/> representing the entities in the repository.
    /// </summary>
    /// <returns>An <see cref="IAsyncQueryable{T}"/> instance.</returns>
    public IAsyncQueryable<T> AsQueryable()
    {
        return new EFCoreAsyncQueryable<T>(DbSet.AsQueryable());
    }

    /// <summary>
    /// Creates a new entity in the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task CreateAsync(T entity)
    {
        DbSet.Add(entity);
        return DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Creates multiple entities in the repository asynchronously.
    /// </summary>
    /// <param name="entities">The entities to create.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task CreateAsync(IEnumerable<T> entities)
    {
        DbSet.AddRange(entities);
        return DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Updates an entity in the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task UpdateAsync(T entity)
    {
        DbSet.Attach(entity);
        DbContext.Entry(entity).State = EntityState.Modified;
        return DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Updates entities in the repository asynchronously based on the specified update operation.
    /// </summary>
    /// <param name="update">The update operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task<long> UpdateAsync(IUpdate<T> update)
    {
        var op = new EFCoreUpdateOperation<T>(
            dbSet: DbSet,
            allowUpdatesWithNoFilter: Settings.AllowUpdatesWithNoFilter
        );

        return op.ExecuteAsync(update);
    }

    /// <summary>
    /// Deletes an entity from the repository asynchronously.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task DeleteAsync(T entity)
    {
        var id = entity.Id;

        return DbSet
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync();
    }

    /// <summary>
    /// Deletes entities from the repository asynchronously based on the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter the entities.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<long> DeleteAsync(Expression<Func<T, bool>> predicate)
    {
        return await DbSet.Where(predicate).ExecuteDeleteAsync();
    }

    /// <summary>
    /// Represents the settings object for the repository.
    /// </summary>
    public class SettingsObject
    {
        /// <summary>
        /// Gets or sets a value indicating whether updates are allowed without a filter.
        /// </summary>
        public bool AllowUpdatesWithNoFilter { get; set; } = false;
    }
}
