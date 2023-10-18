namespace ModularSystem.Core.Threading;

/// <summary>
/// Represents a task that can be scheduled to run after a specified delay.
/// </summary>
/// <remarks>
/// This class provides a mechanism to execute a task asynchronously after a given delay. <br/>
/// It also provides a way to cancel the scheduled task before it's executed.
/// </remarks>
public abstract class DelayedTask
{
    /// <summary>
    /// Gets a value indicating whether the task has started execution.
    /// </summary>
    public bool IsStarted { get; private set; }

    /// <summary>
    /// Gets the delay after which the task should be executed.
    /// </summary>
    private TimeSpan Delay { get; }

    /// <summary>
    /// Gets or sets the cancellation token source for the delayed task.
    /// </summary>
    private CancellationTokenSource? CancellationTokenSource { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DelayedTask"/> class.
    /// </summary>
    /// <param name="delay">The delay after which the task should be executed.</param>
    public DelayedTask(TimeSpan delay)
    {
        IsStarted = false;
        Delay = delay;
        CancellationTokenSource = null;
    }

    /// <summary>
    /// Starts the delayed task.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the task has already been executed.</exception>
    public void Start()
    {
        lock (this)
        {
            if (IsStarted)
            {
                throw new InvalidOperationException("The delayed task has already been executed.");
            }

            IsStarted = true;    
        }

        CancellationTokenSource = new();
        Task.Run(() => ExecuteAsync(CancellationTokenSource.Token));
    }

    /// <summary>
    /// Cancels the scheduled task if it has not yet been executed.
    /// </summary>
    public void Cancel()
    {
        CancellationTokenSource?.Cancel();
    }

    private async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(Delay, cancellationToken);
            
            if(!cancellationToken.IsCancellationRequested)
            {
                await OnExecuteAsync(cancellationToken);
            }
        }
        catch (Exception e)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                await OnExceptionAsync(e, cancellationToken);
            }
        }
        finally
        {
            InternalOnExit();
            OnExit();
        }
    }

    private void InternalOnExit()
    {
        CancellationTokenSource?.Dispose();
    }

    /// <summary>
    /// Invoked when the task is executed. Override to provide task-specific logic.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token that can be used to cancel the task.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Invoked when an exception occurs during the execution of the task. Override to provide exception-handling logic.
    /// </summary>
    /// <param name="e">The exception that occurred.</param>
    /// <param name="cancellationToken">The cancellation token that can be used to cancel the task.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task OnExceptionAsync(Exception e, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Invoked when the task has completed its execution, whether successfully, with an exception, or was cancelled. 
    /// Override to provide logic that should always run after the task's completion.
    /// </summary>
    protected virtual void OnExit()
    {

    }
}