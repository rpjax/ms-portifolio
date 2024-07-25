//using Microsoft.AspNetCore.Http;
//using ModularSystem.Core;

//namespace ModularSystem.Web;

///// <summary>
///// Middleware component responsible for enforcing authorization policies on incoming HTTP requests.
///// This middleware utilizes an <see cref="IAccessManagementService"/> to determine if the current request is authorized
///// to proceed based on the application's defined authorization policies.
///// </summary>
//public class AuthorizationMiddleware : Middleware
//{
//    /// <summary>
//    /// Access management service used to perform authorization checks.
//    /// </summary>
//    private IAccessManagementService Service { get; }

//    /// <summary>
//    /// Initializes a new instance of the <see cref="AuthorizationMiddleware"/> with the necessary services.
//    /// </summary>
//    /// <param name="next">The next request delegate in the middleware pipeline.</param>
//    /// <param name="service">The access management service used for authorization checks.</param>
//    public AuthorizationMiddleware(RequestDelegate next, IAccessManagementService service) : base(next)
//    {
//        Service = service;
//    }

//    /// <summary>
//    /// Executes the middleware logic before the next component in the pipeline. It attempts to authorize
//    /// the current request using the configured <see cref="IAccessManagementService"/>. If authorization fails,
//    /// the request is short-circuited with an appropriate error response.
//    /// </summary>
//    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
//    /// <returns>
//    /// A <see cref="Middleware.Strategy"/> indicating whether to continue with the next middleware in the pipeline or break
//    /// execution. If authorization is successful, execution continues; otherwise, it breaks and an error response is sent.
//    /// </returns>
//    /// <remarks>
//    /// This method is crucial for protecting sensitive resources and operations within the application. By checking
//    /// the authorization status before proceeding, it ensures that only requests from authorized users are allowed to
//    /// access restricted endpoints. If a request is unauthorized, a 401 Unauthorized response is generated, providing
//    /// clear feedback to the client about the failure reason.
//    /// </remarks>
//    protected override async Task<Strategy> BeforeNextAsync(HttpContext context)
//    {
//        var authorizationResult = await Service.AuthorizationService.AuthorizeAsync(context);

//        if (authorizationResult.IsSuccess)
//        {
//            return Strategy.Continue;
//        }

//        var message = "The authenticated user lacks the required permissions to execute this operation. Ensure you have the right privileges.";
//        var error = new Error(message)
//            .AddFlags(ErrorFlags.Public);

//        authorizationResult.AddErrors(error);

//        await context.WriteOperationResponseAsync(authorizationResult, 401);
//        return Strategy.Break;
//    }
//}
