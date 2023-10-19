using System.Text;

namespace ModularSystem.Core.Logging;

/// <summary>
/// Initializes the <see cref="ConsoleLogger"/> class.
/// </summary>
public class ConsoleLoggerInitializer : Initializer
{
    public ConsoleLoggerInitializer()
    {
        Priority = (int)PriorityLevel.High;
    }

    /// <summary>
    /// Initializes the console logger based on the provided options.
    /// </summary>
    /// <param name="options">Custom options for initializing the console logger.</param>
    protected internal override Task InternalInitAsync(Options options)
    {
        if (!options.InitConsoleLogger || !ConsoleLogger.ConsoleIsAvailable())
        {
            return Task.CompletedTask;
        }

        ConsoleLogger.SetAsCurrentStdIo();

        if (options.EnableInitializationLogs)
        {
            ConsoleLogger.Info("Console logger has been initialized.");
        }

        return Task.CompletedTask;
    }
}

public class ConsoleLogger
{
    public static bool IsActive { get; private set; } = false;
    public static TextReader? ConsoleReader { get; set; } = null;
    public static TextWriter? ConsoleWriter { get; set; } = null;

    private static TextReader? Reader { get; set; } = null;
    private static TextWriter? Writer { get; set; } = null;
    private static object SyncMutex { get; } = new object();

    public static void SetAsCurrentStdIo()
    {
        lock (SyncMutex)
        {
            if (!ConsoleIsAvailable())
            {
                throw new NotSupportedException("The console interface is not available in this running environment. Use other interface to perform I/O with the user.");
            }

            IsActive = true;
            ConsoleReader = Console.In;
            ConsoleWriter = Console.Out;

            Reader = new ConsoleLoggerTextInput();
            Writer = new ConsoleLoggerTextOutput();

            Console.SetIn(Reader);
            Console.SetOut(Writer);
        }
    }

    public static void UnsetAsCurrentStdIo()
    {
        lock (SyncMutex)
        {
            if (!IsActive)
            {
                throw new InvalidOperationException();
            }
            if (ConsoleReader == null)
            {
                throw new InvalidOperationException();
            }
            if (ConsoleWriter == null)
            {
                throw new InvalidOperationException();
            }

            IsActive = false;

            Console.SetIn(ConsoleReader);
            Console.SetOut(ConsoleWriter);

            Reader?.Dispose();
            Writer?.Dispose();
        }
    }

    public static bool ConsoleIsAvailable()
    {
        try
        {
            var stdIn = Console.OpenStandardInput();
            var stdOut = Console.OpenStandardOutput();

            _ = Console.CursorTop;
            _ = Console.CursorSize;
            _ = Console.CursorLeft;

            return
                Environment.UserInteractive &&
                stdIn != Stream.Null &&
                stdOut != Stream.Null;
        }
        catch
        {
            return false;
        }
    }

    public static void Log(ConsoleEntry log)
    {
        if (!IsActive)
        {
            Console.WriteLine(log);
            return;
        }

        if (ConsoleReader == null)
        {
            throw new InvalidOperationException();
        }
        if (ConsoleWriter == null)
        {
            throw new InvalidOperationException();
        }

        ClearLine();

        if (log.Prefix != null)
        {
            ConsoleWriter.Write("[");

            ConsoleColor initialForegroundColor = Console.ForegroundColor;
            ConsoleColor initialBackgroundColor = Console.BackgroundColor;

            if (log.PrefixColor != null)
            {
                Console.ForegroundColor = log.PrefixColor.Value;
            }
            if (log.PrefixBackgroundColor != null)
            {
                Console.BackgroundColor = log.PrefixBackgroundColor.Value;
            }

            ConsoleWriter.Write(log.PrefixToString());

            Console.ForegroundColor = initialForegroundColor;
            Console.BackgroundColor = initialBackgroundColor;

            ConsoleWriter.Write("]:");
        }

        if (log.Message != null)
        {
            ConsoleColor initialForegroundColor = Console.ForegroundColor;
            ConsoleColor initialBackgroundColor = Console.BackgroundColor;

            if (log.MessageColor != null)
            {
                Console.ForegroundColor = log.MessageColor.Value;
            }
            if (log.MessageBackgroundColor != null)
            {
                Console.BackgroundColor = log.MessageBackgroundColor.Value;
            }

            ConsoleWriter.Write(log.MessageToString());

            Console.ForegroundColor = initialForegroundColor;
            Console.BackgroundColor = initialBackgroundColor;
        }

        if (log.Suffix != null)
        {
            ConsoleColor initialForegroundColor = Console.ForegroundColor;
            ConsoleColor initialBackgroundColor = Console.BackgroundColor;

            if (log.SuffixColor != null)
            {
                Console.ForegroundColor = log.SuffixColor.Value;
            }
            if (log.SuffixBackgroundColor != null)
            {
                Console.BackgroundColor = log.SuffixBackgroundColor.Value;
            }

            ConsoleWriter.Write(log.SuffixToString());

            Console.ForegroundColor = initialForegroundColor;
            Console.BackgroundColor = initialBackgroundColor;
        }

        ConsoleWriter.Write(Environment.NewLine);
    }

    public static void Info(string message)
    {
        Log(new InfoLog(message));
    }

    public static void Warn(string message)
    {
        Log(new WarnLog(message));
    }

    public static void Error(string message)
    {
        Log(new ErrorLog(message));
    }

    static void ClearLine()
    {
        if (!ConsoleIsAvailable())
        {
            return;
        }

        int currentLineCursor = Console.CursorTop;
        Console.SetCursorPosition(0, Console.CursorTop);

        if (IsActive)
        {
            if (ConsoleWriter == null)
            {
                throw new InvalidOperationException();
            }

            ConsoleWriter.Write(new string(' ', Console.WindowWidth));
        }
        else
        {
            Console.Write(new string(' ', Console.WindowWidth));
        }

        Console.SetCursorPosition(0, currentLineCursor);
    }
}

public class ConsoleLoggerTextInput : TextReader
{
    public override string? ReadLine()
    {
        if (ConsoleLogger.ConsoleReader == null)
        {
            throw new InvalidOperationException();
        }

        return ConsoleLogger.ConsoleReader.ReadLine();
    }

    public override Task<string?> ReadLineAsync()
    {
        if (ConsoleLogger.ConsoleReader == null)
        {
            throw new InvalidOperationException();
        }

        return ConsoleLogger.ConsoleReader.ReadLineAsync();
    }
}

public class ConsoleLoggerTextOutput : TextWriter
{
    public override Encoding Encoding => Encoding.UTF8;

    public override void Write(string? value)
    {
        ConsoleLogger.Log(new Log(value ?? string.Empty));
    }

    public override void WriteLine()
    {
        ConsoleLogger.Log(new Log(string.Empty));
    }

    public override void WriteLine(string? value)
    {
        ConsoleLogger.Log(new Log(value ?? string.Empty));
    }
}

public class Log : ConsoleEntry
{
    public const string PREFIX = "LOG";

    public Log(string message)
    {
        Prefix = PREFIX;
        Message = message;

        PrefixColor = ConsoleColor.DarkMagenta;
        PrefixBackgroundColor = ConsoleColor.Black;

        MessageColor = ConsoleColor.DarkMagenta;
        MessageBackgroundColor = ConsoleColor.Black;
    }
}

public class InfoLog : ConsoleEntry
{
    public const string PREFIX = "INFO";

    public InfoLog(string message)
    {
        Prefix = PREFIX;
        Message = message;

        PrefixColor = ConsoleColor.DarkGreen;
        PrefixBackgroundColor = ConsoleColor.Black;

        MessageColor = ConsoleColor.DarkGreen;
        MessageBackgroundColor = ConsoleColor.Black;
    }
}

public class ErrorLog : ConsoleEntry
{
    public const string PREFIX = "ERROR";

    public ErrorLog(string message)
    {
        Prefix = PREFIX;
        Message = message;

        PrefixColor = ConsoleColor.DarkRed;
        PrefixBackgroundColor = ConsoleColor.Black;

        MessageColor = ConsoleColor.DarkRed;
        MessageBackgroundColor = ConsoleColor.Black;
    }
}

public class WarnLog : ConsoleEntry
{
    public const string PREFIX = "WARN";

    public WarnLog(string message)
    {
        Prefix = PREFIX;
        Message = message;

        PrefixColor = ConsoleColor.DarkYellow;
        PrefixBackgroundColor = ConsoleColor.Black;

        MessageColor = ConsoleColor.DarkYellow;
        MessageBackgroundColor = ConsoleColor.Black;
    }
}