using DnsClient;
using ModularSystem.Core;
using ModularSystem.Core.Cli;
using ModularSystem.EntityFramework;
using ModularSystem.Mailing;
using ModularSystem.Mongo;
using ModularSystem.Web;
using ModularSystem.Web.Expressions;
using MongoDB.Bson;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace ModularSystem.Tester;

public partial class Sandbox : CliCommand
{
    public Sandbox(CLI cli, PromptContext context) : base(cli, context)
    {

    }

    class Env
    {
        public string? Environment { get; set; }
        public string[]? Uris { get; set; }
    }

    protected override async void Execute()
    {
        using var service = new EFTestService();

        //for (int i = 0; i < 100; i++)
        //{
        //    service.CreateAsync(new EFTestEntity()
        //    {
        //        Nickname = "jacques",
        //        FirstName = "Rodrigo",
        //        Email = new("jacques@more.tt")
        //    }).Wait();  
        //}

        //await Console.Out.WriteLineAsync("success");
        //return;

        var query = new QueryWriter<EFTestEntity>()
                .SetLimit(50)
                .SetFilter(x => x.Nickname.ToLower().Contains("amanda"))
                .CreateSerializable();

        var rebQuery = query.ToQuery<EFTestEntity>();

        var update = new UpdateWriter<EFTestEntity>()
                .SetFilter(x => x.Id <= 90)
                .SetModification(x => x.Email.Username, "yummandy")
                //.SetModification(x => x.Email.Domain, "gmail")
                //.SetModification(x => x.Email.Extension, "com")
                .CreateSerializable();

        var rebUpdate = update.ToUpdate<EFTestEntity>();

        await service.UpdateAsync(rebUpdate);
        var queryResult = await service.QueryAsync(rebQuery);

        await Console.Out.WriteLineAsync();
    }

    public override string Instruction()
    {
        return "sandbox";
    }

    public class DateTimeTest
    {
        public DateTime Time { get; set; }
    }
}

class MyMiddleware : EntityMiddleware<MongoTestModel>
{
    public override Task<IQuery<MongoTestModel>> BeforeQueryAsync(IQuery<MongoTestModel> query)
    {
        Console.WriteLine("before query!");
        return base.BeforeQueryAsync(query);
    }

    public override Task<IUpdate<MongoTestModel>> BeforeUpdateAsync(IUpdate<MongoTestModel> update)
    {
        Console.WriteLine("before update!");
        return base.BeforeUpdateAsync(update);
    }
}

public class MongoTestModel : MongoModel
{
    public string FirstName { get; set; } = string.Empty;
    public string[] Surnames { get; set; } = Array.Empty<string>();
    public string? Nickname { get; set; }
    public Email Email { get; set; } = Email.Empty();
}

public class MongoTestEntity : MongoEntityService<MongoTestModel>
{
    public override IDataAccessObject<MongoTestModel> DataAccessObject { get; }

    public MongoTestEntity()
    {
        DataAccessObject = new MongoDataAccessObject<MongoTestModel>(DatabaseSource.TestModel);
        Validator = new EmptyValidator<MongoTestModel>();
        UpdateValidator = new EmptyValidator<MongoTestModel>();
    }
}

public class Register
{
    public ushort Value { get; set; }
}

public class SizedMemory
{
    public long Size { get; }
    private byte[] Bytes { get; }

    public SizedMemory(long size)
    {
        Size = size;
        Bytes = new byte[size];
    }
}

public class EmulatedClock
{

}

public class ClockSignalEvent
{

}

public class InterruptHandler 
{
    public ushort Address { get; }
}

public class CpuEmulator
{
    private Register ProgramCounter { get; } = new();
    private ConcurrentDictionary<string, InterruptHandler> InterruptHandlers { get; } = new();

    private void OnClockSignal(ClockSignalEvent clockSignal)
    {

    }
}