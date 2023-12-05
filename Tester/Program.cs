using ModularSystem.Core;
using ModularSystem.Mongo;
using ModularSystem.Mongo.Webql;
using ModularSystem.Webql;
using ModularSystem.Webql.Synthesis;
using MongoDB.Driver;

namespace ModularSystem.Tester;

public class MyData : MongoModel
{
    public string FirstName { get; set; } = "";
    public string[] Surnames { get; set; } = new string[0];
    public string Cpf { get; set; } = "";
    public int Score { get; set; }
}

public static class Program
{
    public static void Main()
    {
        Initializer.Run();
        var json = "{\r\n    \"$filter\": {\r\n        \"firstName\": {\r\n            \"$or\":[\r\n                { \"$equals\": \"Rodrigo\" },\r\n                { \"$equals\": \"Amanda\" }\r\n            ]\r\n        },\r\n        \"$equals\": [\"$cpf\", \"11548128988\"],\r\n        \"surnames\":{\r\n            \"$any\": { \r\n                \"$equals\": \"Jacques\"\r\n            }\r\n        }\r\n    }\r\n}";

        var parser = new Parser();
        var syntaxTree = parser.Parse(json);
        var generator = new Generator();

        using var service = new MyDataService();
        var serviceQueryable = service.AsQueryable();

        var translatedQueryable = generator.CreateQueryable(syntaxTree, service.AsQueryable());
        var mongoQueryable = new MongoTranslatedQueryable(translatedQueryable);

        var data = mongoQueryable.ToListAsync().Result;

        Console.WriteLine(JsonSerializerSingleton.Serialize(data));
    }

    static void PopulateService()
    {
        using var service = new MyDataService();
        var count = service.CountAllAsync().Result;

        if(count > 0)
        {
            return;
        }

        var data = new MyData[]
        {
            new(){ Cpf = "11709620927", FirstName = "Amanda", Surnames = new[]{ "de", "Lima", "Santos" }, Score = 98 },
            new(){ Cpf = "11548128988", FirstName = "Rodrigo", Surnames = new[]{ "Pazzini", "Jacques" }, Score = 85 },
        };

        service.CreateAsync(data).Wait();
    }
}

public class MyDataService : MongoEntityService<MyData>
{
    public override IDataAccessObject<MyData> DataAccessObject { get; }

    public MyDataService()
    {
        DataAccessObject = CreateDao();
    }

    private static IDataAccessObject<MyData> CreateDao()
    {
        return new MongoDataAccessObject<MyData>(MongoDb.GetCollection<MyData>("my_data_temp"));
    }
}
