using ModularSystem.Core.Cli.Commands;
using ModularSystem.Core.Logging;
using MongoDB.Driver;
using System.Text;

namespace ModularSystem.Core.Cli;

public abstract class ExecutionContext
{
    public PromptContext PromptContext => context;

    protected CLI cli;
    protected PromptContext context;
    protected Thread worker;

    public ExecutionContext(CLI cli, PromptContext context)
    {
        this.cli = cli;
        this.context = context;
        worker = new Thread(ThreadFunction);
    }

    public void Start()
    {
        worker.Start();
    }

    void ThreadFunction()
    {
        try
        {
            Execute();
        }
        catch (Exception e)
        {
            OnException(e);
        }
        finally
        {
            OnExit();
        }
    }

    void OnExit()
    {
        cli.DisposeExecutionContext(this);
    }

    protected abstract void Execute();
    protected virtual void OnException(Exception e)
    {
        cli.Print($"Execution of '{context.Instruction}' threw an exception: {e.Message}\n\n# stack trace: '{e.StackTrace}'.\n");
    }
}

public class LambdaExecutionContext : ExecutionContext
{
    Action lambda;

    public LambdaExecutionContext(CLI cli, PromptContext context, Action lambda) : base(cli, context)
    {
        this.lambda = lambda;
    }

    protected override void Execute()
    {
        lambda.Invoke();
    }
}

/// <summary>
/// A simple command line interface (work in progress)
/// </summary>
public class CLI : IDisposable
{
    public const string VERSION = "0.3.0";
    public const string PREFIX_TEXT_DEFAULT = ">:";
    public const string ARGUMENT_IDENTIFIER = "-";
    public const string FLAG_IDENTIFIER = "--";

    public string inputPrefix = "<: ";
    public string outputPrefix = "=> ";
    public TimeSpan SleepTime = TimeSpan.FromMilliseconds(1);
    public List<CliCommand> Commands => commands;

    static int InstanceCounter = 0;

    bool isRunning = false;

    List<string> inputQueue = new();
    List<string> printQueue = new();

    List<CliCommand> commands = new();
    List<ExecutionContext> executionContexts = new();

    TextReader? consoleIn;
    TextReader? cliIn = null;

    TextWriter? consoleOut;
    TextWriter? cliOut = null;

    Thread? inputWorker;

    bool _shouldRestoreConsoleLoggerAsStdIo = false;

    public CLI()
    {
        if (InstanceCounter > 0)
        {
            throw new InvalidOperationException("Multiple CLI instances were created simoutaniously.");
        }
    }

    public static void StartInstance()
    {
        new CLI().MapCommands().Start();
    }

    public static bool ConsoleIsAvailable()
    {
        return ConsoleLogger.ConsoleIsAvailable();
    }

    public void Dispose()
    {
        isRunning = false;
    }

    public void DisposeExecutionContext(ExecutionContext executionContext)
    {
        lock (executionContexts)
        {
            executionContexts.Remove(executionContext);
        }
    }

    public CLI MapCommands()
    {
        commands.AddRange(GetCommandsFromAssembly());
        return this;
    }

    public void Start()
    {
        if (ConsoleLogger.IsActive)
        {
            ConsoleLogger.UnsetAsCurrentStdIo();
            _shouldRestoreConsoleLoggerAsStdIo = true;
        }

        isRunning = true;
        InstanceCounter++;

        cliIn = new CliTextInput();
        cliOut = new CliTextOutput(this);

        consoleIn = Console.In;
        consoleOut = Console.Out;

        Console.SetIn(cliIn);
        Console.SetOut(cliOut);

        InternalPrint($"CLI started v{VERSION}");
        InternalPrint($"I/O indicators: '{inputPrefix}' indicates input, '{outputPrefix}' indicates output.");
        InternalPrint("Type 'help' to see all available commands...");

        inputWorker = new Thread(InputWorkerThreadFunction);
        inputWorker.Start();

        while (isRunning)
        {
            InputRoutine();
            OutputRoutine();
            Thread.Sleep(SleepTime);
        }

        ClearLine();

        if (ConsoleLogger.IsActive)
        {
            ConsoleLogger.Info("CLI exited.");
        }
        else
        {
            Console.WriteLine("CLI exited.");
        }

        Console.SetIn(consoleIn);
        Console.SetOut(consoleOut);
        cliIn.Dispose();
        cliOut.Dispose();

        InstanceCounter--;
    }

    public void Stop()
    {
        if (consoleIn == null)
        {
            throw new InvalidOperationException();
        }
        if (consoleOut == null)
        {
            throw new InvalidOperationException();
        }
        isRunning = false;

        Console.SetIn(consoleIn);
        Console.SetOut(consoleOut);

        cliIn?.Dispose();
        cliOut?.Dispose();

        if (_shouldRestoreConsoleLoggerAsStdIo)
        {
            ConsoleLogger.SetAsCurrentStdIo();
        }
    }

    public void Print(string? message = null)
    {
        if (message == null)
        {
            return;
        }

        lock (printQueue)
        {
            printQueue.Add(message);
        }
    }

    public void PrintRunningCommands()
    {
        if (executionContexts.IsNotEmpty())
        {
            Print($"Commands running in the background:");
        }
        else
        {
            Print($"No commands running...");
        }

        foreach (var context in executionContexts)
        {
            if (new ListRunningContextsCmd().IsHandlerTo(context.PromptContext))
            {
                continue;
            }

            Print($" # executing: {context.PromptContext.Instruction}");
        }
    }

    public void Clear()
    {
        Console.Clear();
        PromptUser();
    }

    public void ClearLine()
    {
        if (!ConsoleIsAvailable())
        {
            return;
        }

        int currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);

        if (ConsoleLogger.IsActive)
        {
            ConsoleLogger.ConsoleWriter.Write(new string(' ', Console.WindowWidth));
        }
        else
        {
            consoleOut.Write(new string(' ', Console.WindowWidth));
        }

        Console.SetCursorPosition(0, currentLineCursor);
    }

    void InternalPrint(string message)
    {
        ClearLine();

        if (ConsoleLogger.IsActive)
        {
            ConsoleLogger.ConsoleWriter.WriteLine($"{outputPrefix}{message}");
        }
        else
        {
            consoleOut.WriteLine($"{outputPrefix}{message}");
        }
    }

    void PromptUser()
    {
        ClearLine();

        if (ConsoleLogger.IsActive)
        {
            ConsoleLogger.ConsoleWriter.Write($"{inputPrefix}");
        }
        else
        {
            consoleOut.Write($"{inputPrefix}");
        }
    }

    List<CliCommand> GetCommandsFromAssembly()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(CliCommand)))
            .Select(type => Activator.CreateInstance(type) as CliCommand)
            .ToList()!;
    }

    void ExecutePrompt(PromptContext promptContext)
    {
        bool handlerFound = false;

        foreach (var command in commands)
        {
            if (command.IsHandlerTo(promptContext))
            {
                handlerFound = true;
                var executionContext = command.CreateExecutionContext(this, promptContext);

                lock (executionContexts)
                {
                    executionContexts.Add(executionContext);
                }

                executionContext.Start();
            }
        }

        if (!handlerFound && !string.IsNullOrEmpty(promptContext.Instruction))
        {
            Print($"No handlers for the command '{promptContext.Instruction}' were found.");
        }

        if (promptContext.Next != null)
        {
            ExecutePrompt(promptContext.Next);
        }
    }

    void InputWorkerThreadFunction()
    {
        while (isRunning)
        {
            if (printQueue.IsNotEmpty())
            {
                continue;
            }

            PromptUser();
            string? input = consoleIn.ReadLine();

            if (input == null)
            {
                continue;
            }

            lock (inputQueue)
            {
                inputQueue.Add(input);
            }
        }
    }

    void InputRoutine()
    {
        try
        {
            if (inputQueue.IsEmpty())
            {
                return;
            }
            string input;

            lock (inputQueue)
            {
                input = inputQueue.Pop();
            }

            var prompt = PromptContext.From(input);

            if (prompt != null)
            {
                ExecutePrompt(prompt);
            }
        }
        catch (Exception e)
        {
            Print($"{e.Message}");
        }
    }

    void OutputRoutine()
    {
        if (printQueue.IsEmpty())
        {
            return;
        }

        while (printQueue.IsNotEmpty())
        {
            lock (printQueue)
            {
                InternalPrint(printQueue.Pop());
            }
        }

        PromptUser();
    }
}

public class CliTextInput : TextReader
{
    public override string? ReadLine()
    {
        throw new InvalidOperationException();
    }

    public override Task<string?> ReadLineAsync()
    {
        throw new InvalidOperationException();
    }
}

public class CliTextOutput : TextWriter
{
    public override Encoding Encoding => Encoding.UTF8;
    CLI cli;

    public CliTextOutput(CLI cli)
    {
        this.cli = cli;
    }

    public override void Write(string? value)
    {
        cli.Print(value);
    }

    public override void WriteLine()
    {
        cli.Print();
    }

    public override void WriteLine(string? value)
    {
        cli.Print(value);
    }
}