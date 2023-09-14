using Microsoft.AspNetCore.Builder;
using ModularSystem.Core;
using ModularSystem.Web.Authentication;

namespace ModularSystem.Web;

/// <summary>
/// Provides extension methods for the <see cref="IApplicationBuilder"/> to facilitate the integration of the IAM (Identity and Access Management) system.
/// </summary>
public static class IApplicationBuilderExtension
{
    /// <summary>
    /// Registers the IAM system and its middlewares into the application's request pipeline.
    /// </summary>
    /// <param name="builder">The application builder to which the middlewares should be added.</param>
    /// <param name="iamSystem">The IAM system manager responsible for handling authentication and authorization operations.</param>
    /// <returns>The updated <see cref="IApplicationBuilder"/> with the IAM system middlewares added.</returns>
    public static IApplicationBuilder UseIamSystem(this IApplicationBuilder builder, IIamSystem iamSystem)
    {
        DependencyContainer.TryRegister(iamSystem);

        return builder
            .UseMiddleware<IamAuthenticationMiddleware>();
    }
}
