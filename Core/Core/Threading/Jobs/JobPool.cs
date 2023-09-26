using ModularSystem.Core.Logging;

namespace ModularSystem.Core.Threading;

public class JobPoolInitializer : Initializer
{
    public override Task InternalInitAsync(Options options)
    {
        JobPool.Init();

        if (options.EnableInitializationLogs)
        {
            ConsoleLogger.Info("Job pool initialized.");
        }

        return Task.CompletedTask;
    }
}

public interface IJob : IDisposable
{
    int ExecutionAttempts { get; set; }

    bool Execute();
}

public abstract class Job : IJob
{
    public int ExecutionAttempts { get; set; }

    public virtual bool Execute()
    {
        try
        {
            Work();
            return true;
        }
        catch (Exception e)
        {
            OnException(e);
            return false;
        }
    }

    public void Dispose() { }

    protected abstract void Work();

    protected virtual void OnException(Exception e)
    {
        //
    }
}

public class LambdaJob : Job
{
    Action lambda;

    public LambdaJob(Action lambda)
    {
        this.lambda = lambda;
    }

    protected override void Work()
    {
        lambda();
    }
}

public static class JobPool
{
    public static int MaxExecutionAttempts { get; set; } = 5;
    public static TimeSpan IdleSleepTime { get; set; } = TimeSpan.FromMilliseconds(1);

    static bool IsInit { get; set; }
    static List<IJob> Jobs { get; }
    static Thread MainWorker { get; }

    static JobPool()
    {
        Jobs = new List<IJob>(100);
        MainWorker = new Thread(ThreadLoop);
        MainWorker.IsBackground = true;
    }

    /// <summary>
    /// Starts a thread that executes jobs. 
    /// </summary>
    public static void Init()
    {
        if (IsInit)
        {
            ConsoleLogger.Warn("JobPool has already been initialized.");
            return;
        }

        IsInit = true;
        MainWorker.Name = "Job Queue";
        MainWorker.Start();
    }

    /// <summary>
    /// Queues the job to be executed by a separeated thread. Jobs are executed sequentially, one at a time, as fast as possible.
    /// </summary>
    /// <param name="job"></param>
    public static void Queue(IJob job)
    {
        lock (Jobs)
        {
            Jobs.Add(job);
        }
    }

    public static void Queue(Action lambda)
    {
        Queue(new LambdaJob(lambda));
    }

    public static void Unqueue(IJob job)
    {
        lock (Jobs)
        {
            Jobs.Remove(job);
        }
    }

    static IJob? PopFirstJobInQueue()
    {
        lock (Jobs)
        {
            if (Jobs.IsEmpty())
            {
                return null;
            }

            var job = Jobs[0];
            Jobs.Remove(job);
            return job;
        }
    }

    static void Sleep()
    {
        int milliseconds = (int)IdleSleepTime.TotalMilliseconds;
        Thread.Sleep(milliseconds);
    }

    static void ThreadLoop()
    {
        while (true)
        {
            try
            {
                var job = PopFirstJobInQueue();

                if (job == null)
                {
                    Sleep();
                    continue;
                }

                job.ExecutionAttempts++;

                if (job.ExecutionAttempts > MaxExecutionAttempts)
                {
                    job.Dispose();
                    continue;
                }

                if (job.Execute())
                {
                    job.Dispose();
                    continue;
                }
                else
                {
                    Queue(job);
                }
            }
            catch (Exception e)
            {
                ConsoleLogger.Error(e.Message);
            }
        }
    }

    static void JobRunnerFunction()
    {
        try
        {
            var job = PopFirstJobInQueue();

            if (job == null)
            {
                Sleep();
                return;
            }

            job.ExecutionAttempts++;

            if (job.ExecutionAttempts > MaxExecutionAttempts)
            {
                job.Dispose();
                return;
            }

            if (job.Execute())
            {
                job.Dispose();
                return;
            }
            else
            {
                Queue(job);
            }
        }
        catch (Exception e)
        {
            ConsoleLogger.Error(e.Message);
        }
    }
}