namespace ModularSystem.Core.Cli;

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
