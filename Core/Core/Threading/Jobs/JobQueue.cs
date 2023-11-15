namespace ModularSystem.Core.Threading;

/// <summary>
/// Provides static methods to queue and unqueue jobs.
/// </summary>
public static class JobQueue
{
    private static List<Job> RunningJobs = new(50);
    private static TaskCompletionSource ExitEvent { get; set; } = new();

    /// <summary>
    /// Queues the job to be executed by the thread pool. <br/>
    /// Multiple jobs might be executed in parallel depending on available threads.
    /// </summary>
    /// <param name="job">The job to be queued.</param>
    public static void Enqueue(Job job)
    {
        job.Exit += (job) => Untrack(job);
        Track(job);
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

    /// <summary>
    /// Waits asynchronously for all queued jobs to complete.
    /// </summary>
    /// <returns>A task that represents the asynchronous wait operation.</returns>
    /// <remarks>
    /// The method returns a task that completes when all currently queued jobs have finished executing.
    /// If there are no active jobs, the returned task is already completed.
    /// </remarks>
    public static Task WaitAllJobsAsync()
    {
        return ExitEvent.Task;
    }

    private static void Track(Job job)
    {
        lock (RunningJobs)
        {
            RunningJobs.Add(job);

            if (ExitEvent.Task.IsCompleted)
            {
                ExitEvent = new();
            }
        }
    }

    private static void Untrack(Job job)
    {
        lock (RunningJobs)
        {
            RunningJobs.Remove(job);

            if (RunningJobs.IsEmpty())
            {
                ExitEvent.SetResult();
            }
        }
    }

}
