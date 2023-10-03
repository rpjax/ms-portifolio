using ModularSystem.Core;
using ModularSystem.Core.Cli;
using ModularSystem.Core.Logging;
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

        Initializer.Run(config);
        CLI.StartInstance();
        EntityService
        //WebApplicationServer.StartSingleton();
    }
}

//*
// NOTE:
//*
public class ExceptionEntryEntity : EFEntity<ExceptionEntry>
{
    public override IDataAccessObject<ExceptionEntry> DataAccessObject { get; }

    public ExceptionEntryEntity()
    {
        var file = Logger.DefaultPathFile(ExceptionLogger.DEFAULT_FILE_NAME);
        var context = new EFCoreContext<ExceptionEntry>(file);

        DataAccessObject = new EFCoreDataAccessObject<ExceptionEntry>(context);
        Validator = new EmptyValidator<ExceptionEntry>();
        UpdateValidator = new EmptyValidator<ExceptionEntry>();
    }

}