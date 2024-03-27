namespace ModularSystem.Webql.Analysis.Symbols;

public class ProjectionLambdaSymbol : Symbol
{
    public DeclarationStatementSymbol[] Parameters { get; }
    public ProjectionObjectSymbol Body { get; }

    public ProjectionLambdaSymbol(DeclarationStatementSymbol[] parameters, ProjectionObjectSymbol body)
    {
        Parameters = parameters;
        Body = body;
    }

    public override string ToString()
    {
        var @params = Parameters.ToString();
        var body = Body.ToString();

        return $"({@params}){Environment.NewLine}{body}";
    }
}
