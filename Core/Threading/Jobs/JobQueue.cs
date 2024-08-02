using Aidan.Core.Extensions;

namespace Aidan.Core.Threading;

/// <summary>
/// Provides static methods to queue and manage jobs for execution by the thread pool. <br/>
/// This class allows for parallel execution of multiple jobs and handles their lifecycle.
/// </summary>
public static class JobQueue
{
    private static List<Job> RunningJobs = new(50);
    private static TaskCompletionSource ExitEvent { get; set; } = new();

    /// <summary>
    /// Queues a specified job for execution by the thread pool. <br/>
    /// Allows multiple jobs to be executed in parallel depending on available threads.
    /// </summary>
    /// <param name="job">The job to be queued for execution.</param>
    /// <remarks>
    /// The method adds the job to the running jobs list and queues it for execution. <br/>
    /// When the job exits, it is automatically removed from the running jobs list.
    /// </remarks>
    public static void Enqueue(Job job)
    {
        job.Exit += (job) => StopTracking(job);
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
    /// Queues a specified <see cref="Task"/> for execution as a job by the thread pool. <br/>
    /// The task is wrapped in a <see cref="LambdaJob"/> and executed in parallel depending on available threads.
    /// </summary>
    /// <param name="work">The <see cref="Task"/> to be executed as a job.</param>
    /// <returns>A <see cref="Job"/> instance representing the queued task.</returns>
    /// <remarks>
    /// This method allows for the convenient queuing of a <see cref="Task"/> without the need to explicitly create a <see cref="Job"/> instance. <br/>
    /// The returned <see cref="Job"/> provides mechanisms to monitor and control the execution of the queued task.
    /// </remarks>
    public static Job Enqueue(Task work)
    {
        var job = new LambdaJob(x => work);
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
    /// Waits asynchronously for all queued jobs to complete.
    /// </summary>
    /// <returns>A task that represents the asynchronous wait operation for the completion of all jobs.</returns>
    /// <remarks>
    /// The method returns a task that completes when all currently queued jobs have finished executing. <br/>
    /// If there are no active jobs at the time of calling, the returned task is already completed.
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

    private static void StopTracking(Job job)
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
