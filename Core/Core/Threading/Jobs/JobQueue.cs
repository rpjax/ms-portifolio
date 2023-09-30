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
    public int MaxExecutionAttempts { get; set; } = 5;

    /// <summary>
    /// Provides cancellation capabilities for the asynchronous operations.
    /// </summary>
    private CancellationTokenSource CancellationTokenSource { get; } = new();

    /// <summary>
    /// Gets a value indicating whether the current attempt is the last execution attempt.
    /// </summary>
    protected bool IsLastExecutionAttempt => ExecutionAttempts + 1 == MaxExecutionAttempts;

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
                await ExecuteAsync(CancellationTokenSource.Token);
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

        await OnExitAsync(CancellationTokenSource.Token);

        Dispose();
    }

    /// <summary>
    /// Represents the core logic of the job to be executed.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task representing the execution of the job.</returns>
    protected abstract Task ExecuteAsync(CancellationToken cancellationToken);

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
        this.lambda = lambda;
    }

    ///<inheritdoc/>
    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        return lambda.Invoke(cancellationToken);
    }
}

/// <summary>
/// Provides static methods to queue and unqueue jobs.
/// </summary>
public static class JobQueue
{
    /// <summary>
    /// Queues the job to be executed by the thread pool. <br/>
    /// Multiple jobs might be executed in parallel depending on available threads.
    /// </summary>
    /// <param name="job">The job to be queued.</param>
    public static void Enqueue(Job job)
    {
        job.QueueToThreadPool();
    }

    /// <summary>
    /// Queues the lambda function as a job to be executed by the thread pool. <br/>
    /// Multiple jobs might be executed in parallel depending on available threads.
    /// </summary>
    /// <param name="work">The lambda function to execute as a job.</param>
    /// <returns>The job representing the lambda function.</returns>
    public static Job Enqueue(Func<CancellationToken, Task> work)
    {
        var job = new LambdaJob(work);
        Enqueue(job);
        return job;
    }

    /// <summary>
    /// Queues the lambda function as a job to be executed by the thread pool. <br/>
    /// Multiple jobs might be executed in parallel depending on available threads.
    /// </summary>
    /// <param name="work">The lambda function to execute as a job.</param>
    /// <returns>The job representing the lambda function.</returns>
    public static Job Enqueue(Action work)
    {
        var job = new LambdaJob(async (cancellationToken) =>
        {
            work.Invoke();
        });
        Enqueue(job);
        return job;
    }

    /// <summary>
    /// Removes the specified job from the queue.
    /// </summary>
    /// <param name="job">The job to be removed from the queue.</param>
    public static void Dequeue(Job job)
    {
        throw new NotImplementedException();
    }
}
