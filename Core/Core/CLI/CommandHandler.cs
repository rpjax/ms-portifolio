namespace ModularSystem.Core.Cli;

public interface ICommandHandler
{
    bool IsHandlerTo(PromptContext node);
    string GetInstruction();
    string GetDescription();
    void Execute(PromptContext node);
}

public abstract class CommandHandler : ICommandHandler
{
    protected CLI cli;

    public CommandHandler(CLI cli)
    {
        this.cli = cli;
    }

    public abstract string GetInstruction();
    public abstract void Execute(PromptContext context);

    public virtual bool IsHandlerTo(PromptContext context)
    {
        return context.Instruction == GetInstruction();
    }

    public virtual string GetDescription()
    {
        return string.Empty;
    }

    public virtual void OnExit()
    {

    }
}