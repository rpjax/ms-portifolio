using System.Collections;

namespace ModularSystem.Webql.Analysis.Symbols;

public class StatementBlockSymbol : ArgumentSymbol, IEnumerable<StatementSymbol>
{
    public StatementSymbol[] Statements { get; }

    public StatementBlockSymbol(StatementSymbol[] expressions)
    {
        Statements = expressions;
    }

    public IEnumerator<StatementSymbol> GetEnumerator()
    {
        return Statements.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Statements.GetEnumerator();
    }

    public override string ToString()
    {
        var statements = string.Join(Environment.NewLine, Statements.Select(x => $"{x};"));

        return $"{{ {statements} }}";
    }
}
