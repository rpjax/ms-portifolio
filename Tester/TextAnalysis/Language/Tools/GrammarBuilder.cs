using ModularSystem.Core.Helpers;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

public class GrammarBuilder
{
    private IReadOnlyCollection<ProductionRule> Productions { get; set; }
    private NonTerminal? Start { get; set; }

    public GrammarBuilder(GrammarDefinition? grammar = null)
    {
        Productions = new List<ProductionRule>();
        Start = grammar?.Start;

        if (grammar is not null)
        {
            foreach (var production in grammar.Productions)
            {
                AddProduction(production.Head, production.Body);
            }
        }
    }

    public void SetStart(NonTerminal start)
    {
        if (!GetNonTerminals().Contains(start))
        {
            throw new ArgumentException("The start symbol must belong to the non-terminal (N) tuple of the grammar.");
        }

        Start = start;
    }

    public void RemoveProductions(params ProductionRule[] productions)
    {
        Productions = Productions
            .RemoveWhere(x => productions.Any(y => y.Head.Name == x.Head.Name))
            .ToList();
    }

    public void AddProduction(NonTerminal nonTerminal, params ProductionSymbol[] body)
    {
        var list = (List<ProductionRule>)Productions;

        list.Add(new ProductionRule(nonTerminal, body));
    }

    public IEnumerable<ProductionRule> LookUpProductions(NonTerminal nonTerminal)
    {
        return Productions
            .Where(x => x.Head.Name == nonTerminal.Name);
    }

    public void ValidateProductions()
    {
        var errors = GetErrors()
            .ToArray();

        if (errors.Any())
        {
            throw new ErrorException(errors);
        }
    }

    public IEnumerable<Error> GetErrors()
    {
        yield break;
    }

    public void RemoveDirectLeftRecursion()
    {
        while (Productions.Any(x => x.IsLeftRecursive()))
        {
            LeftFactorProductions();
        }
    }

    public void RemoveIndirectLeftRecursion()
    {
        var grammar = Build();

        var path = LocalStorage.GetFileInfo("OUTPUT.txt", true).FullName;
        using var writer = new StreamWriter(path);

        writer.WriteLine("ChatGPT directed notes: consider that \"e\" without quotes means epsilon, and with quotes denotes a terminal, all terminals are represented as strings with double quotes.");
        writer.WriteLine("Original grammar:");
        writer.WriteLine(grammar);
        writer.WriteLine();

        RemoveDirectLeftRecursion();
        grammar = Build();

        writer.WriteLine("Grammar after removing direct left recursion:");
        writer.WriteLine(grammar);
        writer.WriteLine();

        var cicles = RecursionTool.GetLeftRecursionCicles(grammar)
            .ToArray();

        var recursiveNonTerminals = cicles
            .Select(x => x.Derivations.First().OriginalSentence.First().AsNonTerminal())
            .Distinct()
            .ToArray();

        var counter = 1;

        writer.WriteLine("Indirect recursive cicles the algorithm identified:");

        foreach (var cicle in cicles)
        {
            writer.WriteLine($"Cicle {counter}:");
            writer.WriteLine(cicle);
            writer.WriteLine();
            counter++;
        }

        Console.WriteLine("Recursive non-terminals:");

        foreach (var nonTerminal in recursiveNonTerminals)
        {
            writer.WriteLine($"{nonTerminal} is left recursive.");
        }

        writer.Flush();
        Console.WriteLine("done!");
    }

    public void LeftFactorProductions()
    {
        foreach (var nonTerminal in GetNonTerminals().ToArray())
        {
            var productions = LookUpProductions(nonTerminal)
                .ToArray();

            if (productions.All(x => !x.IsLeftRecursive()))
            {
                continue;
            }

            var recursiveProductions = productions
                .Where(productions => productions.IsLeftRecursive())
                .ToArray();

            var alphas = productions
                .Where(productions => productions.IsLeftRecursive())
                .Select(x => x.Body.Skip(1).ToArray())
                .ToArray();

            var betas = productions
                .Where(productions => !productions.IsLeftRecursive())
                .Select(x => x.Body.ToArray())
                .ToArray();

            RemoveProductions(productions);

            var newNonTerminal = new NonTerminal(nonTerminal.Name + "′");

            foreach (var beta in betas)
            {
                var body = new List<ProductionSymbol>()
                    .FluentAdd(beta)
                    .FluentAdd(newNonTerminal)
                    .ToArray();

                /*
                 * if:
                 * A -> Aa | ε
                 * 
                 * then:
                 * A -> A'
                 * 
                 * instead of:
                 * A -> εA'
                 * 
                 * It removes β from the production if it's value is ε
                 */
                if (body.Length == 2 && body[0] is Epsilon)
                {
                    body = new ProductionSymbol[] { newNonTerminal };
                }

                AddProduction(nonTerminal, body);
            }

            foreach (var alpha in alphas)
            {
                var body = new List<ProductionSymbol>()
                    .FluentAdd(alpha)
                    .FluentAdd(newNonTerminal)
                    .ToArray();

                AddProduction(newNonTerminal, body);
            }

            AddProduction(newNonTerminal, new Epsilon());
        }
    
    }

    public void RemoveCommonLeftPrefix()
    {

    }

    public void Simplify()
    {

    }

    public GrammarDefinition Build()
    {
        return new GrammarDefinition(
            productions: Productions.ToArray(),
            start: Start ?? throw new InvalidOperationException("The start symbol must be set before building the grammar.")
        );
    }

    public IEnumerable<NonTerminal> GetNonTerminals()
    {
        return Productions
            .Select(x => x.Head)
            .Distinct();
    }

    public IEnumerable<Terminal> GetTerminals()
    {
        return Productions
            .SelectMany(x => x.Body)
            .OfType<Terminal>()
            .Distinct();
    }

}

public static class NonTerminalExtensions
{

}
