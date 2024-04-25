using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Transformations;

public class LeftFactorization : ProductionSetComplexTransformation
{
    public LeftFactorization(ProductionSet set) : base(set)
    {
    }

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
    protected override ProductionSetOperation[] GetOperations(ProductionSet set)
    {
        if (set.Length == 0)
        {
            throw new InvalidOperationException("The production set is empty.");
        }

        var head = set.First().Head;
        var alpha = set.First().Body.First();

        foreach (var production in set)
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

        var betasEnumerable = set
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

        var builder = new ProductionSetTransformationBuilder()
            .RemoveProductions(set)
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

        return builder.GetOperations();
    }

    protected override string GetExplanation(IReadOnlyList<ProductionSetOperation> operations)
    {
        var removeOperations = operations
            .Where(x => x.Type == ProductionSetOperationType.RemoveProduction)
            .ToArray();

        var addOperations = operations
            .Where(x => x.Type == ProductionSetOperationType.AddProduction)
            .ToArray();

        var removedProductions = removeOperations
            .Select(x => x.AsRemoveProduction().Production)
            .ToArray();

        var addedProductions = addOperations
            .Select(x => x.AsAddProduction().Production)
            .ToArray();

        var removedProductionsStr = string
            .Join(", ", removedProductions.Select(x => $"({x})"));

        var addedProductionsStr = string
            .Join(", ", addedProductions.Select(x => $"({x})"));

        var commonPrefix = removedProductions[0].Body[0];
        var newNonTerminal = addedProductions.Last().Head;

        var explanation = @$"The 'Common Prefix Factorization' transformation was applied to productions that start with {commonPrefix}. These productions were: [{removedProductionsStr}]. A new non-terminal symbol, {newNonTerminal}, was introduced. This symbol now encapsulates the sequences that follow {commonPrefix} in these productions. As a result, the productions were reorganized into: [{addedProductionsStr}].";

        return explanation;
    }

}
