using ModularSystem.Webql.Analysis;
using System.Text.Json.Nodes;

namespace ModularSystem.Webql;

public abstract class ParseException : Exception
{
    public ParseException(string message) : base(message)
    {

    }

    public abstract string GetMessage();
}

public class SyntaxException : ParseException
{
    private JsonNode Stack { get; }

    public SyntaxException(string message, JsonNode stack) : base(message)
    {
        Stack = stack;
    }

    public override string GetMessage()
    {
        var dot = "";

        if (!Message.EndsWith('.'))
        {
            dot = ".";
        }

        return $"Syntax Error: {Message}{dot} This error was identified at: {Stack.GetPath()}";
    }
}

public class SemanticException : ParseException
{
    private SemanticContext Context { get; }

    public SemanticException(string message, SemanticContext context) : base(message)
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
    public Node? Stack { get; }

    public GeneratorException(string message, Node? stack) : base(message)
    {
        Stack = stack;
    }

    public override string GetMessage()
    {
        var dot = "";

        if (!Message.EndsWith('.'))
        {
            dot = ".";
        }

        return $"Translation Error: {Message}{dot}";
    }

}
