using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Parsing.LR1.Tools;
using ModularSystem.Core.TextAnalysis.Tokenization;
using System.Collections;

namespace ModularSystem.Core.TextAnalysis.Parsing.LR1.Components;

/// <summary>
/// Represents a LR(1) parsing table. Contains the entries and the production rules.
/// </summary>
public class LR1ParsingTable : IEnumerable<LR1ParsingTableEntry>
{
    private LR1ParsingTableEntry[] Entries { get; }
    private ProductionRule[] Productions { get; }

    /// <summary>
    /// Creates a new instance of <see cref="LR1ParsingTable"/>.
    /// </summary>
    /// <param name="entries"></param>
    /// <param name="productions"></param>
    /// <exception cref="InvalidOperationException"></exception>
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

    public LR1ParsingTableEntry this[int index] => Entries[index];

    public enum KeyStrategy
    {
        Type,
        TypeAndValue
    }

    public static string CreateActionKey(object obj, KeyStrategy strategy)
    {
        if (obj is Token token)
        {
            return CreateActionKey(token, strategy);
        }

        if (obj is Terminal terminal)
        {
            return CreateActionKey(terminal, strategy);
        }

        if (obj is NonTerminal nonterminal)
        {
            return CreateActionKey(nonterminal);
        }

        if(obj is Epsilon)
        {
            return Epsilon.Instance.ToString();
        }

        throw new Exception();
    }

    public static string CreateActionKey(Token token, KeyStrategy strategy)
    {
        if (token.Value is null)
        {
            return token.Type.ToString();
        }

        switch (strategy)
        {
            case KeyStrategy.Type:
                return token.Type.ToString();

            case KeyStrategy.TypeAndValue:
                return $"{token.Type}({token.Value})";

            default:
                throw new ArgumentOutOfRangeException(nameof(strategy));
        }
    }

    public static string CreateActionKey(Terminal terminal, KeyStrategy strategy)
    {
        if (terminal.Value is null)
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

    public static string CreateActionKey(NonTerminal nonterminal)
    {
        return nonterminal.ToString();
    }

    public static LR1ParsingTable Create(Grammar grammar)
    {
        return new LR1ParsingTableFactory()
            .Create(grammar);
    }

    public LR1Action? Lookup(int state, Token token)
    {
        if(state < 0 || state >= Entries.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(state));
        }

        return Entries[state].Lookup(token);
    }

    public LR1GotoAction? LookupGoto(int state, NonTerminal nonTerminal)
    {
        if (state < 0 || state >= Entries.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(state));
        }

        var action = Entries[state].Lookup(nonTerminal);

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

    public IEnumerator<LR1ParsingTableEntry> GetEnumerator()
    {
        return ((IEnumerable<LR1ParsingTableEntry>)Entries).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
