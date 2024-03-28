namespace ModularSystem.Webql.Analysis.Symbols;

public class LambdaExpressionSymbol : ExpressionSymbol
{
    public override ExpressionType ExpressionType { get; } = ExpressionType.Lambda;
    public DeclarationStatementSymbol[] Parameters { get; }
    public StatementBlockSymbol Body { get; }

    public LambdaExpressionSymbol(DeclarationStatementSymbol[] parameters, StatementBlockSymbol body)
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
