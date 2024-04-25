namespace ModularSystem.Core.TextAnalysis.Language.Components;

/*
    Internal API for manipulating production sets. 
    In order to save memory, the sets are not immutable, but they have to be carefully manipulated.
    Outside of the internal scope of the library, sets can be considered immutable.
*/
public static partial class ProductionSetManipulationsExtensions
{
    internal static void SetStart(this ProductionSet self, NonTerminal start)
    {
        if (!self.GetNonTerminals().Contains(start))
        {
            throw new InvalidOperationException("The start symbol must be a non-terminal.");
        }

        self.Start = start;
    }

    internal static void Add(this ProductionSet self, params ProductionRule[] productions)
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

    internal static void Add(this ProductionSet self, NonTerminal head, params Symbol[] body)
    {
        self.Add(new ProductionRule(head, body));
    }

    internal static void Add(this ProductionSet self, string head, params Symbol[] body)
    {
        self.Add(new ProductionRule(head, body));
    }

    internal static void Remove(this ProductionSet self, params ProductionRule[] productions)
    {
        self.Productions.RemoveAll(x => productions.Any(y => y.Head == x.Head && y.Body == x.Body));
    }

    internal static void Replace(this ProductionSet self, Symbol symbol, Sentence replacement)
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

    internal static void Replace(this ProductionSet self, Symbol symbol, params Symbol[] replacement)
    {
        self.Replace(symbol, new Sentence(replacement));
    }

}

