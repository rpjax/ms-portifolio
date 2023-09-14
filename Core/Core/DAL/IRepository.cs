namespace ModularSystem.Core;

/// <summary>
/// Very simple CRUD implementation.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IRepository<T> where T : class
{
    IQueryable<T> AsQueryable();

    Task<bool> GetIdIsValid(string id);
    // CREATE
    Task<T> CreateAsync(T value);
    // READ
    Task<T?> GetAsync(string id);
    // UPDATE
    Task UpdateAsync(T value);
    // DELETE
    Task DeleteAsync(string id);
}

class CacheRepository<T> : IRepository<T>, IDisposable where T : class
{
    protected CacheRepository<T> cache;

    public CacheRepository()
    {
        cache = new CacheRepository<T>();
    }

    public void Dispose()
    {
        cache.Dispose();
    }

    public IQueryable<T> AsQueryable()
    {
        throw new NotImplementedException();
    }

    public Task<T> CreateAsync(T value)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> GetIdIsValid(string id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(T value)
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetAsync(string id)
    {
        throw new NotImplementedException();
    }
}