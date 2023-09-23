namespace ModularSystem.Core.Threading;

/// <summary>
/// Represents a routine that repetitively performs an asynchronous task with a specified delay between iterations.
/// </summary>
public abstract class TaskRoutine : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the routine is currently running.
    /// </summary>
    public bool IsRunning { get; private set; }

    private TimeSpan Delay { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskRoutine"/> class with a defined delay interval.
    /// </summary>
    /// <param name="delay">The interval between task executions.</param>
    public TaskRoutine(TimeSpan delay)
    {
        Delay = delay;
    }

    /// <summary>
    /// Stops the execution of the task routine and releases resources.
    /// </summary>
    public void Dispose()
    {
        Stop();
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
        lock(this)
        {
            if (IsRunning)
            {
                return;
            }

            IsRunning = true;
            Task.Run(InternalExecuteAsync);
        }
    }

    /// <summary>
    /// Halts the execution of the task routine.
    /// </summary>
    public void Stop()
    {
        lock (this)
        {
            IsRunning = false;
        }
    }

    private async Task InternalExecuteAsync()
    {
        while (IsRunning)
        {
            try
            {
                await IterationWork();
                await Task.Delay(Delay);
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
            await OnExecuteAsync();
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
    protected abstract Task OnExecuteAsync();

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
    protected Func<Task> Callback { get; set; }

    /// <summary>
    /// Constructs a new <see cref="LambdaTaskRoutine"/> with a specified delay and callback function.
    /// </summary>
    /// <param name="delay">The delay between executions of the callback.</param>
    /// <param name="callback">The callback function to be executed.</param>
    public LambdaTaskRoutine(TimeSpan delay, Func<Task> callback) : base(delay)
    {
        Callback = callback;
    }

    /// <summary>
    /// Executes the provided callback function.
    /// </summary>
    protected override Task OnExecuteAsync()
    {
        return Callback.Invoke();
    }
}
