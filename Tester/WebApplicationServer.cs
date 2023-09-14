using ModularSystem.Core.Cli.Commands;
using ModularSystem.Core.Cli;
using ModularSystem.Web;
using ModularSystem.Web.Authentication;

namespace ModularSystem.Tester;

public class WebApplicationServer
{
    public static WebApplicationServer? Singleton { get; set; } = null;

    string[] args;
    Func<Task>? stopFunction;
    Thread worker;

    public WebApplicationServer(string[] args)
    {
        this.args = args;
        stopFunction = null;
        worker = new Thread(ThreadFunction);
    }

    public static void StartSingleton()
    {
        StopSingleton();
        Singleton = new WebApplicationServer(new string[0]);
        Singleton.Start();
    }

    public static void StopSingleton()
    {
        Singleton?.Stop();
    }

    public void Start()
    {
        worker.Name = "HTTP Server Worker";
        worker.Priority = ThreadPriority.Highest;
        worker.Start();
        Console.WriteLine("Tester HTTP server started.");
    }

    public void Stop()
    {
        stopFunction?.Invoke().Wait();
        Console.WriteLine("Tester HTTP server terminated.");
    }

    void ThreadFunction()
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddControllers()
            .AddGlobalJsonConverters();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors(configs =>
        {
            configs.AllowAnyHeader();
            configs.AllowAnyMethod();
            configs.AllowAnyOrigin();
        });

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.UseIamSystem(new AesIamSystem());

        stopFunction = () =>
        {
            return app.StopAsync();
        };

        app.Run();
    }
}

public class StartWebAppCmd : CliCommand
{
    public override void Execute(CLI cli, PromptContext context)
    {
        WebApplicationServer.StartSingleton();
    }

    public override string Instruction()
    {
        return "start_http_server";
    }

    public override string Description()
    {
        return "Starts the tester HTTP server.";
    }
}

public class StopWebAppCmd : CliCommand
{
    public override void Execute(CLI cli, PromptContext context)
    {
        WebApplicationServer.StopSingleton();
    }

    public override string Instruction()
    {
        return "stop_http_server";
    }

    public override string Description()
    {
        return "Shuts down the tester HTTP server.";
    }
}