using ModularSystem.Core.Jobs;

namespace ModularSystem.Core.Logging;

public class LoggerJob<T> : Job where T : ILogEntry
{
    readonly Logger<T> logger;
    readonly T entry;

    public LoggerJob(Logger<T> logger, T entry)
    {
        this.logger = logger;
        this.entry = entry;
    }

    protected override void Work()
    {
        using var writer = logger.GetWriter();
        writer.Write(entry);
    }

    protected override void OnException(Exception e)
    {
        Console.WriteLine(e.ToString());
    }
}