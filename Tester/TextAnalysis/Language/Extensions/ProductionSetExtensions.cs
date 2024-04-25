namespace ModularSystem.Core.TextAnalysis.Language.Components;

public static class ProductionSetExtensions
{
    /// <summary>
    /// Creates a new non-terminal symbol with a unique name based on the given non-terminal.
    /// </summary>
    /// <param name="set">The production set.</param>
    /// <param name="nonTerminal">The original non-terminal.</param>
    /// <returns>A new non-terminal symbol with a unique name.</returns>
    public static NonTerminal CreateNonTerminalPrime(this ProductionSet set, NonTerminal nonTerminal)
    {
        var name = nonTerminal.Name + "′";

        while (set.Productions.Any(p => p.Head.Name == name))
        {
            name += "′";
        }

        return new NonTerminal(name);
    }

}

