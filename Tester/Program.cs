using ModularSystem.Core;
using ModularSystem.Core.Cli;
using ModularSystem.Core.Cryptography;
using ModularSystem.Core.Logging;
using ModularSystem.Core.Security;
using ModularSystem.EntityFramework;
using ModularSystem.Web.Authentication;
using System.Text.Json;

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
        Test();
        CLI.StartInstance();
        //WebApplicationServer.StartSingleton();
    }

    static void Test()
    {
        HashContext a;
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