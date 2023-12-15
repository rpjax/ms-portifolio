using Microsoft.AspNetCore.Mvc;
using ModularSystem.Core;
using ModularSystem.Mongo;
using ModularSystem.Web;
using ModularSystem.Web.Expressions;
using System.Linq.Expressions;

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
        var foo = Expression.Parameter(typeof(int), "x");
        var bar = Expression.Parameter(typeof(int), "x");

        var fooHash = foo.GetHashCode();
        var barHash = bar.GetHashCode();

        Console.WriteLine($"foo equals bar: {fooHash == barHash}");

        WebApplicationServer.StartSingleton();
        return;
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
public class MyDataController : QueryableController<MyData>
{
    protected override EntityService<MyData> Service { get; }

    public MyDataController()
    {
        Service = new MyDataService();
        
    }


}
