using ModularSystem.Core.Logging;

namespace ModularSystem.Core.Caching;

public class CacheRecord<T> where T : class, IQueryableModel
{
    public T? Value { get; set; }

    public CacheRecord(T? value = null, TimeSpan? lifetime = null)
    {
        Value = value;
        Lifetime = lifetime ?? TimeSpan.FromMinutes(Cache<T>.DEFAULT_RECORD_LIFETIME_MINUTES);
    }

    public TimeSpan Lifetime { get; set; }
    public DateTime LastUsedAt { get; set; }

    public string Id()
    {
        if (Value == null)
        {
            return "";
        }
        return Value.GetId();
    }

    public bool IsExpired()
    {
        return LastUsedAt + Lifetime < TimeProvider.Now();
    }

    public void SetValue(T? value)
    {
        Value = value;
    }

    public void SetUsed()
    {
        LastUsedAt = TimeProvider.Now();
    }
}

// some testings have revealed that Dictionary key based search is the most efitient.
// search time is described as constant O(1)

/// <summary>
/// Do not forget to call the dispose method before the object goes out of scope, or on references are to be found.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Cache<T> : IDisposable where T : class, IQueryableModel
{
    public const int DEFAULT_RECORD_LIFETIME_MINUTES = 60;
    const int SLEEP_TIME_MILLISECONDS = 100;

    TimeSpan deletionRoutineInterval;

    bool isAlive;
    Dictionary<string, CacheRecord<T>> records;
    Thread thread;
    DateTime deletionRoutineLastRanAt;

    public Cache(TimeSpan? deletionInterval = null)
    {
        deletionRoutineInterval = deletionInterval ?? TimeSpan.FromSeconds(60);
        isAlive = true;
        records = new Dictionary<string, CacheRecord<T>>();
        thread = new Thread(ThreadLoop);

        StartThread();
    }

    /// <summary>
    /// This method must be MANUALLY called by the owner of the resource. If not there will be a memory leak.
    /// </summary>
    public void Dispose()
    {
        // NOTE:
        // The cache object will not be eligible for deallocation by the GC until the cache thread exits.
        // This will only happen when isAlive is set to false. 
        isAlive = false;
    }

    public T? Get(string id)
    {
        var record = GetRecord(id);

        if (record == null)
        {
            return null;
        }

        record.SetUsed();
        return record.Value;
    }

    public void Set(T data, TimeSpan? lifetime = null)
    {
        var id = data.GetId();
        var record = GetRecord(id);

        if (record != null)
        {
            record.SetValue(data);
            record.SetUsed();
        }
        else
        {
            record = new CacheRecord<T>(data, lifetime);
        }

        lock (records)
        {
            records[id] = record;
        }
    }

    public void Delete(string id)
    {
        lock (records)
        {
            records.Remove(id);
        }
    }

    TimeSpan GetDefaultRecordLifetime()
    {
        return TimeSpan.FromHours(DEFAULT_RECORD_LIFETIME_MINUTES);
    }

    CacheRecord<T>? GetRecord(string key)
    {
        var record = new CacheRecord<T>();
        records.TryGetValue(key, out record);
        return record;
    }

    void StartThread()
    {
        thread.Start();
    }

    void ThreadLoop()
    {
        while (isAlive)
        {
            try
            {
                DeleteExpiredRecords();
                Thread.Sleep(SLEEP_TIME_MILLISECONDS);
            }
            catch (Exception e)
            {
                ExceptionLogger.Log(e);
            }
        }
    }

    void DeleteExpiredRecords()
    {
        if (deletionRoutineLastRanAt + deletionRoutineInterval > TimeProvider.Now())
        {
            return;
        }

        foreach (var item in records)
        {
            var record = item.Value;

            if (record.IsExpired() && record.Value == null)
            {
                records.Remove(item.Key);
            }
        }
        deletionRoutineLastRanAt = TimeProvider.Now();
    }
}