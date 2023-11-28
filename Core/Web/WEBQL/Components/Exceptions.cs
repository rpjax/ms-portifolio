using ModularSystem.Webql.Analysis;

namespace ModularSystem.Webql;

public abstract class ParseException : Exception
{
    public ParseException(string message, Exception? inner = null) : base(message, inner)
    {

    }

    public abstract string GetMessage();

}

public class SyntaxException : ParseException
{
    private SyntaxContext Context { get; }

    public SyntaxException(string message, SyntaxContext context, Exception? inner = null) : base(message, inner)
    {
        Context = context;
    }

    public override string GetMessage()
    {
        var dot = "";
        var stack = Context.Stack;

        if (!Message.EndsWith('.'))
        {
            dot = ".";
        }

        return $"Syntax Error: {Message}{dot} This error was identified at: {stack}";
    }
}

public class SemanticException : ParseException
{
    private SemanticContext Context { get; }

    public SemanticException(string message, SemanticContext context, Exception? inner = null) : base(message, inner)
    {
        Context = context;
    }

    public override string GetMessage()
    {
        var dot = "";

        if (!Message.EndsWith('.'))
        {
            dot = ".";
        }

        return $"Semantic Error: {Message}{dot} This error was identified at: {Context.Stack}";
    }
}

public class GeneratorException : ParseException
{
    public GeneratorContext Context { get; }

    public GeneratorException(string message, GeneratorContext context, Exception? inner = null) : base(message, inner)
    {
        Context = context;
    }

    public override string GetMessage()
    {
        var dot = "";

        if (!Message.EndsWith('.'))
        {
            dot = ".";
        }

        return $"Translation Error: {Message}{dot} This error was identified at: {Context.Stack}";
    }

}
