using Microsoft.AspNetCore.Mvc;
using ModularSystem.Core;
using ModularSystem.Web;
using System.Linq.Expressions;
using Webql;

namespace Tester;

public class WebqlController<TEntity> : WebController where TEntity : IEntity
{
    private WebqlCompiler Compiler { get; }
    private IRepositoryProvider RepositoryProvider { get; }

    public WebqlController(WebqlCompiler compiler, IRepositoryProvider repositoryProvider)
    {
        Compiler = compiler;
        RepositoryProvider = repositoryProvider;
    }

    public async Task<IActionResult> QueryAsync()
    {
        var query = await ReadBodyAsStringAsync();

        var expression = Compiler.Compile(query ?? "", typeof(TEntity));
        var function = expression.Compile();

        var result = function.DynamicInvoke(RepositoryProvider.GetRepository<TEntity>().AsQueryable());

        Console.WriteLine(result);

        return Ok();
    }
}
