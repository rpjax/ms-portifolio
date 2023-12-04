using ModularSystem.Core;
using ModularSystem.Core.Logging;
using ModularSystem.EntityFramework;

namespace ModularSystem.Tester;

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