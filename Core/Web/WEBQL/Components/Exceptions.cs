using ModularSystem.Webql.Analysis;
using ModularSystem.Webql.Synthesis;

namespace ModularSystem.Webql;

/// <summary>
/// Represents a base class for exceptions related to parsing in the WebQL system.
/// </summary>
public abstract class ParseException : Exception
{
    /// <summary>
    /// Initializes a new instance of the ParseException class with a specified error message and optional inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="inner">The inner exception reference, if any, that caused this exception (optional).</param>
    public ParseException(string message, Exception? inner = null) : base(message, inner)
    {

    }

    /// <summary>
    /// Abstract method to get a formatted message for the exception.
    /// </summary>
    /// <returns>A string representing the detailed message of the exception.</returns>
    public abstract string GetMessage();
}


/// <summary>
/// Represents an exception related to syntax errors in the WebQL system.
/// </summary>
public class SyntaxException : ParseException
{
    private SyntaxContext Context { get; }

    /// <summary>
    /// Initializes a new instance of the SyntaxException class with a specified error message, syntax context, and optional inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="context">The syntax context where the error occurred.</param>
    /// <param name="inner">The inner exception reference, if any, that caused this exception (optional).</param>
    public SyntaxException(string message, SyntaxContext context, Exception? inner = null) : base(message, inner)
    {
        Context = context;
    }

    /// <summary>
    /// Provides a detailed message for the syntax error, including the context where it occurred.
    /// </summary>
    /// <returns>A string representing the detailed message of the syntax error.</returns>
    public override string GetMessage()
    {
        var dot = Message.EndsWith('.') ? "" : ".";
        return $"Syntax Error: {Message}{dot} This error was identified at: {Context.Stack}";
    }
}

/// <summary>
/// Represents an exception related to semantic errors in the WebQL system.
/// </summary>
public class SemanticException : ParseException
{
    private SemanticContextOld Context { get; }

    /// <summary>
    /// Initializes a new instance of the SemanticException class with a specified error message, semantic context, and optional inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="context">The semantic context where the error occurred.</param>
    /// <param name="inner">The inner exception reference, if any, that caused this exception (optional).</param>
    public SemanticException(string message, SemanticContextOld context, Exception? inner = null) : base(message, inner)
    {
        Context = context;
    }

    /// <summary>
    /// Provides a detailed message for the semantic error, including the context where it occurred.
    /// </summary>
    /// <returns>A string representing the detailed message of the semantic error.</returns>
    public override string GetMessage()
    {
        var dot = Message.EndsWith('.') ? "" : ".";
        return $"Semantic Error: {Message}{dot} This error was identified at: {Context.Label}";
    }
}

/// <summary>
/// Represents an exception related to translation errors in the WebQL system.
/// </summary>
public class TranslationException : ParseException
{
    private TranslationContextOld Context { get; }

    /// <summary>
    /// Initializes a new instance of the TranslationException class with a specified error message, translation context, and optional inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="context">The translation context where the error occurred.</param>
    /// <param name="inner">The inner exception reference, if any, that caused this exception (optional).</param>
    public TranslationException(string message, TranslationContextOld context, Exception? inner = null)
        : base(message, inner)
    {
        Context = context;
    }

    /// <summary>
    /// Provides a detailed message for the translation error, including the context where it occurred.
    /// </summary>
    /// <returns>A string representing the detailed message of the translation error.</returns>
    public override string GetMessage()
    {
        var dot = Message.EndsWith('.') ? "" : ".";
        return $"Translation Error: {Message}{dot} This error was identified at: {Context.Label}";
    }
}

