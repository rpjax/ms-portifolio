using ModularSystem.Core;
using ModularSystem.Core.Logging;
using ModularSystem.Core.TextAnalysis.Gdef;
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

        Initializer.Run(config);

        var fileInfo = new FileInfo("C:\\RPJ\\Coding\\Sandbox\\Compiler\\Formats\\example.gdef");
        var grammar = GDefReader.Read(fileInfo);

        Console.WriteLine();
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