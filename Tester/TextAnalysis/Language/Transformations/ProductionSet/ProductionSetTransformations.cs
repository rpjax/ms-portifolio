using System.Text;
using System.Text.Json.Serialization;
using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Transformations;

public enum ProductionSetOperationType
{
    AddProduction,
    RemoveProduction,
    ReplaceSymbol,
}

public abstract class ProductionSetOperation
{
    public abstract ProductionSetOperationType Type { get; }
    public string? Explanation { get; set; }

    public abstract override string ToString();
    public abstract ProductionSet Apply(ProductionSet set);
    public abstract ProductionSet Reverse(ProductionSet set);

    public AddProductionOperation AsAddProduction()
    {
        if (this is not AddProductionOperation addProduction)
        {
            throw new InvalidOperationException("The operation is not an AddProductionOperation");
        }

        return addProduction;
    }

    public RemoveProductionOperation AsRemoveProduction()
    {
        if (this is not RemoveProductionOperation removeProduction)
        {
            throw new InvalidOperationException("The operation is not a RemoveProductionOperation");
        }

        return removeProduction;
    }

    public ReplaceSymbolOperation AsReplaceSymbol()
    {
        if (this is not ReplaceSymbolOperation replaceSymbol)
        {
            throw new InvalidOperationException("The operation is not a ReplaceSymbolOperation");
        }

        return replaceSymbol;
    }
}

public class AddProductionOperation : ProductionSetOperation
{
    public override ProductionSetOperationType Type => ProductionSetOperationType.AddProduction;
    public ProductionRule Production { get; }

    public AddProductionOperation(ProductionRule production)
    {
        Production = production;
    }

    public override ProductionSet Apply(ProductionSet set)
    {
        set.Add(Production);
        return set;
    }

    public override ProductionSet Reverse(ProductionSet set)
    {
        set.Remove(Production);
        return set;
    }

    public override string ToString()
    {
        if (!string.IsNullOrWhiteSpace(Explanation))
        {
            return Explanation;
        }

        return $"+ {Production}";
    }
}

public class RemoveProductionOperation : ProductionSetOperation
{
    public override ProductionSetOperationType Type => ProductionSetOperationType.RemoveProduction;
    public ProductionRule Production { get; }

    public RemoveProductionOperation(ProductionRule production)
    {
        Production = production;
    }

    public override ProductionSet Apply(ProductionSet set)
    {
        set.Remove(Production);
        return set;
    }

    public override ProductionSet Reverse(ProductionSet set)
    {
        set.Add(Production);
        return set;
    }

    public override string ToString()
    {
        if (!string.IsNullOrWhiteSpace(Explanation))
        {
            return Explanation;
        }

        return $"- {Production}";
    }
}

public class ReplaceSymbolOperation : ProductionSetOperation
{
    public override ProductionSetOperationType Type => ProductionSetOperationType.ReplaceSymbol;
    public Symbol OldSymbol { get; }
    public Symbol NewSymbol { get; }

    public ReplaceSymbolOperation(Symbol oldSymbol, Symbol newSymbol)
    {
        OldSymbol = oldSymbol;
        NewSymbol = newSymbol;
    }

    public override ProductionSet Apply(ProductionSet set)
    {
        set.Replace(OldSymbol, NewSymbol);
        return set;
    }

    public override ProductionSet Reverse(ProductionSet set)
    {
        set.Replace(NewSymbol, OldSymbol);
        return set;
    }

    public override string ToString()
    {
        if (!string.IsNullOrWhiteSpace(Explanation))
        {
            return Explanation;
        }

        return $"Replace {OldSymbol} with {NewSymbol}";
    }
}

/*
    separator.
*/

public class ProductionSetTransformation
{
    public ProductionSetOperation[] Operations { get; private set; }

    [JsonConstructor]
    public ProductionSetTransformation(params ProductionSetOperation[] operations)
    {
        Operations = operations;
    }

    public ProductionSet Apply(ProductionSet set)
    {
        foreach (var operation in Operations)
        {
            set = operation.Apply(set);
        }

        return set;
    }

    public ProductionSet Reverse(ProductionSet set)
    {
        foreach (var operation in Operations.Reverse())
        {
            set = operation.Reverse(set);
        }

        return set;
    }

    public override string ToString()
    {
        return GetExplanation();
    }

    public virtual string GetExplanation()
    {
        var builder = new StringBuilder();

        foreach (var operation in Operations)
        {
            builder.AppendLine(operation.ToString());
        }

        return builder.ToString();
    }

    public ProductionSetTransformation AddProduction(ProductionRule production)
    {
        Operations = Operations
            .Append(new AddProductionOperation(production))
            .ToArray();
        ;
        return this;
    }

    public ProductionSetTransformation RemoveProduction(ProductionRule production)
    {
        Operations = Operations
            .Append(new RemoveProductionOperation(production))
            .ToArray();
        ;
        return this;
    }

    public ProductionSetTransformation ReplaceSymbol(Symbol oldSymbol, Symbol newSymbol)
    {
        Operations = Operations
            .Append(new ReplaceSymbolOperation(oldSymbol, newSymbol))
            .ToArray();
        ;
        return this;
    }

    internal ProductionSetTransformation AddOperation(params ProductionSetOperation[] operations)
    {
        Operations = Operations
            .Concat(operations)
            .ToArray();
        ;
        return this;
    }

    internal ProductionSetTransformation AddOperations(IEnumerable<ProductionSetOperation> operations)
    {
        Operations = Operations
            .Concat(operations)
            .ToArray();
        ;
        return this;
    }
}

public class ProductionSetTransformationBuilder
{
    private List<ProductionSetOperation> Operations { get; }

    public ProductionSetTransformationBuilder()
    {
        Operations = new();
    }

    public IReadOnlyList<ProductionSetOperation> GetOperations()
    {
        return Operations;
    }

    public ProductionSetTransformationBuilder AddProductions(params ProductionRule[] productions)
    {
        foreach (var production in productions)
        {
            Operations.Add(new AddProductionOperation(production));
        }

        return this;
    }

    public ProductionSetTransformationBuilder RemoveProductions(params ProductionRule[] productions)
    {
        foreach (var production in productions)
        {
            Operations.Add(new RemoveProductionOperation(production));
        }

        return this;
    }

    public ProductionSetTransformationBuilder ReplaceSymbol(Symbol oldSymbol, Symbol newSymbol)
    {
        Operations.Add(new ReplaceSymbolOperation(oldSymbol, newSymbol));
        return this;
    }

    public ProductionSetTransformation Build()
    {
        return new ProductionSetTransformation(Operations.ToArray());
    }
}

public class SentenceTransformation
{

}
