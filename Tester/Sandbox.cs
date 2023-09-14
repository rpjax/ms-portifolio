using ModularSystem.Core;
using ModularSystem.Core.Cli;
using ModularSystem.Core.Cli.Commands;
using ModularSystem.Core.Security;
using ModularSystem.Mailing;
using ModularSystem.Mongo;

namespace ModularSystem.Tester;

public partial class Sandbox : CliCommand
{
    class Env
    {
        public string? Environment { get; set; }
        public string[]? Uris { get; set; }
    }

    public override void Execute(CLI cli, PromptContext context)
    {
        Identity a;
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

    public class TestModelEntity : MongoEntity<MongoTestModel>
    {
        public override IDataAccessObject<MongoTestModel> DataAccessObject { get; }

        public TestModelEntity()
        {
            DataAccessObject = new MongoDataAccessObject<MongoTestModel>(DatabaseSource.TestModel);
            Validator = new EmptyValidator<MongoTestModel>();
            UpdateValidator = new EmptyValidator<MongoTestModel>();
        }
    }
}