using Microsoft.AspNetCore.Http;
using Aidan.Core;
using Aidan.Web.Attributes;

namespace Aidan.Web;

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
            var error = new Error("This operation is forbidden.")
                .AddFlags(ErrorFlags.UserVisible);

            var result = new OperationResult(error);

            await context.WriteOperationResponseAsync(result, 403);

            return Strategy.Break;
        }

        return Strategy.Continue;
    }

}
