using ModularSystem.Core.TextAnalysis.Language.Transformations;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

public static partial class ProductionSetTransformationsExtensions
{
    /*
        Main API methods.
    */
    public static void AutoClean(this ProductionSet set)
    {
        set.ExpandMacros();
        set.RemoveUnreachableProductions();
        set.ExpandUnitProductions();
        set.RemoveDuplicates();
    }

    public static void RecursiveAutoClean(this ProductionSet set)
    {
        var counter = 0;

        while (true)
        {
            set.ExpandMacros();
            set.RemoveUnreachableProductions();
            set.ExpandUnitProductions();
            set.RemoveDuplicates();

            var transformationsCreated = counter - set.Transformations.Count;

            if (transformationsCreated == 0)
            {
                break;
            }

            counter = set.Transformations.Count;
        }
    }

    public static void AutoFix(this ProductionSet set)
    {
        set.RemoveLeftRecursion();
        set.FactorCommonPrefixProductions();

        var errors = set.GetErrors();

        if (errors.Length > 0)
        {
            throw new ErrorException(errors);
        }
    }

    public static void RecursiveAutoFix(this ProductionSet set)
    {
        var counter = 0;

        while (true)
        {
            set.RemoveLeftRecursion();
            set.FactorCommonPrefixProductions();

            var transformationsCreated = counter - set.Transformations.Count;

            if (transformationsCreated == 0)
            {
                break;
            }

            counter = set.Transformations.Count;
        }
    }

    public static void AutoTransform(this ProductionSet set)
    {
        var counter = 0;

        while (true)
        {
            set.RecursiveAutoClean();
            set.RecursiveAutoFix();

            var transformationsCreated = counter - set.Transformations.Count;

            if (transformationsCreated == 0)
            {
                break;
            }

            counter = set.Transformations.Count;
        }
    }

    public static void ExpandMacros(this ProductionSet set)
    {
        new MacroExpansion()
            .ExecuteTransformations(set);
    }

    public static void RemoveLeftRecursion(this ProductionSet set)
    {
        new LeftRecursionRemoval()
            .ExecuteTransformations(set);
    }

    public static void RemoveDuplicates(this ProductionSet set)
    {
        new DuplicateProductionsRemoval()
            .ExecuteTransformations(set);
    }

    public static void RemoveUnreachableProductions(this ProductionSet set)
    {
        new RemoveUnreachableProductions()
            .ExecuteTransformations(set);
    }

    public static void ExpandUnitProductions(this ProductionSet set)
    {
        new UnitProductionExpansion()
            .ExecuteTransformations(set);
    }

    public static void FactorCommonPrefixProductions(this ProductionSet set)
    {
        new CommonPrefixLeftFactorization()
            .ExecuteTransformations(set);
    }

    /*
        development.
    */

    // public static int LeftFactor(this ProductionSet set)
    // {
    //     set.EnsureNoMacros();

    //     int counter = 0;

    //     foreach (var nonTerminal in set.GetNonTerminals().ToArray())
    //     {
    //         var productions = set.Lookup(nonTerminal)
    //             .ToArray();

    //         if (productions.All(x => !x.IsLeftRecursive()))
    //         {
    //             continue;
    //         }

    //         var recursiveProductions = productions
    //             .Where(productions => productions.IsLeftRecursive())
    //             .ToArray();

    //         var nonRecursiveProductions = productions
    //             .Where(productions => !productions.IsLeftRecursive())
    //             .ToArray();

    //         var alphas = recursiveProductions
    //             .Select(x => x.Body.Skip(1).ToArray())
    //             .ToArray();

    //         var betas = nonRecursiveProductions
    //             .Select(x => x.Body.ToArray())
    //             .ToArray();

    //         set.Remove(productions);

    //         var newNonTerminal = new NonTerminal(nonTerminal.Name + "′");

    //         foreach (var beta in betas)
    //         {
    //             var body = new List<Symbol>()
    //                 .FluentAdd(beta)
    //                 .FluentAdd(newNonTerminal)
    //                 .ToArray();

    //             /*
    //              * if:
    //              * A -> Aa | ε
    //              * 
    //              * then:
    //              * A -> A'
    //              * 
    //              * instead of:
    //              * A -> εA'
    //              * 
    //              * It removes β from the production if it's value is ε
    //              */
    //             if (body.Length == 2 && body[0] is Epsilon)
    //             {
    //                 body = new Symbol[] { newNonTerminal };
    //             }

    //             set.Add(new ProductionRule(nonTerminal, body));
    //         }

    //         foreach (var alpha in alphas)
    //         {
    //             var body = new List<Symbol>()
    //                 .FluentAdd(alpha)
    //                 .FluentAdd(newNonTerminal)
    //                 .ToArray();

    //             set.Add(new ProductionRule(newNonTerminal, body));
    //         }

    //         set.Add(new ProductionRule(newNonTerminal, new Symbol[] { new Epsilon() });
    //         counter++;
    //     }

    //     return counter;
    // }

    // public static void RemoveDirectLeftRecursion(this ProductionSet set)
    // {
    //     while (set.Productions.Any(x => x.IsLeftRecursive()))
    //     {
    //         set.LeftFactor();
    //     }
    // }

}
