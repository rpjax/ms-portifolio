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
