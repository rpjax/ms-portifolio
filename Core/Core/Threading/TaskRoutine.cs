namespace ModularSystem.Core.Threading;

/// <summary>
/// Represents a routine that repetitively performs an asynchronous task with a specified delay between iterations.
/// </summary>
public abstract class TaskRoutine : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the routine can start.
    /// </summary>
    public bool CanStart => !IsRunning && ExitEvent.IsSet;

    private bool IsRunning { get; set; }
    private TimeSpan Delay { get; set; }
    private ManualResetEventSlim ExitEvent { get; set; }
    private CancellationTokenSource? CancellationTokenSource { get; set; }

    private readonly object LockObject = new object();

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskRoutine"/> class with a defined delay interval.
    /// </summary>
    /// <param name="delay">The interval between task executions.</param>
    public TaskRoutine(TimeSpan delay)
    {
        Delay = delay;
        ExitEvent = new(true);
    }

    /// <summary>
    /// Stops the execution of the task routine and releases resources.
    /// </summary>
    public void Dispose()
    {
        Stop();
        ExitEvent.Dispose();
        CancellationTokenSource?.Dispose();
    }

    /// <summary>
    /// Configures the delay interval between task executions.
    /// </summary>
    /// <param name="timeSpan">The desired delay interval.</param>
    /// <returns>The current instance, enabling method chaining.</returns>
    public TaskRoutine SetDelay(TimeSpan timeSpan)
    {
        Delay = timeSpan;
        return this;
    }

    /// <summary>
    /// Initiates the task routine.
    /// </summary>
    public void Start()
    {   
        lock(LockObject)
        {            
            if(!CanStart)
            {
                throw new InvalidOperationException("Cannot start the task routine because there is an instance running or that has not exited yet.");
            }

            IsRunning = true;
            ExitEvent.Reset();
            CancellationTokenSource = new();

            Task.Run(InternalExecuteAsync);
        }
    }

    /// <summary>
    /// Halts the execution of the task routine.
    /// </summary>
    public void Stop()
    {
        lock(LockObject)
        {
            if(!IsRunning)
            {
                return;
            }

            IsRunning = false;
        }

        CancellationTokenSource?.Cancel();
        ExitEvent.Wait();
    }

    /// <summary>
    /// Awaits the exit of the task routine.
    /// </summary>
    public void WaitExit()
    {
        if(ExitEvent.IsSet)
        {
            return;
        }

        ExitEvent.Wait();
    }

    private async Task InternalExecuteAsync()
    {
        while (IsRunning)
        {
            try
            {
                await IterationWork();
                await Task.Delay(Delay, CancellationTokenSource!.Token);
            }
            catch (Exception e)
            {
                await OnInternalExceptionAsync(e);
            }
        }

        OnExit();
    }

    private async Task IterationWork()
    {
        try
        {
            await OnExecuteAsync(CancellationTokenSource!.Token);
        }
        catch (Exception e)
        {
            await OnExceptionAsync(e);
        }
    }

    /// <summary>
    /// Provides internal handling for exceptions caught during the main execution loop.
    /// </summary>
    /// <param name="e">The exception that occurred.</param>
    private Task OnInternalExceptionAsync(Exception e)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Main execution logic to be implemented by derived classes.
    /// </summary>
    protected abstract Task OnExecuteAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Exception handling logic for errors that occur during <see cref="OnExecuteAsync"/>.
    /// </summary>
    /// <param name="e">The exception encountered during task execution.</param>
    protected virtual Task OnExceptionAsync(Exception e)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Logic to be executed after the TaskRoutine stops running, either when it is manually stopped or completes its execution.
    /// Derived classes can override this method to provide custom behavior when the TaskRoutine stops.
    /// </summary>
    protected virtual void OnExit()
    {
        CancellationTokenSource?.Dispose();
        CancellationTokenSource = null;

        ExitEvent.Set();
    }

}

/// <summary>
/// A concrete implementation of <see cref="TaskRoutine"/> which performs a given callback function.
/// </summary>
public class LambdaTaskRoutine : TaskRoutine
{
    /// <summary>
    /// The callback function to be executed.
    /// </summary>
    protected Func<CancellationToken, Task> Callback { get; set; }

    /// <summary>
    /// Constructs a new <see cref="LambdaTaskRoutine"/> with a specified delay and callback function.
    /// </summary>
    /// <param name="delay">The delay between executions of the callback.</param>
    /// <param name="callback">The callback function to be executed.</param>
    public LambdaTaskRoutine(TimeSpan delay, Func<CancellationToken, Task> callback) : base(delay)
    {
        Callback = callback;
    }

    /// <summary>
    /// Executes the provided callback function.
    /// </summary>
    protected override Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        return Callback.Invoke(cancellationToken);
    }
}
