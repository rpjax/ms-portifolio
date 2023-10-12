﻿using ModularSystem.Core;
using ModularSystem.Core.Cli;
using ModularSystem.Mailing;
using ModularSystem.Mongo;
using ModularSystem.Web.Expressions;
using MongoDB.Bson;
using System.Linq.Expressions;
using static ModularSystem.Tester.Sandbox;

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

        if (Context.GetFlag("list"))
        {
            var _query = new QueryWriter<MongoTestModel>()
                .SetLimit(50)
                .SetFilter(x => x.FirstName != "Rodrigo")
                .Create();
            var _queryResult = await service.QueryAsync(_query);

            foreach (var item in _queryResult.Data)
            {
                Console.WriteLine("record:");
                Console.WriteLine($"  id: {item.Id}");
                Console.WriteLine($"  email: {item.Email}");
                Console.WriteLine($"  first name: {item.FirstName}");
            }
        }
        else if (Context.GetFlag("update"))
        {
            var _id = new ObjectId("64dcf50977052318c964670e");
            var _update = new UpdateWriter<MongoTestModel>()
                .SetFilter(x => x.Surnames.Contains("Richthofen"))
                .SetModification(x => x.HasKilledChild, true)
                .Create();

            await service.UpdateAsync(_update);

            await Console.Out.WriteLineAsync("entity updated.");
        }
        else
        {
            await Console.Out.WriteLineAsync("No valid flags were provided.");
        }

        var query = new QueryWriter<MongoTestModel>()
                .SetLimit(50)
                .SetFilter(x => 50 == 50 && x.Nickname.Contains("becetinha apertada"))
                .Create();

        var update = new UpdateWriter<MongoTestModel>()
                .SetFilter(x => x.Surnames.Contains("Richthofen"))
                .SetModification(x => x.HasKilledChild, true)
                .CreateSerialized();

        Console.WriteLine(update);
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