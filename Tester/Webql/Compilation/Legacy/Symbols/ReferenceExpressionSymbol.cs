using ModularSystem.Webql.Analysis.Components;

namespace ModularSystem.Webql.Analysis.Symbols;

public class ReferenceExpressionSymbol : ExpressionSymbol
{
    public override ExpressionType ExpressionType { get; } = ExpressionType.Reference;
    private string Value { get; }

    public ReferenceExpressionSymbol(string value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return GetNormalizedValue();
    }

    public string GetNormalizedValue()
    {
        return $"{Value[1..]}";
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteReferenceExpression(this);
    }

    protected override IEnumerable<Symbol> GetChildren()
    {
        yield break;
    }
}
