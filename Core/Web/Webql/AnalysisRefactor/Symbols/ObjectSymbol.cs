using System.Collections;

namespace ModularSystem.Webql.Analysis.Symbols;

public class ObjectSymbol : ArgumentSymbol, IEnumerable<ExprSymbol>
{
    public ExprSymbol[] Expressions { get; }

    public ObjectSymbol(ExprSymbol[] expressions)
    {
        Expressions = expressions;
    }

    public IEnumerator<ExprSymbol> GetEnumerator()
    {
        return Expressions.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Expressions.GetEnumerator();
    }

    public override string ToString()
    {
        var exprs = string.Join(Environment.NewLine, Expressions.Select(x => x.ToString() + ";"));

        return $"{{ {exprs} }}";
    }
}
