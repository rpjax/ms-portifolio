using ModularSystem.Core;
using ModularSystem.Core.Logging;
using ModularSystem.Core.TextAnalysis.Gdef;
using ModularSystem.Core.Threading;
using ModularSystem.EntityFramework;

namespace ModularSystem.Tester;

public static class Program
{
    public static void Main()
    {
        var config = new Initializer.Options()
        {
            InitConsoleLogger = true,
        };

        //Initializer.Run(config);

        //var fileInfo = new FileInfo("C:\\RPJ\\Coding\\Sandbox\\Compiler\\Formats\\example.gdef");
        //var grammar = GDefReader.Read(fileInfo);

        JobQueue.Enqueue(new LambdaJob(async x =>
        {
            await Task.Delay(3000);
            await Console.Out.WriteLineAsync("Exited task 1.");
        }));

        JobQueue.Enqueue(new LambdaJob(async x =>
        {
            await Task.Delay(6000);
            await Console.Out.WriteLineAsync("Exited task 2.");
        }));

        Task.WhenAll(new[]
        {
            JobQueue.WaitAllJobsAsync(),
            JobQueue.WaitAllJobsAsync(),
            JobQueue.WaitAllJobsAsync()
        }).Wait();

        JobQueue.WaitAllJobsAsync().Wait();

        JobQueue.Enqueue(new LambdaJob(async x =>
        {
            await Task.Delay(3000);
            await Console.Out.WriteLineAsync("Exited task 3.");
        }));

        JobQueue.Enqueue(new LambdaJob(async x =>
        {
            await Task.Delay(6000);
            await Console.Out.WriteLineAsync("Exited task 4.");
        }));

        JobQueue.WaitAllJobsAsync().Wait();
        Console.WriteLine("Exiting program.");
    }
}

//*
// NOTE:
//*
public class ExceptionEntryService : EFEntityService<ExceptionEntry>
{
    public override IDataAccessObject<ExceptionEntry> DataAccessObject { get; }

    public ExceptionEntryService()
    {
        var file = Logger.DefaultPathFile(ExceptionLogger.DefaultFileName);
        var context = new EFCoreSqliteContext<ExceptionEntry>(file);

        DataAccessObject = new EFCoreDataAccessObject<ExceptionEntry>(context);
        Validator = new EmptyValidator<ExceptionEntry>();
        UpdateValidator = new EmptyValidator<ExceptionEntry>();
    }

}