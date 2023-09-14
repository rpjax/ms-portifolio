using ModularSystem.Core.Cli;
using ModularSystem.Core.Cli.Commands;

namespace ModularSystem.Tester;

public class DemoJeferton : CliCommand
{
    public override void Execute(CLI cli, PromptContext context)
    {

    }

    public override string Instruction()
    {
        return "demo";
    }
}
