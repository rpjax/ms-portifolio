namespace ModularSystem.Webql.Analysis.Symbols;

public class ProjectionLambdaSymbol : Symbol
{
    public LambdaArgumentSymbol[] Arguments { get; }
    public ProjectionObjectSymbol Body { get; }

    public ProjectionLambdaSymbol(LambdaArgumentSymbol[] arguments, ProjectionObjectSymbol body)
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
