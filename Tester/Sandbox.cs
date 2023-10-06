using ModularSystem.Core;
using ModularSystem.Core.Cli;
using ModularSystem.Core.Initialization;
using ModularSystem.Core.Security;
using ModularSystem.Mailing;
using ModularSystem.Mongo;
using MongoDB.Bson;

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
        service.AddMiddleware<MyMiddleware>();

        if (Context.GetFlag("list"))
        {
            var query = new QueryWriter<MongoTestModel>()
                .SetLimit(50)
                .SetFilter(x => x.FirstName != "Rodrigo")
                .Create();
            var queryResult = await service.QueryAsync(query);

            foreach (var item in queryResult.Data)
            {
                Console.WriteLine("record:");
                Console.WriteLine($"  id: {item.Id}");
                Console.WriteLine($"  email: {item.Email}");
                Console.WriteLine($"  first name: {item.FirstName}");
            }
        }
        else if (Context.GetFlag("update"))
        {
            var id = new ObjectId("64dcf50977052318c964670e");
            var update = new UpdateWriter<MongoTestModel>()
                //.SetFilter(x => x.Id == id)
                .AddModification(x => x.FirstName, "Amanda")
                .Create();

            await service.UpdateAsync(update);

            await Console.Out.WriteLineAsync("entity updated.");
        }
        else
        {
            await Console.Out.WriteLineAsync("No valid flags were provided.");
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

    public override string Instruction()
    {
        return "sandbox";
    }

    public class DateTimeTest
    {
        public DateTime Time { get; set; }
    }

    public class MongoTestModel : MongoModel
    {
        public string FirstName { get; set; }
        public string[] Surnames { get; set; }
        public string? Nickname { get; set; }
        public Email Email { get; set; }

        private MongoTestModel()
        {
            //*
            // this constructor exists so that LINQ providers can instantiate this object with no params.
            //*

            FirstName = string.Empty;
            Surnames = new string[0];
            Email = Email.Empty();
        }

        public MongoTestModel(string firstName, string[] surnames, Email email)
        {
            FirstName = firstName;
            Surnames = surnames;
            Email = email;
        }
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
}