using ModularSystem.Core.Logging;

namespace ModularSystem.Core.Threading;

/// <summary>
/// A single threaded scope of execution for a set of functions. This class spawns a new worker thread. 
/// The worker sequentially executes the provided functions.
/// </summary>
public class SingleThreadRoutine : IDisposable
{
    public bool LogExceptions { get; set; } = true;

    private int ExecutionIntervalMilliseconds { get; set; }
    private bool IsRunning { get; set; }
    private List<ScheduledCallback> Routines { get; set; }
    private Thread Worker { get; set; }

    public SingleThreadRoutine(TimeSpan executionInterval, IEnumerable<ScheduledCallback>? routines = null)
    {
        ExecutionIntervalMilliseconds = (int)executionInterval.TotalMilliseconds;
        IsRunning = false;
        Routines = routines?.ToList() ?? new();
        Worker = new Thread(ThreadLoop);

        Worker.Name = "Routine Worker";
        Worker.IsBackground = true;
    }

    public void Dispose()
    {
        IsRunning = false;
    }

    public SingleThreadRoutine SetExecutionInterval(int milliseconds)
    {
        ExecutionIntervalMilliseconds = milliseconds;
        return this;
    }

    public SingleThreadRoutine SetExecutionInterval(TimeSpan timeSpan)
    {
        ExecutionIntervalMilliseconds = (int)timeSpan.TotalMilliseconds;
        return this;
    }

    public SingleThreadRoutine AddSubRoutine(ScheduledCallback subRoutine)
    {
        Routines.Add(subRoutine);
        return this;
    }

    /// <summary>
    /// Starts the execution loop in a single worker thread.
    /// </summary>
    /// <returns></returns>
    public SingleThreadRoutine Start()
    {
        IsRunning = true;
        Worker.Start();
        return this;
    }

    void OnException(Exception e)
    {
        if (LogExceptions)
        {
            Console.WriteLine(e);
        }
    }

    void ThreadLoop()
    {
        while (IsRunning)
        {
            try
            {
                foreach (var routine in Routines)
                {
                    try
                    {
                        routine.Execute();
                    }
                    catch (Exception e)
                    {
                        routine.OnException(e);
                    }
                }
            }
            catch (Exception e)
            {
                OnException(e);
            }
            finally
            {
                Thread.Sleep(ExecutionIntervalMilliseconds);
            }
        }
    }
}

public abstract class ScheduledCallback
{
    /// <summary>
    /// Invoked by the routine's worker thread.
    /// </summary>
    public abstract void Execute();


    public virtual void OnException(Exception e)
    {

    }
}