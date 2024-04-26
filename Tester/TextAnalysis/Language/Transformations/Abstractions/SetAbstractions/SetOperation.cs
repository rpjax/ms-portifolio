using System.Diagnostics.CodeAnalysis;
using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Language.Transformations;

public enum SetOperationType
{
    AddProduction,
    RemoveProduction,
    ReplaceSymbol,
}

public abstract class SetOperation : IEquatable<SetOperation>, IEqualityComparer<SetOperation>
{
    public abstract SetOperationType Type { get; }
    public string? Explanation { get; set; }

    public static bool operator ==(SetOperation left, SetOperation right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SetOperation left, SetOperation right)
    {
        return !left.Equals(right);
    }

    public abstract bool Equals(SetOperation? other);
    public abstract bool Equals(SetOperation? x, SetOperation? y);
    public abstract override bool Equals(object? obj);
    public abstract int GetHashCode([DisallowNull] SetOperation obj);
    public abstract override int GetHashCode();
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

public class AddProductionOperation : SetOperation
{
    public override SetOperationType Type => SetOperationType.AddProduction;
    public ProductionRule Production { get; }

    public AddProductionOperation(ProductionRule production)
    {
        Production = production;
    }

    public override bool Equals(SetOperation? other)
    {
        return other is AddProductionOperation addProduction
            && addProduction.Production == Production;
    }

    public override bool Equals(SetOperation? x, SetOperation? y)
    {
        if (x is not AddProductionOperation addProductionX || y is not AddProductionOperation addProductionY)
        {
            return false;
        }

        return addProductionX.Production == addProductionY.Production;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as SetOperation);
    }

    public override int GetHashCode([DisallowNull] SetOperation obj)
    {
        return obj.GetHashCode();
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            hash = (hash * 16777619) ^ Production.GetHashCode();
            return hash;
        }
    }

    public override string ToString()
    {
        if (!string.IsNullOrWhiteSpace(Explanation))
        {
            return Explanation;
        }

        return $"+ {Production}";
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
}

public class RemoveProductionOperation : SetOperation
{
    public override SetOperationType Type => SetOperationType.RemoveProduction;
    public ProductionRule Production { get; }

    public RemoveProductionOperation(ProductionRule production)
    {
        Production = production;
    }

    public override bool Equals(SetOperation? other)
    {
        return other is RemoveProductionOperation removeProduction
            && removeProduction.Production == Production;
    }

    public override bool Equals(SetOperation? x, SetOperation? y)
    {
        if (x is not RemoveProductionOperation removeProductionX || y is not RemoveProductionOperation removeProductionY)
        {
            return false;
        }

        return removeProductionX.Production == removeProductionY.Production;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as SetOperation);
    }

    public override int GetHashCode([DisallowNull] SetOperation obj)
    {
        return obj.GetHashCode();
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            hash = (hash * 16777619) ^ Production.GetHashCode();
            return hash;
        }
    }

    public override string ToString()
    {
        if (!string.IsNullOrWhiteSpace(Explanation))
        {
            return Explanation;
        }

        return $"- {Production}";
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

}

public class ReplaceSymbolOperation : SetOperation
{
    public override SetOperationType Type => SetOperationType.ReplaceSymbol;
    public Symbol OldSymbol { get; }
    public Symbol NewSymbol { get; }

    public ReplaceSymbolOperation(Symbol oldSymbol, Symbol newSymbol)
    {
        OldSymbol = oldSymbol;
        NewSymbol = newSymbol;
    }

    public override bool Equals(SetOperation? other)
    {
        return other is ReplaceSymbolOperation replaceSymbol
            && replaceSymbol.OldSymbol == OldSymbol
            && replaceSymbol.NewSymbol == NewSymbol;
    }

    public override bool Equals(SetOperation? x, SetOperation? y)
    {
        if (x is not ReplaceSymbolOperation replaceSymbolX || y is not ReplaceSymbolOperation replaceSymbolY)
        {
            return false;
        }

        return replaceSymbolX.OldSymbol == replaceSymbolY.OldSymbol
            && replaceSymbolX.NewSymbol == replaceSymbolY.NewSymbol;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as SetOperation);
    }

    public override int GetHashCode([DisallowNull] SetOperation obj)
    {
        return obj.GetHashCode();
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            hash = (hash * 16777619) ^ OldSymbol.GetHashCode();
            hash = (hash * 16777619) ^ NewSymbol.GetHashCode();
            return hash;
        }
    }

    public override string ToString()
    {
        if (!string.IsNullOrWhiteSpace(Explanation))
        {
            return Explanation;
        }

        return $"Replace {OldSymbol} with {NewSymbol}";
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
}
