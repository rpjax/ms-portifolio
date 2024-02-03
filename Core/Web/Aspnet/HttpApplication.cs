using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ModularSystem.Core;

namespace ModularSystem.Web;

/// <summary>
/// Represents a configurable HTTP application based on ASP.NET Core. <br/>
/// Provides mechanisms to configure and run a web application.
/// </summary>
public class HttpApplication : IAsyncDisposable
{
    //*
    // Services settings.
    //*

    /// <summary>
    /// Flag to enable or disable ASP.NET Controllers.
    /// </summary>
    public bool AddAspnetControllers { get; set; } = true;

    /// <summary>
    /// Flag to enable or disable the API explorer functionality. Useful for Swagger/OpenAPI support.
    /// </summary>
    public bool AddAspnetApiExplorer { get; set; } = true;

    /// <summary>
    /// Flag to enable or disable global JSON converters, which affect serialization and deserialization behavior.
    /// </summary>
    public bool AddGlobalJsonConverters { get; set; } = true;

    //*
    // Middlewares settings.
    //*

    /// <summary>
    /// Flag to enable or disable a global exception handler middleware. Useful for centralized error handling.
    /// </summary>
    public bool UseExceptionHandler { get; set; } = true;

    /// <summary>
    /// Flag to enable or disable MVC controller routing. Essential for applications using MVC controllers.
    /// </summary>
    public bool UseAspnetControllerMapping { get; set; } = true;

    //*
    // Other settings.
    //*

    /// <summary>
    /// Gets or sets an arbitrary name to label this instance of application.
    /// </summary>
    public string? Name { get; set; }    

    /// <summary>
    /// The URI where the server will be hosted.
    /// </summary>
    protected URI HostUri { get; set; }

    /// <summary>
    /// Command-line arguments provided during server initialization.
    /// </summary>
    protected string[] Args { get; set; }

    private WebApplication? App { get; set; }

    /// <summary>
    /// Initializes a new instance of the HttpServer class with the specified host URI and command-line arguments.
    /// </summary>
    /// <param name="hostUri">The URI where the server will be hosted.</param>
    /// <param name="args">Command-line arguments provided during server initialization.</param>
    public HttpApplication(URI hostUri, params string[] args)
    {
        HostUri = hostUri;
        Args = args;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if(App != null)
        {
            await App.DisposeAsync();
        }
    }

    /// <summary>
    /// Starts the web application, making it available for handling incoming requests.
    /// </summary>
    public virtual void Start()
    {
        if (App != null)
        {
            throw new InvalidOperationException("The HTTP application is already running.");
        }

        var builder = WebApplication.CreateBuilder(Args);
        OnBuilderCreated(builder);
        var app = builder.Build();
        OnApplicationCreated(app);
        App = app;
        App.Run(HostUri.ToString());
    }

    /// <summary>
    /// Stops the web application gracefully.
    /// </summary>
    /// <returns>A task representing the asynchronous stop operation.</returns>
    public virtual Task Stop()
    {
        if (App == null)
        {
            throw new InvalidOperationException("The HTTP application is not running.");
        }

        return App.StopAsync();
    }

    /// <summary>
    /// Invoked after a WebApplicationBuilder has been created and configured. <br/> 
    /// This method can be overridden to apply additional configurations to the builder.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder instance to configure.</param>
    protected virtual void OnBuilderCreated(WebApplicationBuilder builder)
    {
        if(AddAspnetControllers)
        {
            OnMvcBuilderCreated(builder.Services.AddControllers());
        }

        if (AddAspnetApiExplorer)
        {
            builder.Services.AddEndpointsApiExplorer();
        }
    }

    /// <summary>
    /// Invoked when ASP.NET Controllers are added to the services collection. <br/>
    /// Use this method to apply additional configurations to the MVC builder.
    /// </summary>
    /// <param name="builder">The IMvcBuilder instance to configure.</param>
    protected void OnMvcBuilderCreated(IMvcBuilder builder)
    {
        if (AddGlobalJsonConverters)
        {
            builder.AddGlobalJsonConverters();
        }
    }

    /// <summary>
    /// Invoked after a WebApplication has been created and before it starts running. <br/> 
    /// This method can be overridden to configure the application's request handling pipeline.
    /// </summary>
    /// <param name="app">The WebApplication instance to configure.</param>
    protected virtual void OnApplicationCreated(WebApplication app)
    {
        if (UseExceptionHandler)
        {
            app.UseExceptionHandlerMiddleware();
        }

        if (UseAspnetControllerMapping)
        {
            app.MapControllers();
        }
    }
}
