namespace ModularSystem.Webql.Analysis.Symbols;

public class LambdaSymbol : Symbol
{
    public LambdaArgumentsSymbol Arguments { get; }
    public ObjectSymbol Body { get; }

    public LambdaSymbol(LambdaArgumentsSymbol arguments, ObjectSymbol body)
    {
        Arguments = arguments;
        Body = body;
    }

    public override string ToString()
    {
        return $"{Arguments} => {Body}";
    }

}

public class LambdaArgumentsSymbol : Symbol
{
    public string[] Arguments { get; }

    public LambdaArgumentsSymbol(string[] args)
    {
        Arguments = args;
    }

    public override string ToString()
    {
        return $"({string.Join(", ", Arguments)})";
    }
}

public class ProjectionLambdaSymbol : Symbol
{
    public LambdaArgumentsSymbol Arguments { get; }
    public ProjectionObjectSymbol Body { get; }

    public ProjectionLambdaSymbol(LambdaArgumentsSymbol arguments, ProjectionObjectSymbol body)
    {
        Arguments = arguments;
        Body = body;
    }

    public override string ToString()
    {
        throw new NotImplementedException();
    }
}
