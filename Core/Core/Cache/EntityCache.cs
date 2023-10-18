using ModularSystem.Core.Threading;
using System.Collections.Concurrent;

namespace ModularSystem.Core.Caching;


/// <summary>
/// Represents a cache for storing entities of a given type.
/// </summary>
/// <typeparam name="T">The type of entity to be cached.</typeparam>
public class EntityCache<T> : IDisposable where T : class
{
    /// <summary>
    /// Specifies the strategies for invalidating cache records.
    /// </summary>
    public enum InvalidationStrategyType
    {
        None,
        LastUsageLifetime,
        CreationLifetime
    }

    public static readonly TimeSpan DefaultRecordLifetime = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets the current size of the cache.
    /// </summary>
    public long Size { get => Records.Count; }

    /// <summary>
    /// Gets or sets the maximum size or capacity of the cache.
    /// </summary>
    public long Capacity { get; set; }

    /// <summary>
    /// Gets or sets the lifetime of a record in the cache.
    /// </summary>
    public TimeSpan RecordLifetime { get; set; }

    /// <summary>
    /// Gets or sets the strategy used for invalidating cache records.
    /// </summary>
    public InvalidationStrategyType InvalidationStrategy { get; set; }

    private ConcurrentDictionary<string, CacheRecord> Records { get; set; }

    private object SetLock { get; } = new object();

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityCache{T}"/> class with a specified maximum size.
    /// </summary>
    /// <param name="maxSize">The maximum size of the cache.</param>
    public EntityCache(long maxSize)
    {
        Capacity = maxSize;
        RecordLifetime = DefaultRecordLifetime;
        InvalidationStrategy = InvalidationStrategyType.LastUsageLifetime;
        Records = new();
    }

    public void Dispose()
    {
        
    }

    public EntityCache<T> EmptyCopy(long? maxSize = null)
    {
        return new EntityCache<T>(maxSize ?? Capacity)
            .SetInvalidationStrategy(InvalidationStrategy)
            .SetRecordLifetime(RecordLifetime);
    }

    public EntityCache<T> SetRecordLifetime(TimeSpan recordLifetime)
    {
        RecordLifetime = recordLifetime;
        return this;
    }

    public EntityCache<T> SetInvalidationStrategy(InvalidationStrategyType strategyType)
    {
        InvalidationStrategy = strategyType;
        return this;
    }

    public IQueryable<T> AsQueryable()
    {
        return Records.Values
            .AsQueryable()
            .Where(x => x.Value != null)
            .Select(x => x.Value!);
    }

    public bool IsFull()
    {
        return Records.Count >= Capacity;
    }

    public CacheRecord? TryGetRecord(string key)
    {
        if (Records.TryGetValue(key, out var record))
        {
            record.UpdateLastUsedAt();
            record.UpdateDeletionTask();
            return record;
        }

        return null;
    }

    public T? TryGet(string key)
    {
        return TryGetRecord(key)?.Value;
    }

    public bool TrySet(string key, T data, TimeSpan? lifetime = null)
    {
        var record = TryGetRecord(key);

        if (record != null)
        {
            record.SetValue(data);
            record.UpdateLastUsedAt();
            record.UpdateDeletionTask();
            return true;
        }

        lock (SetLock)
        {
            if (IsFull())
            {
                return false;
            }
        }

        if(record == null)
        {
            record = new CacheRecord(this, key, data, lifetime ?? RecordLifetime);
        }

        Records[key] = record;
        return true;
    }

    public T? Remove(string key)
    {
        Records.Remove(key, out var value);
        value?.DeletionTask?.Cancel();
        return value?.Value;
    }

    public T? Remove(CacheRecord record)
    {
        return Remove(record.Key);
    }

    public void Clear()
    {
        foreach (var record in Records.Values)
        {
            Remove(record.Key);
            record.DeletionTask?.Cancel();
        }
    }

    public void RemoveExpiredRecords()
    {
        foreach (var record in Records.Values)
        {
            if (record.IsExpired())
            {
                Remove(record);
            }
        }
    }

    public class CacheRecord 
    {
        public string Key { get; set; }
        public T Value { get; set; }
        public TimeSpan Lifetime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUsedAt { get; set; }

        internal EntityCache<T> Cache { get; }
        internal DelayedTask? DeletionTask { get; set; }

        public CacheRecord(EntityCache<T> cache, string key, T value, TimeSpan lifetime)
        {
            Key = key;
            Value = value;
            Lifetime = lifetime;
            CreatedAt = TimeProvider.UtcNow();
            LastUsedAt = TimeProvider.UtcNow();
            Cache = cache;
            
            if(cache.InvalidationStrategy != InvalidationStrategyType.None)
            {
                DeletionTask = new RecordDeletionTask(this, lifetime);
            }
        }

        public bool IsExpired()
        {
            switch (Cache.InvalidationStrategy)
            {
                case InvalidationStrategyType.None:
                    return false;

                case EntityCache<T>.InvalidationStrategyType.LastUsageLifetime:
                    return IsExpiredByLastUsage();

                case EntityCache<T>.InvalidationStrategyType.CreationLifetime:
                    return IsExpiredByCreationDate();

                default:
                    throw new InvalidOperationException();
            }
        }

        public void SetValue(T value)
        {
            Value = value;
        }

        public void SetLifetime(TimeSpan lifetime)
        {
            Lifetime = lifetime;
        }

        public void UpdateLastUsedAt()
        {
            LastUsedAt = TimeProvider.UtcNow();
        }

        public void UpdateDeletionTask()
        {
            switch (Cache.InvalidationStrategy)
            {
                case EntityCache<T>.InvalidationStrategyType.None:
                    return;

                case EntityCache<T>.InvalidationStrategyType.LastUsageLifetime:
                    DeletionTask?.Cancel();
                    DeletionTask = new RecordDeletionTask(this, Lifetime);
                    return;

                case EntityCache<T>.InvalidationStrategyType.CreationLifetime:
                    return;

                default:
                    throw new InvalidOperationException();
            }
        }

        private bool IsExpiredByLastUsage()
        {
            return LastUsedAt + Lifetime < TimeProvider.UtcNow();
        }

        private bool IsExpiredByCreationDate()
        {
            return CreatedAt + Lifetime < TimeProvider.UtcNow();
        }
    }

    internal class RecordDeletionTask : DelayedTask
    {
        private EntityCache<T> Cache => Record.Cache;
        private CacheRecord Record { get; }

        public RecordDeletionTask(CacheRecord record, TimeSpan delay) : base(delay)
        {
            Record = record;
        }

        protected override Task OnExecuteAsync(CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                Cache.Remove(Record.Key);
            }

            return Task.CompletedTask;
        }
    }
}