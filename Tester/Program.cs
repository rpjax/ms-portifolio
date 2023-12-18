using Microsoft.AspNetCore.Mvc;
using ModularSystem.Core;
using ModularSystem.Mongo;
using ModularSystem.Web;
using ModularSystem.Webql;
using ModularSystem.Webql.Synthesis;
using MongoDB.Driver.Linq;

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

    [HttpPost("webql-debug")]
    public async Task<IActionResult> DebugQuery()
    {
        try
        {
            var json = (await ReadBodyAsStringAsync()) ?? Translator.EmptyQuery;
            var translator = new Translator(GetTranslatorOptions());
            var syntaxTree = translator.RunAnalysis(json, typeof(IEnumerable<MyData>));
            //var expression = translator.TranslateToExpression(json, typeof(IEnumerable<MyData>));

            return Ok(syntaxTree.ToString());
        }
        catch (Exception e)
        {
            if (e is ParseException parseException)
            {
                return HandleException(new AppException(parseException.GetMessage(), ExceptionCode.InvalidInput));
            }

            return HandleException(e);
        }
    }
}
