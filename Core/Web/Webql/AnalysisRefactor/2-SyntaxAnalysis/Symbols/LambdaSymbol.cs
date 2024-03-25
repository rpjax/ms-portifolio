namespace ModularSystem.Webql.Analysis.Symbols;

public class LambdaArgumentSymbol : Symbol
{
    public string Type { get; }
    public string Identifier { get; }

    public override string ToString()
    {
        return $"{Type} {Identifier}";
    }
}

public class LambdaArgumentsSymbol : Symbol
{
    public LambdaArgumentSymbol[] Arguments { get; }

    public LambdaArgumentsSymbol(LambdaArgumentSymbol[] args)
    {
        Arguments = args;
    }

    public override string ToString()
    {
        return $"{string.Join(", ", Arguments.Select(x => x.ToString()))}";
    }

}

public class LambdaSymbol : Symbol
{
    public LambdaArgumentsSymbol Arguments { get; }
    public StatementBlockSymbol Body { get; }

    public LambdaSymbol(LambdaArgumentsSymbol arguments, StatementBlockSymbol body)
    {
        Arguments = arguments;
        Body = body;
    }

    public override string ToString()
    {
        var args = Arguments.ToString();
        var body = Body.ToString();

        return $"({args}){Environment.NewLine}{body}";
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
        var args = Arguments.ToString();
        var body = Body.ToString();

        return $"({args}){Environment.NewLine}{body}";
    }
}
