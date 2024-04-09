using ModularSystem.Webql.Analysis.Components;
using System.Collections;

namespace ModularSystem.Webql.Analysis.Symbols;

public class StatementBlockSymbol : Symbol, IEnumerable<StatementSymbol>
{
    public StatementSymbol[] Statements { get; }

    public StatementBlockSymbol(StatementSymbol[] statements)
    {
        Statements = statements;
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

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteStatementBlock(this);
    }

    protected override IEnumerable<Symbol> GetChildren()
    {
        foreach (var item in Statements)
        {
            yield return item;
        }
    }
}
