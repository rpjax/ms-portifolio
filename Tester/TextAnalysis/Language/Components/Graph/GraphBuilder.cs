namespace ModularSystem.Core.TextAnalysis.Language.Components;

public static class GraphBuilder
{
    public static GraphNode FromProductionSet(ProductionSet set)
    {
        if (set.Start is null)
        {
            throw new InvalidOperationException("The production set has no start symbol.");
        }

        return FromSymbol(
            set: set,
            symbol: set.Start,
            stack: new Stack<GraphNode>()
        );
    }

    private static GraphNode FromSymbol(
        ProductionSet set,
        ProductionSymbol symbol,
        Stack<GraphNode> stack,
        ProductionRule? symbolProduction = null)
    {
        if (symbol.IsMacro)
        {
            throw new InvalidOperationException($"The symbol {symbol} is a macro. It cannot be added to the graph.");
        }

        if (symbol.IsTerminal)
        {
            return new GraphNode(symbol, symbolProduction);
        }

        var recursiveNode = stack
            .Where(x => x.Symbol == symbol)
            .FirstOrDefault();

        if (recursiveNode is not null)
        {
            recursiveNode.IsRecursive = true;
            return recursiveNode;
        }

        if (symbol is NonTerminal nonTerminal)
        {
            var root = new GraphNode(nonTerminal, symbolProduction);
            var productions = set
                .Lookup(nonTerminal)
                .ToArray();

            stack.Push(root);

            foreach (var production in productions)
            {
                foreach (var bodySymbol in production.Body)
                {
                    var child = FromSymbol(
                        set: set,
                        symbol: bodySymbol,
                        stack: stack,
                        symbolProduction: production
                    );

                    child.SetParent(root);
                    root.AddChild(child);
                }
            }

            stack.Pop();

            return root;
        }

        throw new InvalidOperationException($"The symbol {symbol} is not a terminal or non-terminal symbol.");
    }

}
