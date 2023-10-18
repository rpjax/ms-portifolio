using Microsoft.AspNetCore.Http;
using ModularSystem.Core;
using ModularSystem.Web.Attributes;

namespace ModularSystem.Web;

public class AttributesHandlerMiddleware : Middleware
{
    public AttributesHandlerMiddleware(RequestDelegate next) : base(next)
    {
    }

    protected override async Task<Strategy> BeforeNextAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        if (endpoint == null)
        {
            return Strategy.Continue;
        }

        var forbbidenActionAttribute = endpoint.Metadata.GetMetadata<ForbbidenActionAttribute>();

        if (forbbidenActionAttribute != null)
        {
            await WriteErrorResponseAsync(context, new AppException("", ExceptionCode.Forbidden));
            return Strategy.Break;
        }

        return Strategy.Continue;
    }

}
