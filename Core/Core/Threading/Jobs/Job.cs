namespace ModularSystem.Core.Threading;

/// <summary>
/// Represents an abstract base job that can be queued for execution.
/// </summary>
public abstract class Job : IDisposable
{
    /// <summary>
    /// Gets or sets the number of times the job has attempted execution.
    /// </summary>
    public int ExecutionAttempts { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of execution attempts before the job is disposed.
    /// Default value is 5.
    /// </summary>
    public int MaxExecutionAttempts { get; set; } = 1;

    /// <summary>
    /// Occurs when the job has completed its execution cycle, either by successfully completing its task 
    /// or after reaching the maximum number of execution attempts.
    /// </summary>
    /// <remarks>
    /// This event is an essential part of the job lifecycle. It is triggered after the job's execution logic 
    /// is done, signifying the end of its activity. Subscribers to this event can perform cleanup, logging, 
    /// or other finalization tasks relevant to the job's completion.
    ///
    /// It's important to note that this event is invoked regardless of the job's success or failure, 
    /// making it a reliable point for implementing post-execution logic. The event is raised after 
    /// the <see cref="OnExitAsync"/> method has been called, ensuring that any asynchronous cleanup 
    /// or finalization logic has been completed.
    ///
    /// Handlers attached to this event should be prepared to execute in a thread-safe manner, as jobs 
    /// may complete on different threads depending on how they are scheduled and executed.
    /// </remarks>
    public event Action<Job> Exit;

    /// <summary>
    /// Provides cancellation capabilities for the asynchronous operations.
    /// </summary>
    private CancellationTokenSource CancellationTokenSource { get; } = new();

    /// <summary>
    /// Gets a value indicating whether the current attempt is the last execution attempt.
    /// </summary>
    protected bool IsLastExecutionAttempt => ExecutionAttempts + 1 == MaxExecutionAttempts;

    /// <summary>
    /// Initializes a new instance of the <see cref="Job"/> class with a default action for the Exit event.
    /// </summary>
    /// <remarks>
    /// This constructor sets up the default state of the Job, ensuring the Exit event has an initial,
    /// empty action to avoid null reference issues. <br/>
    /// Subscribers can later attach their specific actions
    /// to this event for handling job completion tasks.
    /// </remarks>
    public Job()
    {
        Exit = new Action<Job>(x => { });
    }

    /// <summary>
    /// Releases the unmanaged resources used by the Job and optionally releases the managed resources.
    /// </summary>
    public virtual void Dispose()
    {
        CancellationTokenSource.Cancel();
        CancellationTokenSource.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Queues the job to the ThreadPool for execution.
    /// </summary>
    public Task QueueToThreadPool()
    {
        return Task.Run(InternalExecuteAsync);
    }

    private async Task InternalExecuteAsync()
    {
        while (true)
        {
            try
            {
                await OnExecuteAsync(CancellationTokenSource.Token);
                break;
            }
            catch (Exception e)
            {
                await OnExceptionAsync(e, CancellationTokenSource.Token);
            }

            ExecutionAttempts++;

            if (ExecutionAttempts >= MaxExecutionAttempts)
            {
                break;
            }

            await Task.Delay(TimeOut(), CancellationTokenSource.Token);
        }
    
        try 
        { 
            await OnExitAsync(CancellationTokenSource.Token); 
        } 
        catch { }

        try
        {
            Exit.Invoke(this);
        }
        catch { }
        
        Dispose();
    }

    /// <summary>
    /// Represents the core logic of the job to be executed.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task representing the execution of the job.</returns>
    protected abstract Task OnExecuteAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Provides a mechanism to handle exceptions that might occur during execution.
    /// </summary>
    /// <param name="e">The exception encountered.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task representing the exception handling process.</returns>
    protected virtual Task OnExceptionAsync(Exception e, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Represents actions to be taken when the job finishes execution, either successfully or after max attempts.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous exit operation.</returns>
    protected virtual Task OnExitAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines the delay between execution attempts.
    /// </summary>
    /// <returns>The duration of the delay.</returns>
    protected virtual TimeSpan TimeOut()
    {
        return TimeSpan.FromMilliseconds(250);
    }
}

/// <summary>
/// Represents a job that can execute a provided lambda function.
/// </summary>
public class LambdaJob : Job
{
    private Func<CancellationToken, Task> lambda;

    /// <summary>
    /// Initializes a new instance of the <see cref="LambdaJob"/> class.
    /// </summary>
    /// <param name="lambda">The function to execute when the job runs.</param>
    public LambdaJob(Func<CancellationToken, Task> lambda)
    {
        MaxExecutionAttempts = 1;
        this.lambda = lambda;
    }

    ///<inheritdoc/>
    protected override Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        return lambda.Invoke(cancellationToken);
    }
}