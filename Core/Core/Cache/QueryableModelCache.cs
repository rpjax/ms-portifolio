using ModularSystem.Core.Threading;
using System.Collections.Concurrent;

namespace ModularSystem.Core.Caching;

/// <summary>
/// Do not forget to call the dispose method before the object goes out of scope, or on references are to be found.
/// </summary>
/// <typeparam name="T"></typeparam>
public class QueryableModelCache<T> : IDisposable where T : class, IQueryableModel
{
    public static readonly TimeSpan DefaultRecordLifetime = TimeSpan.FromMinutes(60);
    public static readonly TimeSpan DefaultDeletionRoutineDelay = TimeSpan.FromMinutes(1);

    public long Size { get => Records.Count; }
    public long SizeLimit { get; private set; }

    private ConcurrentDictionary<string, CacheRecord<T>> Records { get; set; }

    private TimeSpan RecordLifetime { get; set; }
    private TimeSpan DeletionRoutineDelay { get; set; }
    private DateTime DeletionRoutineLastRanAt { get; set; }
    private TaskRoutine DeletionRoutine { get; }

    public QueryableModelCache(long sizeLimit)
    {
        SizeLimit = sizeLimit;
        Records = new();
        RecordLifetime = DefaultRecordLifetime;
        DeletionRoutineDelay = DefaultDeletionRoutineDelay;
        DeletionRoutineLastRanAt = TimeProvider.UtcNow();
        DeletionRoutine = new DeletionTaskRoutine(this, DefaultDeletionRoutineDelay);

        DeletionRoutine.Start();
    }

    /// <summary>
    /// This method must be MANUALLY called by the owner of the resource. If not there will be a memory leak.
    /// </summary>
    public void Dispose()
    {
        DeletionRoutine.Dispose();
    }

    public QueryableModelCache<T> SetRecordLifetime(TimeSpan recordLifetime)
    {
        RecordLifetime = recordLifetime;
        return this;
    }

    public QueryableModelCache<T> SetDeletionRoutineDelay(TimeSpan deletionRoutineDelay)
    {
        DeletionRoutineDelay = deletionRoutineDelay;
        return this;
    }

    public bool IsFull()
    {
        return Records.Count >= SizeLimit;
    }

    public IQueryable<T> AsQueryable()
    {
        return Records.Values
            .AsQueryable()
            .Where(x => x.Value != null)
            .Select(x => x.Value!);
    }

    public T? TryGet(string key)
    {
        return TryGetRecord(key)?.Value;
    }

    public bool TrySet(T data, TimeSpan? lifetime = null)
    {
        var id = data.GetId();
        var record = TryGetRecord(id);

        if (record != null)
        {
            record.SetValue(data);
        }
        else
        {
            record = new CacheRecord<T>(data, lifetime ?? RecordLifetime);
        }

        if (IsFull())
        {
            return false;
        }

        Records[id] = record;

        return true;
    }

    public void Set(T data, TimeSpan? lifetime = null)
    {
        if(!TrySet(data, lifetime))
        {
            throw new Exception("Failed to add the record to the cache because it would surpass the maximum allowed size.");
        }
    }

    public T? Remove(string key)
    {
        Records.Remove(key, out var value);
        return value?.Value;
    }

    public void Clear()
    {
        Records.Clear();
    }

    public void DisableDeletionRoutine()
    {
        DeletionRoutine.Stop();
    }

    private CacheRecord<T>? TryGetRecord(string key)
    {
        if (Records.TryGetValue(key, out var record))
        {
            record.UpdateLastUsedAt();
            return record;
        }

        return null;
    }

    internal class CacheRecord<TValue> where TValue : class, IQueryableModel
    {
        public TValue? Value { get; set; }
        public TimeSpan Lifetime { get; set; }
        public DateTime LastUsedAt { get; set; }
        public string? RecordId => Value?.GetId();

        public CacheRecord(TValue? value, TimeSpan lifetime)
        {
            Value = value;
            Lifetime = lifetime;
        }

        public bool IsExpired()
        {
            return LastUsedAt + Lifetime < TimeProvider.UtcNow();
        }

        public void SetValue(TValue? value)
        {
            Value = value;
        }

        public void UpdateLastUsedAt()
        {
            LastUsedAt = TimeProvider.UtcNow();
        }
    }

    internal class DeletionTaskRoutine : TaskRoutine
    {
        private QueryableModelCache<T> Cache { get; }

        public DeletionTaskRoutine(QueryableModelCache<T> cache, TimeSpan delay) : base(delay)
        {
            Cache = cache;
        }

        protected override Task OnExecuteAsync(CancellationToken cancellationToken)
        {
            if (Cache.DeletionRoutineLastRanAt + Cache.DeletionRoutineDelay > TimeProvider.UtcNow())
            {
                return Task.CompletedTask;
            }

            foreach (var item in Cache.Records)
            {
                var record = item.Value;

                if (record.IsExpired())
                {
                    Cache.Remove(item.Key);
                }
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }

            Cache.DeletionRoutineLastRanAt = TimeProvider.UtcNow();

            return Task.CompletedTask;
        }
    }
}