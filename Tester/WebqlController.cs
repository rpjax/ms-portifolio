using Microsoft.AspNetCore.Mvc;
using ModularSystem.Core;
using ModularSystem.Core.Patterns;
using ModularSystem.Web.AccessManagement.Attributes;
using ModularSystem.Web.AccessManagement.Jwt.Services;
using ModularSystem.Web.Controllers;
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

        var x = new OperationResult<string>(new Error(title: ""));

        Console.WriteLine(result);

        return Ok();
    }

    [AuthorizeAction("auth:sign-in")]
    public IActionResult SignIn([FromQuery] string username, [FromQuery] string password)
    {
        if(username != "jacques" || password != "nderakore")
        {
            return ProblemResponse(401, title: "Unauthorized", detail: "Invalid credentials");
        }

        var tokenService = GetRequiredService<JwtIdentityService>();

        var identity = new JwtIdentityBuilder()
            .AddPermission("admin")
            .Build();

        var token = tokenService.CreateToken(identity);

        return Ok();
    }
}


