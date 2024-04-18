namespace ModularSystem.Webql.Analysis.Parsing;

public class ParsingException : Exception
{
    private ParsingContext Context { get; }

    public ParsingException(string message, ParsingContext context) : base(message)
    {
        Context = context;
    }
}
