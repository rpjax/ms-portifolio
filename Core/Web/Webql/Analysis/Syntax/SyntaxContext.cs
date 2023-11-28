namespace ModularSystem.Webql.Analysis;

public class SyntaxContext
{
    /// <summary>
    /// Gets the stack trace of the context for debugging and tracing purposes.
    /// </summary>
    public string Stack { get; }

    /// <summary>
    /// Initializes a new instance of the SyntaxContext class.
    /// </summary>
    /// <param name="stack">The stack trace for the context.</param>
    public SyntaxContext(string? stack = "$")
    {
        Stack = stack ?? "$";
    }

    /// <summary>
    /// Creates a sub-context based on a specified sub-stack trace.
    /// </summary>
    /// <param name="subStack">The sub-stack trace for the new context.</param>
    /// <returns>A new SyntaxContext instance representing the sub-context.</returns>
    public SyntaxContext CreateSubContext(string subStack)
    {
        return new SyntaxContext(Stack + subStack);
    }
}
