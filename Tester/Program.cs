using ModularSystem.Core;
using ModularSystem.Mongo;
using ModularSystem.Mongo.Webql;
using ModularSystem.Web;
using ModularSystem.Webql;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ModularSystem.Tester;

public static class Program
{
    public static void Main()
    {
        var json = "{ \r\n    \"$filter\": { \r\n        \"$and\": [\r\n            { \"cpf\": { \"$equals\": \"11548128988\" } },\r\n            { \"firstName\": { \"$equals\": \"Rodrigo\" } },\r\n            { \r\n                \"surnames\": { \r\n                    \"$any\": { \"$equals\": \"Jacques\" } \r\n                }\r\n            }\r\n        ] \r\n    },\r\n    \"$project\": {\r\n        \"Nome\": { \"$select\": \"$firstName\" },\r\n        \"Sobrenomes\": {\"$select\": \"$surnames\" }\r\n    }\r\n}";
        var parser = new Parser();
        var syntaxTree = parser.Parse(json);
        var generator = new Generator();

        PopulateService();

        using var service = new MyDataService();
        var serviceQueryable = service.AsQueryable();

        var translatedEnumerable = generator.CreateEnumerable(syntaxTree, service.AsQueryable());
        var mongoEnum = new MongoTranslatedEnumerable(translatedEnumerable.InputType, translatedEnumerable.OutputType, translatedEnumerable.Enumerable);

        var serviceQuery = serviceQueryable
           .Where(x => x.Cpf == "11548128988")
           .Select(x => new { Nome = x.FirstName, Sobrenomes = x.Surnames });

        var mongoQueryable = serviceQueryable as IMongoQueryable<MyData>;

        var mongoQueryableQuery = mongoQueryable
           .Where(x => x.Cpf == "11548128988")
           .Select(x => new { Nome = x.FirstName, Sobrenomes = x.Surnames });

        var data = mongoEnum.ToListAsync().Result;
        var result = translatedEnumerable.ToArray(); 

        Console.WriteLine(syntaxTree.ToString());
        //var projectedQueryable = queryable.Select(x => new { Nome = x.FirstName, Sobrenomes = x.Surnames });
        //var anonymousType = TypeHelper.CreateAnonymousType(new AnonymousPropertyDefinition[] 
        //{ 
        //    new("FirstName", typeof(string)),
        //    new("Surnames", typeof(string[]))
        //});

        //      call expression (IQueryable<T>.Select()) arguments:
        //          constant expression (IEnumerable<T>)
        //          quoat expression operand:
        //              lambda expression (Func<T, projectedT>) body:
        //                  new expression:
        //                      members: in order, the lhs of the assignments.
        //                      arguments: in order, the rhs of the assignments.
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

public class MyData : MongoModel
{
    public string FirstName { get; set; } = "";
    public string[] Surnames { get; set; } = new string[0];
    public string Cpf { get; set; } = "";
    public int Score { get; set; }
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
