using System.Diagnostics;

namespace ModularSystem.Core.Cli.Commands;

public class ClearCmd : CliCommand
{
    public ClearCmd(CLI cli, PromptContext context) : base(cli, context)
    {
    }

    public override string Instruction()
    {
        return "clear";
    }

    public override string Description()
    {
        return "Clears the screen.";
    }

    protected override void Execute()
    {
        CliReference.Clear();
    }
}

public class ExitCmd : CliCommand
{
    public ExitCmd(CLI cli, PromptContext context) : base(cli, context)
    {
    }

    public override string Instruction()
    {
        return "exit";
    }

    public override string Description()
    {
        return "Exits the CLI.";
    }

    protected override void Execute()
    {
        CliReference.Stop();
    }
}

public class HelpCmd : CliCommand
{
    public HelpCmd(CLI cli, PromptContext context) : base(cli, context)
    {
    }

    public override string Instruction()
    {
        return "help";
    }

    public override string Description()
    {
        return "Lists all available instructions with a description.";
    }

    protected override void Execute()
    {
        var commands = CliReference.Commands;

        CliReference.Print($"CLI v{CLI.Version}");

        foreach (var cmd in commands.OrderBy(x => x))
        {
            CliReference.Print($"{cmd}");
        }
    }
}

public class KillCmd : CliCommand
{
    public KillCmd(CLI cli, PromptContext context) : base(cli, context)
    {
    }

    public override string Instruction()
    {
        return "kill";
    }

    public override string Description()
    {
        return "Terminates all threads and exits the process execution. Use it with caution.";
    }

    protected override void Execute()
    {
        Process.GetCurrentProcess().Kill();
    }
}

//public class PrintArgsCmd : CliCommand
//{
//    public PrintArgsCmd(CLI cli, PromptContext context) : base(cli, context)
//    {
//    }

//    public override string Instruction()
//    {
//        return "print_args";
//    }

//    public override string Description()
//    {
//        return "Prints all arguments passed.";
//    }  

//    protected override void Execute()
//    {
//        foreach (var arg in Context.Arguments)
//        {
//            CliReference.Print($"{arg.Key}: {arg.Value}");
//        }
//    }
//}

//public class PrintFlagsCmd : CliCommand
//{
//    public PrintFlagsCmd(CLI cli, PromptContext context) : base(cli, context)
//    {
//    }

//    public override string Instruction()
//    {
//        return "print_flags";
//    }

//    public override string Description()
//    {
//        return "Prints all flags passed.";
//    }    

//    protected override void Execute()
//    {
//        foreach (var flag in Context.Flags)
//        {
//            CliReference.Print($"{flag}");
//        }
//    }

//}

public class ListRunningContextsCmd : CliCommand
{
    public ListRunningContextsCmd(CLI cli, PromptContext context) : base(cli, context)
    {
    }

    public override string Instruction()
    {
        return "lst";
    }

    public override string Description()
    {
        return "Lists all the commands that are executing in the background, in a separeted thread.";
    }

    protected override void Execute()
    {
        CliReference.PrintRunningCommands();
    }
}