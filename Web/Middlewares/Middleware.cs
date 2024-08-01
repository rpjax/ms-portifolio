using Microsoft.AspNetCore.Http;

namespace ModularSystem.Web;

/// <summary>
/// Provides a foundational structure for creating custom middleware components in an ASP.NET Core application.
/// </summary>
public abstract class Middleware
{
    /// <summary>
    /// Enumerates strategies for middleware execution flow.
    /// </summary>
    public enum Strategy
    {
        /// <summary>
        /// Continue executing the next middleware in the pipeline.
        /// </summary>
        Continue,

        /// <summary>
        /// Break the middleware execution and do not proceed to the next middleware.
        /// </summary>
        Break
    }

    /// <summary>
    /// Gets the next middleware in the request processing pipeline. <br/>
    /// This delegate is invoked after the current middleware completes its processing.
    /// </summary>
    protected RequestDelegate Next { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Middleware"/> class with the specified next delegate in the pipeline.
    /// </summary>
    /// <param name="next">The next delegate in the request pipeline.</param>
    public Middleware(RequestDelegate next)
    {
        Next = next;
    }

    /// <summary>
    /// Invokes the middleware using the provided context.
    /// Depending on the result of <see cref="BeforeNextAsync"/>, the subsequent middleware in the pipeline might be executed.
    /// Post that, <see cref="AfterNextAsync"/> is executed.
    /// </summary>
    /// <param name="context">The prevailing HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task InvokeAsync(HttpContext context)
    {
        try
        {
            switch (await BeforeNextAsync(context))
            {
                case Strategy.Continue:
                    await Next.Invoke(context);
                    break;
                case Strategy.Break:
                    return;

                default:
                    throw new InvalidOperationException();
            }

            await AfterNextAsync(context);
        }
        catch (Exception e)
        {
            switch (await OnExceptionAsync(context, e))
            {
                case Strategy.Continue:
                    throw;
                case Strategy.Break:
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }
    }

    /// <summary>
    /// Method to be executed before the next middleware in the pipeline.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A task that, when executed, will return true if the next middleware should be executed, otherwise false.</returns>
    protected virtual Task<Strategy> BeforeNextAsync(HttpContext context)
    {
        return Task.FromResult(Strategy.Continue);
    }

    /// <summary>
    /// Method to be executed after the next middleware in the pipeline.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A task that, when executed, will return true if reserved future additions should be executed, otherwise false.</returns>
    protected virtual Task AfterNextAsync(HttpContext context)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles exceptions that occur during the processing of the middleware.
    /// This method provides a mechanism to handle exceptions in a centralized manner, 
    /// allowing for custom error handling, logging, or response modification.
    /// By default, this method re-throws the exception, allowing it to be caught by 
    /// any subsequent error-handling middleware in the pipeline. Override this method 
    /// in derived classes to implement custom exception handling behavior.
    /// </summary>
    /// <param name="context">The prevailing HTTP context.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected virtual Task<Strategy> OnExceptionAsync(HttpContext context, Exception exception)
    {
        return Task.FromResult(Strategy.Continue);
    }

}
