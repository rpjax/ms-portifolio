using ModularSystem.Core.Helpers;
using System.Linq.Expressions;

namespace ModularSystem.Core;

public class DatabaseFile<T>
{
    public int IdCounter { get; set; }
    public List<T> Data { get; set; } = new List<T>();

    public int CreateId()
    {
        IdCounter++;
        return IdCounter;
    }

    public void Add(T value) => Data.Add(value);
}

public class JsonDao<T> : IDataAccessObject<T> where T : class, IQueryableModel
{
    JsonStorage<DatabaseFile<T>> storage;
    DatabaseFile<T> file;

    public JsonDao(string fileName)
    {
        storage = new JsonStorage<DatabaseFile<T>>(LocalStorage.GetFileInfo(fileName), true);
        file = storage.Read()!;
    }

    public void Dispose()
    {
        Save();
    }

    public void Save()
    {
        storage.Write(file);
    }

    public Task<string> InsertAsync(T data)
    {
        var id = file.CreateId().ToString();

        data.SetId(id);
        file.Data.Add(data);
        return Task.FromResult(id);
    }

    public Task InsertAsync(IEnumerable<T> entries)
    {
        foreach (var entry in entries)
        {
            file.Data.Add(entry);
        }

        return Task.CompletedTask;
    }

    public Task<T> GetAsync(string id)
    {
        var record = AsQueryable().Where(x => x.GetId() == id).First();
        return Task.FromResult(record);
    }

    public async Task<IQueryResult<T>> QueryAsync(IQuery<T> request)
    {
        var query = AsQueryable();

        if (request.Filter != null)
        {
            query = query.Where(request.Filter);
        }

        var data = new QueryResult<T>()
        {
            Data = query.Skip(request.Pagination.Offset).Take(request.Pagination.Limit).ToList(),
            Pagination = new PaginationOut()
            {
                Limit = request.Pagination.Limit,
                Offset = request.Pagination.Offset,
                Total = query.LongCount()
            }
        };

        return data;
    }

    public async Task UpdateAsync(T data)
    {
        T record = await GetAsync(data.GetId());
        int index = file.Data.IndexOf(record);

        record = data;
        file.Data.RemoveAt(index);
        file.Data.Insert(index, record);
    }

    public async Task DeleteAsync(string id)
    {
        var record = await GetAsync(id);
        var index = file.Data.IndexOf(record);
        file.Data.RemoveAt(index);
    }

    public Task DeleteAsync(Expression<Func<T, bool>> predicate)
    {
        file.Data = file.Data.RemoveWhere(predicate.Compile());
        storage.Write(file);
        return Task.CompletedTask;
    }

    public Task DeleteAllAsync()
    {
        file.Data.Clear();
        return Task.CompletedTask;
    }

    public async Task SoftDeleteAsync(string id)
    {
        T record = await GetAsync(id);
        record.IsSoftDeleted = true;
        await UpdateAsync(record);
    }

    public IQueryable<T> AsQueryable()
    {
        return file.Data.AsQueryable();
    }

    public Task<long> CountAsync(string id)
    {
        var count = AsQueryable().LongCount(x => x.GetId() == id);
        return Task.FromResult(count);
    }

    public Task<long> CountAsync(Expression<Func<T, bool>> selector)
    {
        var count = AsQueryable().LongCount(selector);
        return Task.FromResult(count);
    }

    public Task<long> CountAllAsync()
    {
        long count = file.Data.Count;
        return Task.FromResult(count);
    }

    public bool ValidateIdFormat(string id)
    {
        if (int.TryParse(id, out int num))
        {
            return num <= file.IdCounter;
        }

        return false;
    }

    public async Task<bool> ValidateIdAsync(string id)
    {
        if (ValidateIdFormat(id))
        {
            var count = await CountAsync(id);
            return count > 0;
        }

        return false;
    }

    public Task UpdateAsync<TField>(Expression<Func<T, bool>> selector, Expression<Func<T, TField?>> fieldSelector, TField? value)
    {
        throw new NotImplementedException();
    }
}