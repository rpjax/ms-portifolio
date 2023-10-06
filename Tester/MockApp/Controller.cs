using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ModularSystem.Core;
using ModularSystem.Core.Helpers;
using ModularSystem.Core.Logging;
using ModularSystem.Core.Security;
using ModularSystem.Web;

namespace ModularSystem.Tester;

[ApiController, Route("")]
public class RootController : WebController
{
    public RootController()
    {

    }

}

[Route("api/exception-logs")]
public class ExceptionLogsController : CrudController<ExceptionEntry>
{
    protected override EntityService<ExceptionEntry> Entity { get; }

    public ExceptionLogsController()
    {
        Entity = new ExceptionEntryEntity();
    }
}

[Route("api/mongo-ant")]
public class TestModelController : CrudController<MongoAnt>
{
    protected override EntityService<MongoAnt> Entity { get; }

    public TestModelController()
    {
        Entity = new MongoAntEntity();
    }
}

[Route("api/tester")]
public class Testercontroller : WebController
{
    [HttpPost("resource")]
    public virtual async Task<IActionResult> CreateAsync(string name, [BindNever] IResourcePolicy? resourcePolicy = null)
    {
        try
        {
            var iam = DependencyContainer.Get<AesIamSystem>();
            var userDomain = "users";
            var identityResource = "identities";
            var roles = new IdentityRole[]
            {
                DefinedRoles.Admin(userDomain, identityResource),
                DefinedRoles.CreateReadOnly(userDomain, identityResource),
            };
            var identity = new Identity(name)
                .AddRoles(roles);

            var token = iam.AuthenticationProvider.GetToken(identity);
            var stringToken = iam.GetAuthenticationProvider().GetTokenEncrypter().Encrypt(token);

            return Ok(new { token = stringToken });
        }
        catch (Exception e)
        {
            return HandleException(e);
        }
    }

    [HttpGet("resource")]
    public virtual async Task<IActionResult> CreateAsync([BindNever] IResourcePolicy? resourcePolicy = null)
    {
        try
        {
            var authHeader = GetAuthorizationHeader();
            var bearer = GetBearerToken();

            resourcePolicy = new Resource("user", "identity")
                .SetRequiredPermission(DefinedPermissions.Read)
                .GetResourcePolicy();

            var identity = TryGetIdentity();
            var isAuthorized = await resourcePolicy.TryAuthorizeAsync(identity);
            var data = LoremIpsum.Sample();

            return Ok(new { data, isAuthorized });
        }
        catch (Exception e)
        {
            return HandleException(e);
        }
    }
}
