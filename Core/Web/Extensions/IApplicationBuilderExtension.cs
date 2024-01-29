using Microsoft.AspNetCore.Builder;
using ModularSystem.Core;

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
    /// <returns>The updated <see cref="IApplicationBuilder"/> with the IAM system middlewares added.</returns>
    public static IApplicationBuilder UseIamService(this IApplicationBuilder builder)
    {
        return builder
            .UseMiddleware<IamAuthenticationMiddleware>()
            .UseMiddleware<IamAuthorizationMiddleware>();
    }

    /// <summary>
    /// Adds a middleware of type <typeparamref name="T"/> to the application's request pipeline.
    /// This extension method is specifically designed for exception handling middlewares.
    /// </summary>
    /// <typeparam name="T">The type of the middleware to add. Must be derived from <see cref="Middleware"/>.</typeparam>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
    /// <returns>The same <see cref="IApplicationBuilder"/> instance so that additional calls can be chained.</returns>
    public static IApplicationBuilder UseExceptionHandlerMiddleware<T>(this IApplicationBuilder builder) where T : Middleware
    {
        return builder
            .UseMiddleware(typeof(T));
    }

    /// <summary>
    /// Adds the <see cref="ExceptionHandlerMiddleware"/> to the application's request pipeline. <br/>
    /// This is a convenience method for adding the default exception handler middleware.
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
    /// <returns>The same <see cref="IApplicationBuilder"/> instance so that additional calls can be chained.</returns>
    public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
    {
        return builder
            .UseMiddleware<ExceptionHandlerMiddleware>();
    }

    public static IApplicationBuilder UseAttributeHandlerMiddleware(this IApplicationBuilder builder)
    {
        return builder
            .UseMiddleware<AttributesHandlerMiddleware>();
    }

}
