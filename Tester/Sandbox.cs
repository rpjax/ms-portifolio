using DnsClient;
using ModularSystem.Core;
using ModularSystem.Core.Cli;
using ModularSystem.Mailing;
using ModularSystem.Mongo;
using ModularSystem.Web.Expressions;
using MongoDB.Bson;
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
        using var service = new MongoTestEntity();

        var query = new QueryWriter<MongoTestModel>()
                .SetLimit(50)
                .SetFilter(x => 50 == 50 && x.FirstName.ToLower().Contains("ama"))
                .CreateSerializable();

        var rebQuery = query.ToQuery<MongoTestModel>();

        var update = new UpdateWriter<MongoTestModel>()
                .SetFilter(x => x.FirstName.ToLower().Contains("amanda"))
                .SetModification(x => x.Email, new Email("amandatoques@gmail.com"))
                .CreateSerializable();

        var rebUpdate = update.ToUpdate<MongoTestModel>();

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
    public bool HasKilledChild { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string[] Surnames { get; set; } = Array.Empty<string>();
    public string? Nickname { get; set; }
    public Email Email { get; set; } = Email.Empty();
}

public class MongoTestEntity : MongoEntity<MongoTestModel>
{
    public override IDataAccessObject<MongoTestModel> DataAccessObject { get; }

    public MongoTestEntity()
    {
        DataAccessObject = new MongoDataAccessObject<MongoTestModel>(DatabaseSource.TestModel);
        Validator = new EmptyValidator<MongoTestModel>();
        UpdateValidator = new EmptyValidator<MongoTestModel>();
    }
}