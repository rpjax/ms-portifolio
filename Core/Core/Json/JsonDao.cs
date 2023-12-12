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
    JsonStorage<DatabaseFile<T>> Storage { get; }
    DatabaseFile<T> File { get; }

    public JsonDao(string fileName)
    {
        Storage = new JsonStorage<DatabaseFile<T>>(LocalStorage.GetFileInfo(fileName), true);
        File = Storage.Read()!;
    }

    public void Dispose()
    {
        Save();
    }

    public void Save()
    {
        Storage.Write(File);
    }

    public Task<string> InsertAsync(T data)
    {
        var id = File.CreateId().ToString();

        data.SetId(id);
        File.Data.Add(data);
        return Task.FromResult(id);
    }

    public Task InsertAsync(IEnumerable<T> entries)
    {
        foreach (var entry in entries)
        {
            File.Data.Add(entry);
        }

        return Task.CompletedTask;
    }

    public Task<T> GetAsync(string id)
    {
        var record = AsQueryable().Where(x => x.GetId() == id).First();
        return Task.FromResult(record);
    }

    public async Task<IQueryResult<T>> QueryAsync(IQuery<T> query)
    {
        var queryable = AsQueryable();
        var reader = new QueryReader<T>(query);
        var predicate = reader.GetFilterExpression();

        if (predicate != null)
        {
            queryable = queryable.Where(predicate);
        }

        var data = queryable
            .Skip(reader.IntOffset)
            .Take(reader.IntLimit)
            .ToArray();

        var result = new QueryResult<T>()
        {
            Data = data,
            Pagination = new PaginationOut()
            {
                Limit = reader.Limit,
                Offset = reader.Offset,
                Total = queryable.LongCount()
            }
        };

        return result;
    }

    public async Task UpdateAsync(T data)
    {
        T record = await GetAsync(data.GetId());
        int index = File.Data.IndexOf(record);

        record = data;
        File.Data.RemoveAt(index);
        File.Data.Insert(index, record);
    }

    public async Task DeleteAsync(string id)
    {
        var record = await GetAsync(id);
        var index = File.Data.IndexOf(record);
        File.Data.RemoveAt(index);
    }

    public Task<long?> DeleteAsync(Expression<Func<T, bool>> predicate)
    {
        File.Data = File.Data.RemoveWhere(predicate.Compile());
        Storage.Write(File);
        return null;
    }

    public Task DeleteAllAsync()
    {
        File.Data.Clear();
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
        return File.Data.AsQueryable();
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
        long count = File.Data.Count;
        return Task.FromResult(count);
    }

    public bool ValidateIdFormat(string id)
    {
        if (int.TryParse(id, out int num))
        {
            return num <= File.IdCounter;
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

    public Task<long?> UpdateAsync(IUpdate<T> update)
    {
        throw new NotImplementedException();
    }
}