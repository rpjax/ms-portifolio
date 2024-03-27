namespace ModularSystem.Webql.Analysis.Symbols;

public class LambdaSymbol : Symbol
{
    public DeclarationStatementSymbol[] Parameters { get; }
    public StatementBlockSymbol Body { get; }

    public LambdaSymbol(DeclarationStatementSymbol[] parameters, StatementBlockSymbol body)
    {
        Parameters = parameters;
        Body = body;
    }

    public override string ToString()
    {
        var args = string.Join(", ", Parameters.Select(x => x.ToString()));
        var body = Body.ToString();

        return $"({args}){Environment.NewLine}{body}";
    }

}
