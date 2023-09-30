using ModularSystem.Core.Threading;

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

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var writer = logger.GetWriter();
        return writer.WriteAsync(entry);
    }

    protected override Task OnExceptionAsync(Exception e, CancellationToken cancellationToken)
    {
        Console.WriteLine(e.ToString());
        return Task.CompletedTask;
    }
}