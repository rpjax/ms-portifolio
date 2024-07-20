using ModularSystem.Webql.Analysis.Components;

namespace ModularSystem.Webql.Analysis.Symbols;

public class LambdaExpressionSymbol : ExpressionSymbol, IScopeSymbol
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

        return $"({args}) => {Environment.NewLine}{body}";
    }

    public override ExpressionSymbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteLambdaExpression(this);
    }

    protected override IEnumerable<Symbol> GetChildren()
    {
        foreach (var item in Parameters)
        {
            yield return item;
        }

        yield return Body;
    }

}
