namespace Aidan.Core.Threading;

/// <summary>
/// Represents an abstract base job that can be queued for execution. <br/>
/// This base class provides the common framework for job execution, including retry logic, <br/>
/// exception handling, and execution tracking.
/// </summary>
public abstract class Job : IDisposable
{
    /// <summary>
    /// Gets or sets the total number of times the job has attempted execution. <br/>
    /// This count includes both successful and unsuccessful attempts. It is incremented each time the job begins
    /// an execution attempt.
    /// </summary>
    /// <value>
    /// The total number of execution attempts. This number is incremented after each execution attempt, <br/>
    /// regardless of whether the attempt was successful or encountered an exception.
    /// </value>
    /// <remarks>
    /// This property is crucial for understanding the job's execution history and for implementing <br/>
    /// logic that depends on the number of attempts, such as exponential backoff or other retry strategies. <br/>
    /// The <see cref="OnExceptionAsync(Exception, CancellationToken)"/> method increments this counter <br/>
    /// each time it is called, reflecting the job's resilience in the face of errors. <br/>
    ///
    /// Note: This counter is reset to zero when the job is instantiated and is only meant to track <br/>
    /// attempts for the current execution cycle of the job. It does not persist across job restarts
    /// or system reboots.
    /// </remarks>
    public int TotalExecutionAttempts { get; set; }

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
    protected bool IsLastExecutionAttempt { get; private set; }

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
        var executionAttempts = 0;

        while (true)
        {
            IsLastExecutionAttempt = executionAttempts + 1 >= MaxExecutionAttempts;

            try
            {
                await BeforeExecuteAsync(CancellationTokenSource.Token);
                await OnExecuteAsync(CancellationTokenSource.Token);
                await AfterExecuteAsync(CancellationTokenSource.Token);
                break;
            }
            catch (Exception e)
            {
                await OnExceptionAsync(e, CancellationTokenSource.Token);
            }

            executionAttempts++;
            TotalExecutionAttempts++;

            if (executionAttempts >= MaxExecutionAttempts)
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
    /// Executes custom logic before the main job execution begins.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. This can be used to cancel the operation before it starts.</param>
    /// <returns>A task that represents the asynchronous pre-execution operation. The default implementation returns a completed task.</returns>
    /// <remarks>
    /// Override this method to implement any initialization or setup procedures that need to occur before the job's primary logic executes.
    /// This could include setting up logging, validating job state, or preparing resources required for execution.
    /// It's important to check the <paramref name="cancellationToken"/> regularly in long-running or potentially blocking operations
    /// to ensure responsive cancellation if requested.
    /// </remarks>
    protected virtual Task BeforeExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Executes custom logic after the main job execution completes successfully.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. This can be used to cancel the post-execution operations if necessary.</param>
    /// <returns>A task that represents the asynchronous post-execution operation. The default implementation returns a completed task.</returns>
    /// <remarks>
    /// Override this method to implement any teardown or cleanup procedures that should occur only after the job's primary logic has executed successfully.
    /// This method is invoked if the job completes without throwing any exceptions, making it an ideal place for operations that should only be performed
    /// after a successful execution, such as committing transactions, updating job status in a database, or releasing allocated resources that are no longer needed.
    ///
    /// It's important to note that this method will not be called if an exception is thrown during the job execution, which is handled by the <see cref="OnExceptionAsync(Exception, CancellationToken)"/> method.
    /// Therefore, any cleanup or finalization logic that must run regardless of job success or failure should be placed within <see cref="OnExitAsync(CancellationToken)"/> or the exception handling routine.
    ///
    /// As with <see cref="BeforeExecuteAsync"/>, ensure to regularly check the <paramref name="cancellationToken"/> during potentially long-running operations
    /// or while awaiting other asynchronous operations, to maintain responsiveness to cancellation requests.
    /// </remarks>
    protected virtual Task AfterExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

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