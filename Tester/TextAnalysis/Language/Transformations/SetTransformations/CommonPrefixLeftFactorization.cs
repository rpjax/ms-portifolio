using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Transformations;

/// <summary>
/// Represents a transformation that factors out common prefixes in a production subset.
/// </summary>
/// <remarks>
/// All productions in the subset must have a common head and a common prefix (alpha).
/// This transformation differs from <see cref="CommonPrefixLeftFactorizationPass"/> in that it can only operate on a subset of productions.
/// </remarks>
// public class CommonPrefixLeftFactorization : SetComplexTransformation
// {
//     public CommonPrefixLeftFactorization(ProductionSet set) : base(set)
//     {
//     }

//     protected override string GetExplanation(IReadOnlyList<SetOperation> operations)
//     {
//         var removeOperations = operations
//             .Where(x => x.Type == SetOperationType.RemoveProduction)
//             .ToArray();

//         var addOperations = operations
//             .Where(x => x.Type == SetOperationType.AddProduction)
//             .ToArray();

//         var commonPrefix = removeOperations.First().AsRemoveProduction().Production.Body.First();
//         var newNonTerminal = addOperations.Last().AsAddProduction().Production.Head;

//         var operationStr = string.Join("\n", operations);

//         var explanation = $"Left factorization on productions with alpha: {commonPrefix}. Introduction of non-terminal: {newNonTerminal}.\n{operationStr}";

//         return explanation;
//     }

//     /*  
//         A -> a A B.  
//         A -> a B c.  
//         A -> a A c.   
//         A -> a.

//         after factorization:
//         A - a A′. 
//         A′ -> A B.
//         A′ -> B c.
//         A′ -> A c.
//         A′ -> ε.
//     */
//     protected override SetOperation[] GetOperations(ProductionSet set)
//     {
//         if (set.Length == 0)
//         {
//             throw new InvalidOperationException("The production set is empty.");
//         }

//         var head = set.First().Head;
//         var alpha = set.First().Body.First();

//         foreach (var production in set)
//         {
//             if (production.Head != head)
//             {
//                 throw new InvalidOperationException("The productions do not have a common head.");
//             }
//             if (production.Body.First() != alpha)
//             {
//                 throw new InvalidOperationException("The productions do not have a common prefix.");
//             }
//         }

//         var betasEnumerable = set
//             .Select(x => x.Body.Skip(1).ToArray())
//             ;

//         var betas = betasEnumerable
//             .Where(x => x.Length > 0)
//             .Select(x => new Sentence(x))
//             .ToArray();

//         var producesEpsilon = betasEnumerable
//             .Any(x => x.Length == 1);

//         var newNonTerminal = set.CreateNonTerminalPrime(head);

//         var newNonTerminalProduction = new ProductionRule(
//             head: head,
//             body: new Sentence(alpha, newNonTerminal)
//         );

//         var builder = new SetTransformationBuilderOld()
//             .RemoveProductions(set)
//             .AddProductions(newNonTerminalProduction)
//             ;

//         foreach (var beta in betas)
//         {
//             var newProduction = new ProductionRule(
//                 head: newNonTerminal,
//                 body: beta
//             );

//             builder.AddProductions(newProduction);
//         }

//         if (producesEpsilon)
//         {
//             var epsilonProduction = new ProductionRule(
//                 head: newNonTerminal,
//                 body: new Sentence(new Epsilon())
//             );

//             builder.AddProductions(epsilonProduction);
//         }

//         return builder.GetOperations();
//     }

// }

public class CommonPrefixLeftFactorization : ISetTransformer
{
    /*  
        A -> a A B.  
        A -> a B c.  
        A -> a A c.   
        A -> a.

        after factorization:
        A - a A′. 
        A′ -> A B.
        A′ -> B c.
        A′ -> A c.
        A′ -> ε.
    */
    public SetTransformationCollection ExecuteTransformations(ProductionSet set)
    {
        set.EnsureNoMacros();
        set.ResetTransformationsTracker();

        while (true)
        {
            var group = set.GetCommonPrefixProductions();

            if (group.Length == 0)
            {
                break;
            }

            foreach (var subSet in group)
            {
                if (subSet.Length == 0)
                {
                    throw new InvalidOperationException("The production set is empty.");
                }

                var head = subSet.First().Head;
                var alpha = subSet.First().Body.First();

                foreach (var production in subSet)
                {
                    if (production.Head != head)
                    {
                        throw new InvalidOperationException("The productions do not have a common head.");
                    }
                    if (production.Body.First() != alpha)
                    {
                        throw new InvalidOperationException("The productions do not have a common prefix.");
                    }
                }

                var betasEnumerable = subSet
                    .Select(x => x.Body.Skip(1).ToArray())
                    ;

                var betas = betasEnumerable
                    .Where(x => x.Length > 0)
                    .Select(x => new Sentence(x))
                    .ToArray();

                var producesEpsilon = betasEnumerable
                    .Any(x => x.Length == 1);

                var newNonTerminal = set.CreateNonTerminalPrime(head);

                var newNonTerminalProduction = new ProductionRule(
                    head: head,
                    body: new Sentence(alpha, newNonTerminal)
                );

                var builder = set.GetTransformationBuilder("Common Prefix Left Factorization")
                    .RemoveProductions(subSet)
                    .AddProductions(newNonTerminalProduction)
                    ;

                foreach (var beta in betas)
                {
                    var newProduction = new ProductionRule(
                        head: newNonTerminal,
                        body: beta
                    );

                    builder.AddProductions(newProduction);
                }

                if (producesEpsilon)
                {
                    var epsilonProduction = new ProductionRule(
                        head: newNonTerminal,
                        body: new Sentence(new Epsilon())
                    );

                    builder.AddProductions(epsilonProduction);
                }

                builder.Build();
            }
        }

        return set.GetTrackedTransformations();
    }

}

