namespace ModularSystem.Core.TextAnalysis.Language.Components;

public static class ProductionSetExtensions
{
    public static void Add(this ProductionSet self, params ProductionRule[] productions)
    {
        if (productions.Length == 0)
        {
            throw new InvalidOperationException("The list of productions is empty.");
        }

        if (self.Start is null)
        {
            self.Start = productions[0].Head;
        }

        foreach (var production in productions)
        {
            self.Productions.Add(production);
        }
    }

    public static void Add(this ProductionSet self, NonTerminal head, params Symbol[] body)
    {
        self.Add(new ProductionRule(head, body));
    }

    public static void Add(this ProductionSet self, string head, params Symbol[] body)
    {
        self.Add(new ProductionRule(head, body));
    }

    public static void Remove(this ProductionSet self, params ProductionRule[] productions)
    {
        self.Productions.RemoveAll(x => productions.Any(y => y.Head == x.Head && y.Body == x.Body));
    }

    public static void SetStart(this ProductionSet self, NonTerminal start)
    {
        if (!self.GetNonTerminals().Contains(start))
        {
            throw new InvalidOperationException("The start symbol must be a non-terminal.");
        }

        self.Start = start;
    }


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

    public static void Replace(this ProductionSet self, Symbol symbol, Sentence replacement)
    {
        foreach (var production in self.Productions)
        {
            if (!production.Body.Contains(symbol))
            {
                continue;
            }

            var head = production.Head;
            var newBody = production.Body
                .Replace(symbol, replacement);
                ;

            self.Remove(production);
            self.Add(head, newBody);
        }
    }

}
