using ModularSystem.Core.Threading;
using System.Collections.Concurrent;

namespace ModularSystem.Core.Caching;

public class EntityCache<T> : IDisposable where T : class
{
    public enum InvalidationStrategyType
    {
        LastUsageLifetime,
        CreationLifetime
    }

    public static readonly TimeSpan DefaultRecordLifetime = TimeSpan.FromMinutes(5);
    public static readonly TimeSpan DefaultDeletionRoutineDelay = TimeSpan.FromMinutes(1);

    public long Size { get => Records.Count; }
    public long SizeLimit { get; set; }
    public TimeSpan RecordLifetime { get; set; }
    public TimeSpan DeletionRoutineDelay { get; set; }
    public InvalidationStrategyType InvalidationStrategy { get; set; }

    private ConcurrentDictionary<string, CacheRecord<T>> Records { get; set; }
    private DateTime DeletionRoutineLastRanAt { get; set; }
    private TaskRoutine DeletionRoutine { get; }

    public EntityCache(long sizeLimit)
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

    public EntityCache<T> SetRecordLifetime(TimeSpan recordLifetime)
    {
        RecordLifetime = recordLifetime;
        return this;
    }

    public EntityCache<T> SetDeletionRoutineDelay(TimeSpan deletionRoutineDelay)
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

    public bool TrySet(string key, T data, TimeSpan? lifetime = null)
    {
        var record = TryGetRecord(key);

        if (record != null)
        {
            record.SetValue(data);
            record.UpdateLastUsedAt();
        }
        else
        {
            record = new CacheRecord<T>(data, lifetime ?? RecordLifetime);
        }

        if (IsFull())
        {
            return false;
        }

        Records[key] = record;

        return true;
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

    internal class CacheRecord<TValue> 
    {
        public TValue? Value { get; set; }
        public TimeSpan Lifetime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUsedAt { get; set; }

        public CacheRecord(TValue? value, TimeSpan lifetime)
        {
            Value = value;
            Lifetime = lifetime;
            CreatedAt = TimeProvider.UtcNow();
            LastUsedAt = TimeProvider.UtcNow();
        }

        public bool IsExpiredByLastUsage()
        {
            return LastUsedAt + Lifetime < TimeProvider.UtcNow();
        }

        public bool IsExpiredByCreationDate()
        {
            return CreatedAt + Lifetime < TimeProvider.UtcNow();
        }

        public void SetValue(TValue? value)
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
    }

    internal class DeletionTaskRoutine : TaskRoutine
    {
        private EntityCache<T> Cache { get; }

        public DeletionTaskRoutine(EntityCache<T> cache, TimeSpan delay) : base(delay)
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

                if(Cache.InvalidationStrategy == InvalidationStrategyType.LastUsageLifetime)
                {
                    if (record.IsExpiredByLastUsage())
                    {
                        Cache.Remove(item.Key);
                    }
                }
                if (Cache.InvalidationStrategy == InvalidationStrategyType.CreationLifetime)
                {
                    if (record.IsExpiredByCreationDate())
                    {
                        Cache.Remove(item.Key);
                    }
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