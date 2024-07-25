namespace ModularSystem.Core.Cli;

public abstract class CliCommand : ExecutionContext
{
    public CliCommand(CLI cli, PromptContext context) : base(cli, context)
    {
    }

    public abstract string Instruction();

    public virtual string Description()
    {
        return string.Empty;
    }

    public virtual List<KeyValuePair<string, string>> Flags()
    {
        return new();
    }

    public virtual List<KeyValuePair<string, string>> Arguments()
    {
        return new();
    }
}
