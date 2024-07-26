using ModularSystem.Core.Exceptions;
using ModularSystem.Core.Extensions;

namespace ModularSystem.Core.Threading;

/// <summary>
/// Provides an asynchronous mutual exclusion lock mechanism.
/// </summary>
public class TaskLock : IDisposable
{
    /// <summary>
    /// Indicates whether the lock is currently held.
    /// </summary>
    private bool IsLocked { get; set; }

    /// <summary>
    /// Queue of awaiters waiting for the lock to be released.
    /// </summary>
    private Queue<TaskCompletionSource> WaiterQueue { get; }


    /// <summary>
    /// Initializes a new instance of the <see cref="TaskLock"/> class.
    /// </summary>
    public TaskLock()
    {
        IsLocked = false;
        WaiterQueue = new Queue<TaskCompletionSource>();
    }

    /// <summary>
    /// Cleans up the TaskLock resources, by releasing any pending waiters with an exception to signal
    /// that the lock object has been disposed. <br/>
    /// This prevents deadlocks when the TaskLock is disposed while tasks are still waiting for the lock.
    /// </summary>
    public void Dispose()
    {
        lock (this)
        {
            IsLocked = false;

            foreach (var waiter in WaiterQueue)
            {
                var message = "The waiter lock was disposed of before all waiters could ";
                var error = new Error(message);

                waiter.SetException(new ErrorException(error));
            }

            WaiterQueue.Clear();
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Asynchronously acquires the lock. If the lock is already held, waits until the lock is released.
    /// </summary>
    /// <returns>A task that will complete when the lock has been acquired.</returns>
    public Task AcquireAsync()
    {
        lock (this)
        {
            if (!IsLocked)
            {
                IsLocked = true;
                return Task.CompletedTask;
            }

            var waiter = CreateWaiter();
            WaiterQueue.Enqueue(waiter);
            return waiter.Task;
        }
    }

    /// <summary>
    /// Releases the lock, allowing the next waiter in the queue (if any) to acquire the lock.
    /// </summary>
    public void Release()
    {
        lock (this)
        {
            if (WaiterQueue.IsNotEmpty())
            {
                YieldToNextWaiter();
            }
            else
            {
                IsLocked = false;
            }
        }
    }

    /// <summary>
    /// Dequeues the next waiter in the queue and sets its task as completed, allowing it to acquire the lock.
    /// </summary>
    private void YieldToNextWaiter()
    {
        WaiterQueue.Dequeue().SetResult();
    }

    /// <summary>
    /// Creates a new <see cref="TaskCompletionSource"/> for a waiter.
    /// </summary>
    /// <returns>A new <see cref="TaskCompletionSource"/>.</returns>
    private TaskCompletionSource CreateWaiter()
    {
        return new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    }


    private async Task TimeoutFunction(TimeSpan timeout, CancellationToken cancellationToken)
    {
        await Task.Delay(timeout, cancellationToken);
        Release();
    }
}

public class TaskLockTimeout
{
    public Task Task => TaskCompletionSource.Task;

    private TaskCompletionSource TaskCompletionSource { get; }
    private TimeSpan Timeout { get; }
    private Action Release { get; }
    private TaskRoutine TimeoutTask { get; }

    public TaskLockTimeout(TimeSpan timeout, Action release)
    {
        TaskCompletionSource = new();
        Timeout = timeout;
        Release = release;
        TimeoutTask = new LambdaTaskRoutine(timeout, TimeoutFunction);
    }

    public void StopTimeout()
    {
        TimeoutTask.Stop();
    }

    private async Task TimeoutFunction(CancellationToken cancellationToken)
    {
        await Task.Delay(Timeout, cancellationToken);

        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        var error = new Error("The lock has timed out.");
        var exception = new ErrorException(error);

        TaskCompletionSource.SetException(exception);
        Release.Invoke();
    }
}
