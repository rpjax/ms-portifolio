using System.Text;

namespace ModularSystem.Core.Cli;

/// <summary>
/// Represents a base execution context for command-line interface operations.
/// </summary>
public abstract class ExecutionContext
{
    /// <summary>
    /// Gets the context in which the command prompt is executed.
    /// </summary>
    public PromptContext PromptContext => Context;

    /// <summary>
    /// Gets a reference to the command-line interface that initiated this execution context.
    /// </summary>
    protected CLI CliReference { get; }

    /// <summary>
    /// Gets the context in which the command is executed.
    /// </summary>
    protected PromptContext Context { get; }

    /// <summary>
    /// Represents the worker thread used for executing the command.
    /// </summary>
    protected Thread Worker { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExecutionContext"/> class.
    /// </summary>
    /// <param name="cli">The CLI reference.</param>
    /// <param name="context">The context of the command prompt.</param>
    public ExecutionContext(CLI cli, PromptContext context)
    {
        CliReference = cli;
        Context = context;
        Worker = new Thread(ThreadFunction);
    }

    /// <summary>
    /// Starts the execution of the command in its own thread.
    /// </summary>
    public void Start()
    {
        Worker.Start();
    }

    /// <summary>
    /// Represents the method containing the logic to execute the command.
    /// </summary>
    protected abstract void Execute();

    /// <summary>
    /// Disposes the execution context and releases associated resources.
    /// </summary>
    private void Dispose()
    {
        CliReference.DisposeExecutionContext(this);
    }

    /// <summary>
    /// Represents the thread function where the command execution and related operations are performed.
    /// </summary>
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
            Dispose();
        }
    }

    /// <summary>
    /// Handles exceptions that occur during the execution of the command.
    /// </summary>
    /// <param name="e">The exception that occurred.</param>
    protected virtual void OnException(Exception e)
    {
        var strBuilder = new StringBuilder();

        strBuilder.Append($"Execution of '{Context.Instruction}' threw an exception.");
        strBuilder.Append($"\tMessage: \"{e.Message}\".");

        if (Context.GetFlag("show_stack"))
        {
            CliReference.Print($"\tStack-trace: '{e.StackTrace}'.\n");
        }        
    }

    /// <summary>
    /// Provides a hook for derived classes to implement additional logic that should run when the command execution is complete.
    /// </summary>
    protected virtual void OnExit()
    {

    }
}

/// <summary>
/// Represents an execution context for lambda-based commands in the command-line interface.
/// </summary>
public class LambdaExecutionContext : ExecutionContext
{
    /// <summary>
    /// Gets the lambda action to be executed.
    /// </summary>
    protected Action Lambda { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LambdaExecutionContext"/> class.
    /// </summary>
    /// <param name="cli">The CLI reference.</param>
    /// <param name="context">The context of the command prompt.</param>
    /// <param name="lambda">The lambda action to be executed.</param>
    public LambdaExecutionContext(CLI cli, PromptContext context, Action lambda) : base(cli, context)
    {
        Lambda = lambda;
    }

    /// <summary>
    /// Executes the provided lambda action.
    /// </summary>
    protected override void Execute()
    {
        Lambda.Invoke();
    }
}
