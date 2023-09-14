using ModularSystem.Core.Logging;

namespace ModularSystem.Core.Threading;

/// <summary>
/// A single threaded scope of execution for a set of functions. This class spawns a new worker thread. 
/// The worker sequentially executes the provided functions.
/// </summary>
public class SingleThreadRoutine : IDisposable
{
    int executionIntervalMilliseconds;
    List<SubRoutine> routines;
    Thread thread;
    bool isRunning;

    public SingleThreadRoutine(TimeSpan executionInterval)
    {
        executionIntervalMilliseconds = (int)executionInterval.TotalMilliseconds;
        routines = new();
        thread = new Thread(ThreadLoop);
        isRunning = false;

        thread.Name = "Routine Worker";
    }

    public SingleThreadRoutine(TimeSpan executionInterval, SubRoutine routine)
    {
        executionIntervalMilliseconds = (int)executionInterval.TotalMilliseconds;
        routines = new() { routine };
        thread = new Thread(ThreadLoop);
        isRunning = false;

        thread.Name = "Routine Worker";
    }

    public SingleThreadRoutine(TimeSpan executionInterval, IEnumerable<SubRoutine> routines)
    {
        executionIntervalMilliseconds = (int)executionInterval.TotalMilliseconds;
        this.routines = routines.ToList();
        thread = new Thread(ThreadLoop);
        isRunning = false;

        thread.Name = "Routine Worker";
    }

    public bool LogExceptions { get; set; } = true;

    public void Dispose()
    {
        isRunning = false;
    }

    public SingleThreadRoutine SetExecutionInterval(int milliseconds)
    {
        executionIntervalMilliseconds = milliseconds;
        return this;
    }

    public SingleThreadRoutine SetExecutionInterval(TimeSpan timeSpan)
    {
        executionIntervalMilliseconds = (int)timeSpan.TotalMilliseconds;
        return this;
    }

    public SingleThreadRoutine SetSubRoutine(SubRoutine subRoutine)
    {
        routines.Add(subRoutine);
        return this;
    }

    /// <summary>
    /// Starts the execution loop in a single worker thread.
    /// </summary>
    /// <returns></returns>
    public SingleThreadRoutine Start()
    {
        isRunning = true;
        thread.Start();
        return this;
    }

    void OnException(Exception e)
    {
        if (LogExceptions)
        {
            ExceptionLogger.Log(e);
        }
    }

    void ThreadLoop()
    {
        while (isRunning)
        {
            try
            {
                foreach (var routine in routines)
                {
                    routine.Execute();
                }
            }
            catch (Exception e)
            {
                OnException(e);
            }
            finally
            {
                Thread.Sleep(executionIntervalMilliseconds);
            }
        }
    }
}

public abstract class SubRoutine
{
    /// <summary>
    /// Invoked by the routine's worker thread.
    /// </summary>
    public abstract void Execute();
}