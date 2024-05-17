using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Parsing.LR1.Tools;
using System.Text;

namespace ModularSystem.Core.TextAnalysis.Parsing.LR1.Components;

public class LR1ParsingTableEntry
{
    public int Id { get; }
    internal Dictionary<string, LR1Action> ActionTable { get; }

    public LR1ParsingTableEntry(
        int id,
        Dictionary<Symbol, LR1Action> actionTable)
    {
        Id = id;
        ActionTable = actionTable
            .ToDictionary(x => CreateKey(x.Key), x => x.Value);
    }

    private static string CreateKey(Symbol symbol)
    {
        return LR1ParsingTable.CreateActionKey(symbol, LR1ParsingTable.KeyStrategy.TypeAndValue);
    }

    public override string ToString()
    {
        var actionStrBuilder = new StringBuilder();
        var mainStrBuilder = new StringBuilder();

        foreach (var action in ActionTable)
        {
            actionStrBuilder.AppendLine($"On ({action.Key}) {action.Value.ToString()};");
        }

        mainStrBuilder.AppendLine($"State: {Id}");
        mainStrBuilder.AppendLine(actionStrBuilder.ToString());

        return mainStrBuilder.ToString();
    }

    public LR1Action? Lookup(Symbol symbol)
    {
        var key1 = LR1ParsingTable.CreateActionKey(symbol, LR1ParsingTable.KeyStrategy.TypeAndValue);
        var key2 = LR1ParsingTable.CreateActionKey(symbol, LR1ParsingTable.KeyStrategy.Type);

        if (ActionTable.TryGetValue(key1, out var action1))
        {
            return action1;
        }

        if (ActionTable.TryGetValue(key2, out var action2))
        {
            return action2;
        }

        return null;
    }

}

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
        if (symbol.IsNonTerminal)
        {
            return symbol.ToString();
        }

        if (symbol is not Terminal terminal)
        {
            throw new InvalidOperationException("The symbol is not a terminal.");
        }

        if (terminal.Value is null)
        {
            return terminal.ToString();
        }

        switch (strategy)
        {
            case KeyStrategy.Type:
                return terminal.TokenType.ToString();

            case KeyStrategy.TypeAndValue:
                return terminal.ToString();

            default:
                throw new ArgumentOutOfRangeException(nameof(strategy));
        }
    }

    public static LR1ParsingTable Create(ProductionSet set)
    {
        return new LR1ParsingTableFactory(set).Create();
    }

    public LR1Action? Lookup(int state, Symbol symbol)
    {
        if(state < 0 || state >= Entries.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(state));
        }

        return Entries[state].Lookup(symbol: symbol);
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
