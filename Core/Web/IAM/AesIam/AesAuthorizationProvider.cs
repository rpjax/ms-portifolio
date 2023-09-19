using Microsoft.AspNetCore.Http;
using ModularSystem.Core.Security;
using ModularSystem.Web.Attributes;
using ModularSystem.Web.Authorization;

namespace ModularSystem.Web.Authentication;

public class AesAuthorizationProvider : IAuthorizationProvider
{
    public Task<IResourcePolicy?> GetResourcePolicyAsync(HttpContext httpContext)
    {
        throw new NotImplementedException();
    }

    private IdentityActionAttribute? GetActionAttributeFrom(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        if (endpoint == null)
        {
            return null;
        }

        var actionAttribute = endpoint.Metadata.GetMetadata<IdentityActionAttribute>();

        if (actionAttribute == null)
        {
            return null;
        }

        return actionAttribute;
    }
}