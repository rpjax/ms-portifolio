using ModularSystem.Core.Threading;

namespace ModularSystem.Core.Caching;

/// <summary>
/// Do not forget to call the dispose method before the object goes out of scope, or on references are to be found.
/// </summary>
/// <typeparam name="T"></typeparam>
public class EntityCache<T> : IDisposable where T : class, IQueryableModel
{
    public static readonly TimeSpan DefaultRecordLifetime = TimeSpan.FromMinutes(60);
    public static readonly TimeSpan DefaultDeletionRoutineDelay = TimeSpan.FromMinutes(1);

    private Dictionary<string, CacheRecord<T>> Records { get; set; }

    private TimeSpan RecordLifetime { get; set; }
    private TimeSpan DeletionRoutineDelay { get; set; }
    private DateTime DeletionRoutineLastRanAt { get; set; }
    private TaskRoutine DeletionRoutine { get; }

    public EntityCache()
    {
        Records = new ();
        RecordLifetime = DefaultRecordLifetime;
        DeletionRoutineDelay = DefaultDeletionRoutineDelay;
        DeletionRoutineLastRanAt = TimeProvider.UtcNow();
        DeletionRoutine = new DeletionTaskRoutine(this, DefaultDeletionRoutineDelay);
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

    public T? TryGet(string key)
    {
        return TryGetRecord(key)?.Value;
    }

    public void Set(T data, TimeSpan? lifetime = null)
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

        lock (Records)
        {
            Records[id] = record;
        }

        if(!DeletionRoutine.IsRunning)
        {
            DeletionRoutine.Start();
        }
    }

    public void Delete(string key)
    {
        lock (Records)
        {
            Records.Remove(key);

            if (Records.IsEmpty())
            {
                DeletionRoutine.Stop();
            }
        }     
    }

    public void Clear()
    {
        lock (Records)
        {
            Records.Clear();
        }
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

    internal class CacheRecord<T> where T : class, IQueryableModel
    {
        public T? Value { get; set; }
        public TimeSpan Lifetime { get; set; }
        public DateTime LastUsedAt { get; set; }
        public string? RecordId => Value?.GetId();

        public CacheRecord(T? value, TimeSpan lifetime)
        {
            Value = value;
            Lifetime = lifetime;
        }

        public bool IsExpired()
        {
            return LastUsedAt + Lifetime < TimeProvider.Now();
        }

        public void SetValue(T? value)
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
        private EntityCache<T> Cache { get; }

        public DeletionTaskRoutine(EntityCache<T> cache, TimeSpan delay) : base(delay)
        {
            Cache = cache;
        }

        protected override Task OnExecuteAsync()
        {
            if (Cache.DeletionRoutineLastRanAt + Cache.DeletionRoutineDelay > TimeProvider.UtcNow())
            {
                return Task.CompletedTask;
            }

            foreach (var item in Cache.Records)
            {
                var record = item.Value;

                if (record.IsExpired() && record.Value == null)
                {
                    Cache.Records.Remove(item.Key);
                }
            }

            Cache.DeletionRoutineLastRanAt = TimeProvider.UtcNow();

            return Task.CompletedTask;
        }
    }
}