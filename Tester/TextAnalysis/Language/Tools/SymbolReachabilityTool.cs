using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Language.Graph;

namespace ModularSystem.Core.TextAnalysis.Language.Tools;

public class SymbolReachabilityTool : GraphVisitor
{
    private List<Symbol> ReachableSymbols { get; } = new();

    public Symbol[] Execute(GraphNode node)
    {
        Visit(node);
        return ReachableSymbols.ToArray();
    }

    public Symbol[] Execute(ProductionSet set)
    {
        return Execute(GraphBuilder.CreateDerivationTree(set));
    }

    protected override GraphNode Visit(GraphNode node)
    {
        if (!ReachableSymbols.Contains(node.Symbol))
        {
            ReachableSymbols.Add(node.Symbol);
        }

        return base.Visit(node);
    }
}
