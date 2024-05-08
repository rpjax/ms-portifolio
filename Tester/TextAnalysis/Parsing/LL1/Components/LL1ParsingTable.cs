using ModularSystem.Core.TextAnalysis.Tokenization;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

public class LL1ParsingTableEntry
{
    public NonTerminal State { get; }
    public Terminal Lookahead { get; }
    public ProductionRule Production { get; }

    public LL1ParsingTableEntry(NonTerminal state, Terminal lookahead, ProductionRule production)
    {
        State = state;
        Lookahead = lookahead;
        Production = production;
    }

    public string CreateKey()
    {
        var strategy = Lookahead.Value != null
            ? LL1ParsingTable.KeyStrategy.TypeAndValue
            : LL1ParsingTable.KeyStrategy.TypeOnly;

        return LL1ParsingTable.CreateKey(State, Lookahead, strategy);
    }
}

public class LL1ParsingTable
{
    public enum KeyStrategy
    {
        TypeAndValue,
        TypeOnly
    }

    private Dictionary<string, ProductionRule> Entries = new();

    public LL1ParsingTable(IEnumerable<LL1ParsingTableEntry> entries)
    {
        Entries = entries
            .ToDictionary(x => x.CreateKey(), x => x.Production);
    }

    internal static string CreateKey(string state, TokenType type, string? value, KeyStrategy strategy)
    {
        switch (strategy)
        {
            case KeyStrategy.TypeAndValue:
                return $"{state} + {type}({value?.ToString()})";

            case KeyStrategy.TypeOnly:
                return $"{state} + {type}";

            default:
                throw new InvalidOperationException();
        }
    }

    internal static string CreateKey(NonTerminal state, Terminal lookahead, KeyStrategy strategy)
    {
        return CreateKey(state.Name, lookahead.TokenType, lookahead.Value, strategy);
    }

    public override string ToString()
    {
        var keys = Entries.Keys
            .Select(x => x)
            ;
        var values = Entries.Values
            .Select(x => x.ToString())
            ;

        return string.Join("\n", keys.Zip(values, (k, v) => $"[{k}] = ({v})"));
    }

    public ProductionRule? Lookup(NonTerminal state, Terminal lookahead)
    {
        if (Entries.TryGetValue(CreateKey(state, lookahead, KeyStrategy.TypeAndValue), out var production))
        {
            return production;
        }

        if (Entries.TryGetValue(CreateKey(state, lookahead, KeyStrategy.TypeOnly), out var _production))
        {
            return _production;
        }

        return null;
    }

}
