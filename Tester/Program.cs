using Microsoft.AspNetCore.Mvc;
using ModularSystem.Core;
using ModularSystem.Mongo;
using ModularSystem.Mongo.Webql;
using ModularSystem.Web;
using ModularSystem.Webql;
using ModularSystem.Webql.Analysis;
using ModularSystem.Webql.Synthesis;

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
        Initializer.Run(new() { InitConsoleLogger = true });

        WebApplicationServer.StartSingleton();
        return;

        var json = "{\r\n    \"$limit\": 25,\r\n    \"$filter\": {\r\n        \"cpf\": {\r\n            \"$equals\": \"11548128988\"\r\n        },\r\n        \"firstName\": {\r\n            \"$equals\": \"Rodrigo\"\r\n        },\r\n        \"surnames\": {\r\n            \"$any\": {\r\n                \"$or\":[\r\n                    { \"$equals\": \"Jacques\" },\r\n                    { \"$equals\": \"Aamanda\" }\r\n                ]\r\n            },\r\n            \"$count\": {},\r\n            \"$greater\": 1\r\n        }\r\n    }\r\n}";


        var translator = new Translator(new() { LinqProvider = new MongoLinqProvider() });
        using var service = new MyDataService();
        var serviceQueryable = service.AsQueryable();

        var translatedQueryable = translator.TranslateToQueryable(json, service.AsQueryable());
        var mongoQueryable = new MongoTranslatedQueryable(translatedQueryable);

        Console.WriteLine(translatedQueryable.Expression);

        var data = mongoQueryable.ToListAsync().Result;

        Console.WriteLine(JsonSerializerSingleton.Serialize(data));
    }

    static void PopulateService()
    {
        using var service = new MyDataService();
        var count = service.CountAllAsync().Result;

        if (count > 0)
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

[Route("my-data")]
public class MyDataController : WebQlController<MyData>
{
    protected override EntityService<MyData> Service { get; }

    public MyDataController()
    {
        Service = new MyDataService();
    }

}