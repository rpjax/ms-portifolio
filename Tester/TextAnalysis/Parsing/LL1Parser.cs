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
    private Stack<Symbol> Symbols { get; } = new();

    public void Push(Symbol symbol)
    {
        Symbols.Push(symbol);
    }

    public Symbol Pop()
    {
        return Symbols.Pop();
    }

    public Symbol? Peek()
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
