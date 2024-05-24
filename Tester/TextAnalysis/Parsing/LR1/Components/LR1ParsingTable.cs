using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Parsing.LR1.Tools;

namespace ModularSystem.Core.TextAnalysis.Parsing.LR1.Components;

public class LR1ParsingTable
{
    private LR1ParsingTableEntry[] Entries { get; }
    private ProductionRule[] Productions { get; }

    public LR1ParsingTable(
        LR1ParsingTableEntry[] entries,
        ProductionRule[] productions)
    {
        Entries = entries;
        Productions = productions;

        for (int i = 0; i < Entries.Length; i++)
        {
            var entry = Entries[i];

            if (entry.Id != i)
            {
                throw new InvalidOperationException("The entry id is not sequential.");
            }

            foreach (var action in entry.ActionTable.Values)
            {
                if (action is LR1ShiftAction shiftAction)
                {
                    if (shiftAction.NextState < 0 || shiftAction.NextState >= Entries.Length)
                    {
                        throw new InvalidOperationException("The shift action references an invalid state.");
                    }
                }
                if (action is LR1ReduceAction reduceAction)
                {
                    if (reduceAction.ProductionIndex < 0 || reduceAction.ProductionIndex >= Productions.Length)
                    {
                        throw new InvalidOperationException("The reduce action references an invalid production.");
                    }
                }
            }
        }

    }

    public enum KeyStrategy
    {
        Type,
        TypeAndValue
    }

    public static string CreateActionKey(Symbol symbol, KeyStrategy strategy)
    {
        if (symbol is NonTerminal || symbol is Epsilon)
        {
            return symbol.ToString();
        }

        if (symbol is not Terminal terminal)
        {
            throw new InvalidOperationException("The symbol is not a terminal.");
        }

        if(terminal.Value is null)
        {
            return terminal.Type.ToString();
        }

        switch (strategy)
        {
            case KeyStrategy.Type:
                return terminal.Type.ToString();

            case KeyStrategy.TypeAndValue:
                return $"{terminal.Type}({terminal.Value})";

            default:
                throw new ArgumentOutOfRangeException(nameof(strategy));
        }
    }

    public static LR1ParsingTable Create(Grammar grammar)
    {
        return new LR1ParsingTableFactory()
            .Create(grammar);
    }

    public LR1Action? Lookup(int state, Symbol symbol)
    {
        if(state < 0 || state >= Entries.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(state));
        }

        return Entries[state].Lookup(symbol: symbol);
    }

    public LR1ShiftAction? LookupShift(int state, Symbol symbol)
    {
        var action = Lookup(state, symbol);

        if (action is LR1ShiftAction shiftAction)
        {
            return shiftAction;
        }

        return null;
    }

    public LR1ReduceAction? LookupReduce(int state, Symbol symbol)
    {
        var action = Lookup(state, symbol);

        if (action is LR1ReduceAction reduceAction)
        {
            return reduceAction;
        }

        return null;
    }

    public LR1GotoAction? LookupGoto(int state, Symbol symbol)
    {
        var action = Lookup(state, symbol);

        if (action is LR1GotoAction gotoAction)
        {
            return gotoAction;
        }

        return null;
    }

    public ProductionRule GetProduction(int index)
    {
        if(index < 0 || index >= Productions.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        return Productions[index];
    }

}
