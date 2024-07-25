using ModularSystem.Core.Cli.Commands;
using ModularSystem.Core.Logging;
using System.Text;

namespace ModularSystem.Core.Cli;

/// <summary>
/// A simple command line interface (work in progress)
/// </summary>
public class CLI : IDisposable
{
    public const string Version = "0.3.0";
    public const string DefaultInputPrefixText = "<: ";
    public const string DefaultOutputPrefixText = "=> ";

    public const string ArgumentIdentifier = "-";
    public const string FlagIdentifier = "--";

    public string InputPrefix { get; private set; } = "$:";
    public string OutputPrefix { get; private set; } = "> ";
    public TimeSpan SleepTime { get; set; } = TimeSpan.FromMilliseconds(1);
    public IEnumerable<string> Commands => FactoryDictionary.Keys;

    static int InstanceCounter { get; set; } = 0;

    bool IsRunning { get; set; }
    bool ShouldRestoreConsoleLoggerAsStdIo { get; set; }

    List<string> InputQueue { get; set; } = new();
    List<string> PrintQueue { get; set; } = new();

    Dictionary<string, ExecutionContextFactory> FactoryDictionary { get; } = new();
    List<ExecutionContext> ExecutionContexts { get; set; } = new();

    TextReader? ConsoleIn { get; set; } = null;
    TextReader? CliIn { get; set; } = null;

    TextWriter? ConsoleOut { get; set; } = null;
    TextWriter? CliOut { get; set; } = null;

    Thread? InputWorker { get; set; } = null;

    public CLI()
    {
        if (InstanceCounter > 0)
        {
            throw new InvalidOperationException("Multiple CLI instances were created simoutaniously.");
        }
    }

    public void Dispose()
    {
        IsRunning = false;
    }

    public static void StartInstance()
    {
        new CLI().MapCommands().Start();
    }

    public static bool ConsoleIsAvailable()
    {
        return ConsoleLogger.ConsoleIsAvailable();
    }

    public void DisposeExecutionContext(ExecutionContext executionContext)
    {
        lock (ExecutionContexts)
        {
            ExecutionContexts.Remove(executionContext);
        }
    }

    public CLI MapCommands()
    {
        CreateFactories();
        return this;
    }

    public void Start()
    {
        if (ConsoleLogger.IsActive)
        {
            ConsoleLogger.UnsetAsCurrentStdIo();
            ShouldRestoreConsoleLoggerAsStdIo = true;
        }

        IsRunning = true;
        InstanceCounter++;

        CliIn = new CliTextInput();
        CliOut = new CliTextOutput(this);

        ConsoleIn = Console.In;
        ConsoleOut = Console.Out;

        Console.SetIn(CliIn);
        Console.SetOut(CliOut);

        InternalPrint($"CLI started v{Version}");
        InternalPrint($"I/O indicators: '{InputPrefix}' indicates input, '{OutputPrefix}' indicates output.");
        InternalPrint("Type 'help' to see all available commands...");

        InputWorker = new Thread(InputWorkerThreadFunction);
        InputWorker.Start();

        while (IsRunning)
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

        Console.SetIn(ConsoleIn);
        Console.SetOut(ConsoleOut);
        CliIn.Dispose();
        CliOut.Dispose();

        InstanceCounter--;
    }

    public void Stop()
    {
        if (ConsoleIn == null)
        {
            throw new InvalidOperationException();
        }
        if (ConsoleOut == null)
        {
            throw new InvalidOperationException();
        }
        IsRunning = false;

        Console.SetIn(ConsoleIn);
        Console.SetOut(ConsoleOut);

        CliIn?.Dispose();
        CliOut?.Dispose();

        if (ShouldRestoreConsoleLoggerAsStdIo)
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

        lock (PrintQueue)
        {
            PrintQueue.Add(message);
        }
    }

    public void Print(IEnumerable<string> messages)
    {
        lock (PrintQueue)
        {
            foreach (var message in messages)
            {
                PrintQueue.Add(message);
            }
        }
    }

    public void PrintRunningCommands()
    {
        if (ExecutionContexts.IsNotEmpty())
        {
            Print($"Commands running in the background:");
        }
        else
        {
            Print($"No commands running...");
        }

        foreach (var context in ExecutionContexts)
        {
            if (context.GetType() == typeof(ListRunningContextsCmd))
            {
                continue;
            }

            Print($"executing \"{context.PromptContext.Instruction}\"");
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
            ConsoleOut.Write(new string(' ', Console.WindowWidth));
        }

        Console.SetCursorPosition(0, currentLineCursor);
    }

    void CreateFactories()
    {
        var types = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(CliCommand)));

        foreach (var type in types)
        {
            var factory = new ExecutionContextFactory(type);
            var typeInstance = factory.Create(null, null).TypeCast<CliCommand>();
            var key = typeInstance.Instruction();
            var value = factory;

            if (key == null)
            {
                throw new InvalidOperationException();
            }
            if (FactoryDictionary.ContainsKey(key))
            {
                throw new Exception($"The CLI instruction \"{key}\" is already mapped.");
            }

            FactoryDictionary.Add(key, value);
        }
    }

    void InternalPrint(string message)
    {
        ClearLine();

        if (ConsoleLogger.IsActive)
        {
            ConsoleLogger.ConsoleWriter.WriteLine($"{OutputPrefix}{message}");
        }
        else
        {
            ConsoleOut.WriteLine($"{OutputPrefix}{message}");
        }
    }

    void PromptUser()
    {
        ClearLine();

        if (ConsoleLogger.IsActive)
        {
            ConsoleLogger.ConsoleWriter.Write($"{InputPrefix}");
        }
        else
        {
            ConsoleOut.Write($"{InputPrefix}");
        }
    }

    void ExecutePrompt(PromptContext promptContext)
    {
        if (promptContext.Instruction == null)
        {
            return;
        }

        if (!FactoryDictionary.TryGetValue(promptContext.Instruction, out var factory))
        {
            Print($"No handlers for the command '{promptContext.Instruction}' were found.");
            return;
        }

        var executionContext = factory.Create(this, promptContext);

        lock (ExecutionContexts)
        {
            ExecutionContexts.Add(executionContext);
        }

        executionContext.Start();

        if (promptContext.Next != null)
        {
            ExecutePrompt(promptContext.Next);
        }
    }

    void InputWorkerThreadFunction()
    {
        while (IsRunning)
        {
            if (PrintQueue.IsNotEmpty())
            {
                continue;
            }

            PromptUser();
            string? input = ConsoleIn!.ReadLine();

            if (input == null)
            {
                continue;
            }

            lock (InputQueue)
            {
                InputQueue.Add(input);
            }
        }
    }

    void InputRoutine()
    {
        try
        {
            if (InputQueue.IsEmpty())
            {
                return;
            }

            string input;

            lock (InputQueue)
            {
                input = InputQueue.Pop();
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
        if (PrintQueue.IsEmpty())
        {
            return;
        }

        while (PrintQueue.IsNotEmpty())
        {
            lock (PrintQueue)
            {
                InternalPrint(PrintQueue.Pop());
            }
        }

        PromptUser();
    }

    private class ExecutionContextFactory
    {
        private Type Type { get; }

        public ExecutionContextFactory(Type type)
        {
            Type = type;
        }

        public ExecutionContext Create(CLI? cli, PromptContext? promptContext)
        {
            var args = new object?[] { cli, promptContext };
            var instance = Activator.CreateInstance(Type, args);

            if (instance == null)
            {
                throw new InvalidOperationException($"Could not create instance of type \"{Type.FullName ?? Type.Name}\"."); ;
            }

            return instance.TypeCast<ExecutionContext>();
        }
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