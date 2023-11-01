﻿using ModularSystem.Core;
using ModularSystem.Core.Cli;
using ModularSystem.Core.Logging;
using ModularSystem.Core.Security;
using ModularSystem.Core.TextAnalysis;
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
        var reader = new GDefReader();
        var prods = reader.GetProdutions(new FileInfo("C:\\RPJ\\Coding\\Sandbox\\Compiler\\Formats\\example.gdef"));
        reader.GetDefinitions(new FileInfo("C:\\RPJ\\Coding\\Sandbox\\Compiler\\Formats\\example.gdef"));
        //WebApplicationServer.StartSingleton();
    }
}

//*
// NOTE:
//*
public class ExceptionEntryEntity : EFEntityService<ExceptionEntry>
{
    public override IDataAccessObject<ExceptionEntry> DataAccessObject { get; }

    public ExceptionEntryEntity()
    {
        var file = Logger.DefaultPathFile(ExceptionLogger.DefaultFileName);
        var context = new EFCoreSqliteContext<ExceptionEntry>(file);

        DataAccessObject = new EFCoreDataAccessObject<ExceptionEntry>(context);
        Validator = new EmptyValidator<ExceptionEntry>();
        UpdateValidator = new EmptyValidator<ExceptionEntry>();
    }

}