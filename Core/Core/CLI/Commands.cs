using System.Diagnostics;

namespace ModularSystem.Core.Cli.Commands;

public abstract class CliCommand
{
    public abstract void Execute(CLI cli, PromptContext context);

    public abstract string Instruction();

    public virtual string Description()
    {
        return string.Empty;
    }

    public virtual List<KeyValuePair<string, string>> Flags()
    {
        return new List<KeyValuePair<string, string>>();
    }

    public virtual List<KeyValuePair<string, string>> Arguments()
    {
        return new List<KeyValuePair<string, string>>();
    }

    public virtual bool IsHandlerTo(PromptContext context)
    {
        if (context.Instruction == null)
        {
            return false;
        }

        var instruction = context.Instruction.ToLower().Trim();

        foreach (var item in ParseInstruction(Instruction()))
        {
            if (instruction == item)
            {
                return true;
            }
        }
        return false;
    }

    public ExecutionContext CreateExecutionContext(CLI cli, PromptContext context)
    {
        return new LambdaExecutionContext(cli, context, () => Execute(cli, context));
    }

    string[] ParseInstruction(string instruction)
    {
        var split = instruction.Split(',');

        if (split.IsEmpty())
        {
            return new string[] { instruction };
        }

        return split.ToList().ConvertAll(x =>
        {
            return x.ToLower().Trim();
        })
            .ToArray();
    }
}

public class ClearCmd : CliCommand
{
    public override void Execute(CLI cli, PromptContext context)
    {
        cli.Clear();
    }

    public override string Instruction()
    {
        return "clear";
    }

    public override string Description()
    {
        return "Clears the screen.";
    }

    public override bool IsHandlerTo(PromptContext node)
    {
        return node.Instruction == "clear" || node.Instruction == "cls";
    }
}

public class ExitCmd : CliCommand
{
    public override void Execute(CLI cli, PromptContext context)
    {
        cli.Stop();
    }

    public override string Instruction()
    {
        return "exit";
    }

    public override string Description()
    {
        return "Exits the CLI.";
    }

    public override bool IsHandlerTo(PromptContext node)
    {
        return node.Instruction == "exit";
    }
}

public class HelpCmd : CliCommand
{
    public override void Execute(CLI cli, PromptContext context)
    {
        var commands = cli.Commands;

        cli.Print($"CLI v{CLI.VERSION}");

        foreach (var cmd in commands)
        {
            cli.Print($"'{cmd?.Instruction()}': {cmd?.Description()}");
        }
    }

    public override string Instruction()
    {
        return "help";
    }

    public override string Description()
    {
        return "Lists all available instructions with a description.";
    }
}

public class KillCmd : CliCommand
{
    public override void Execute(CLI cli, PromptContext context)
    {
        Process.GetCurrentProcess().Kill();
    }

    public override string Instruction()
    {
        return "kill";
    }

    public override string Description()
    {
        return "Terminates all threads and exits the process execution. Use it with caution.";
    }
}

public class PrintArgsCmd : CliCommand
{
    public override void Execute(CLI cli, PromptContext context)
    {
        foreach (var arg in context.Arguments)
        {
            cli.Print($"{arg.Key}: {arg.Value}");
        }
    }

    public override string Instruction()
    {
        return "print_args";
    }

    public override string Description()
    {
        return "Prints all arguments passed.";
    }
}

public class PrintFlagsCmd : CliCommand
{
    public override void Execute(CLI cli, PromptContext context)
    {
        foreach (var flag in context.Flags)
        {
            cli.Print($"{flag}");
        }
    }

    public override string Instruction()
    {
        return "print_flags";
    }

    public override string Description()
    {
        return "Prints all flags passed.";
    }
}

public class ListRunningContextsCmd : CliCommand
{
    public override void Execute(CLI cli, PromptContext context)
    {
        cli.PrintRunningCommands();
    }

    public override string Instruction()
    {
        return "lst,list_threads";
    }

    public override string Description()
    {
        return "Lists all the commands that are executing in the background, in a separeted thread.";
    }
}