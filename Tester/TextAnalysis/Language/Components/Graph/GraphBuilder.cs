using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Graph;

public enum ChildPosition
{
    Left,
    Middle,
    Right
}

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
            return new GraphNode(
                symbol: symbol,
                production: symbolProduction
            );
        }

        var recursiveNode = stack
            .Where(x => x.Symbol == symbol)
            .FirstOrDefault();

        if (recursiveNode is not null)
        {
            var terminalFound = false;

            if (symbolProduction is null)
            {
                throw new InvalidOperationException("The symbol production is null.");
            }

            foreach (var item in symbolProduction.Body)
            {
                if (item == symbol)
                {
                    break;
                }
                if (item.IsTerminal)
                {
                    terminalFound = true;
                    break;
                }
            }

            if (!terminalFound)
            {
                foreach (var node in stack)
                {
                    var nodeSymbol = node.Symbol;

                    if (node.Production is null)
                    {
                        continue;
                    }

                    foreach (var item in node.Production.Body)
                    {
                        if (item == nodeSymbol)
                        {
                            break;
                        }
                        if (item.IsTerminal)
                        {
                            terminalFound = true;
                            break;
                        }
                    }
                }
            }

            var recursionType = terminalFound 
                ? RecursionType.Normal
                : RecursionType.IndirectLeft;

            if(!terminalFound && stack.Peek() == recursiveNode)
            {
                recursionType = RecursionType.Left;
            }

            return new GraphNode(
                symbol: symbol,
                production: symbolProduction,
                recursionType: recursionType,
                children: recursiveNode.Children
            );
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
                var count = production.Body.Length;

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
