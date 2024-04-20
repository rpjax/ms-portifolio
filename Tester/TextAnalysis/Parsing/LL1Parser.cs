using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Parsing;

public class LL1Parser
{

}

public class ParsingBuffer
{

}

public class ParsingStack
{
    private Stack<ProductionSymbol> Symbols { get; } = new();

    public void Push(ProductionSymbol symbol)
    {
        Symbols.Push(symbol);
    }

    public ProductionSymbol Pop()
    {
        return Symbols.Pop();
    }

    public ProductionSymbol? Peek()
    {
        if (Symbols.Count == 0)
        {
            return null;
        }

        return Symbols.Peek();
    }
}

public class ParsingTable
{

}
