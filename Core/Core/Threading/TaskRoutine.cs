namespace ModularSystem.Core.Threading;

public class TaskRoutine : IDisposable
{
    private bool IsRunning { get; set; }
    private TimeSpan Delay { get; set; }
    private ScheduledTask? ScheduledTask { get; set; }

    public TaskRoutine(TimeSpan delay, ScheduledTask? scheduledTask = null)
    {
        Delay = delay;
        ScheduledTask = scheduledTask;
    }

    public void Dispose()
    {
        IsRunning = false;
    }

    public TaskRoutine SetSleepTime(TimeSpan timeSpan)
    {
        Delay = timeSpan;
        return this;
    }

    public TaskRoutine SetScheduledTask(ScheduledTask scheduledTask)
    {
        ScheduledTask = scheduledTask;
        return this;
    }

    public TaskRoutine SetScheduledTask(Func<Task> callback)
    {
        ScheduledTask = new LambdaScheduledTask(callback);
        return this;
    }

    public void Start()
    {
        TimerQueueTimer timer;
    }

    public void Stop() { }

    private async Task ExecuteAsync()
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
                OnException(e);
            }
        }
    }

    private void OnException(Exception e)
    {

    }

    private async Task IterationWork()
    {
        if (ScheduledTask == null)
        {
            return;
        }

        try
        {
            await ScheduledTask.ExecuteAsync();
        }
        catch (Exception e)
        {
            await ScheduledTask.OnExceptionAsync(e);
        }
        finally
        {
            ScheduledTask.OnExit();
        }
    }
}

public abstract class ScheduledTask
{
    /// <summary>
    /// Invoked by the routine's worker thread.
    /// </summary>
    public abstract Task ExecuteAsync();


    public virtual Task OnExceptionAsync(Exception e)
    {
        return Task.CompletedTask;
    }

    public virtual void OnExit()
    {

    }
}

internal class LambdaScheduledTask : ScheduledTask
{
    protected Func<Task> Callback { get; set; }

    public LambdaScheduledTask(Func<Task> callback)
    {
        Callback = callback;
    }

    public override Task ExecuteAsync()
    {
        return Callback.Invoke();
    }
}