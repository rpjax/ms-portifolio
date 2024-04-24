namespace ModularSystem.Core.TextAnalysis;

// A regular grammar is defined by:
// - A finite collection of terminal symbols Σ (alfabet).
// - A finite collection of non terminal symbols N.
// - A finite collection of productions: P
// - An initial symbol S, where S is contained in N.

/// <summary>
/// It represents a context-free-grammar (TYPE 2).
/// </summary>
public class ContextFreeGrammar
{
    public char[] IgnoredFirstChars { get; set; } = new[] { ' ' };
    public LegacyNonTerminalSymbol InitialSymbol { get; set; }
    public Production[] Productions { get; set; }

    public ContextFreeGrammar(LegacyNonTerminalSymbol initialSymbol, Production[] productions)
    {
        InitialSymbol = initialSymbol;
        Productions = productions;
    }

    public Task CompileAsync()
    {
        return Task.CompletedTask;
    }

}
