using Microsoft.AspNetCore.Mvc;
using ModularSystem.Core;
using ModularSystem.Mongo;
using ModularSystem.Web;

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
public class MyDataController : WebqlCrudController<MyData>
{
    protected override EntityService<MyData> Service { get; }

    public MyDataController()
    {
        Service = new MyDataService();
        
    }

}
