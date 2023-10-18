using Microsoft.AspNetCore.Builder;

namespace ModularSystem.Web;

/// <summary>
/// Represents an HTTP server that facilitates the creation and startup of a web application.
/// </summary>
public class HttpServer
{
    /// <summary>
    /// Gets the URI where the server will be hosted.
    /// </summary>
    private Uri HostUri { get; set; }

    /// <summary>
    /// Gets the command-line arguments provided during server initialization.
    /// </summary>
    private string[]? Args { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpServer"/> class.
    /// </summary>
    /// <param name="hostUri">The URI where the server will be hosted.</param>
    /// <param name="args">The command-line arguments provided during server initialization.</param>
    public HttpServer(Uri hostUri, string[]? args)
    {
        HostUri = hostUri;
        Args = args;
    }

    /// <summary>
    /// Starts the web application, making it available for incoming requests.
    /// </summary>
    public void Start()
    {
        CreateWebApplication().Run(HostUri.ToString());
    }

    /// <summary>
    /// Creates a new instance of <see cref="WebApplicationBuilder"/> and configures it.
    /// </summary>
    /// <returns>A configured instance of <see cref="WebApplicationBuilder"/>.</returns>
    private WebApplicationBuilder CreateApplicationBuilder()
    {
        var builder = WebApplication.CreateBuilder(Args ?? Array.Empty<string>());
        OnApplicationBuilderCreated(builder);
        return builder;
    }

    /// <summary>
    /// Creates and configures a new instance of <see cref="WebApplication"/>.
    /// </summary>
    /// <returns>A configured instance of <see cref="WebApplication"/>.</returns>
    private WebApplication CreateWebApplication()
    {
        var app = CreateApplicationBuilder().Build();
        OnWebApplicationCreated(app);
        return app;
    }

    /// <summary>
    /// Invoked after the <see cref="WebApplicationBuilder"/> has been created.
    /// Provides an opportunity to further configure the builder.
    /// </summary>
    /// <param name="builder">The created <see cref="WebApplicationBuilder"/>.</param>
    protected virtual void OnApplicationBuilderCreated(WebApplicationBuilder builder)
    {

    }

    /// <summary>
    /// Invoked after the <see cref="WebApplication"/> has been created.
    /// Provides an opportunity to further configure the application.
    /// </summary>
    /// <param name="app">The created <see cref="WebApplication"/>.</param>
    protected virtual void OnWebApplicationCreated(WebApplication app)
    {

    }
}
