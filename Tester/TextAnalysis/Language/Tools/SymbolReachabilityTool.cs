﻿using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Language.Graph;

namespace ModularSystem.Core.TextAnalysis.Language.Tools;

public class SymbolReachabilityTool : GraphVisitor
{
    private List<Symbol> ReachableSymbols { get; } = new();

    public Symbol[] Execute(LL1GraphNode node)
    {
        Visit(node);
        return ReachableSymbols.ToArray();
    }

    public Symbol[] Execute(ProductionSet set)
    {
        return Execute(LL1GraphBuilder.CreateGraphTree(set));
    }

    protected override LL1GraphNode Visit(LL1GraphNode node)
    {
        if (!ReachableSymbols.Contains(node.Symbol))
        {
            ReachableSymbols.Add(node.Symbol);
        }

        return base.Visit(node);
    }
}
