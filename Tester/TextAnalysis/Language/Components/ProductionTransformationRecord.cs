using System.Text;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

public enum ProductionTransformationType
{
    Removal,
    Replacement
}

public enum ProductionTransformationReason
{
    MacroExpansion,
    LeftRecursionExpansion,
    DuplicateProductionRemoval,
    UnreachableSymbolRemoval,
    LeftFactorization,
    UnitProductionExpansion,
    CommonPrefixFactorization
}

public class ProductionTransformationRecord
{
    public ProductionRule OriginalProduction { get; }
    public ProductionSet Replacements { get; }
    public ProductionTransformationReason Reason { get; }

    public ProductionTransformationRecord(
        ProductionRule originalProduction,
        ProductionSet? replacements,
        ProductionTransformationReason reason)
    {
        OriginalProduction = originalProduction;
        Replacements = replacements ?? new ProductionSet();
        Reason = reason;
    }

    public ProductionTransformationType Type
    {
        get
        {
            if (Replacements.Length == 0)
            {
                return ProductionTransformationType.Removal;
            }

            return ProductionTransformationType.Replacement;
        }
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        switch (Reason)
        {
            case ProductionTransformationReason.MacroExpansion:
                builder.Append("Macro expansion: ");
                break;
                
            case ProductionTransformationReason.LeftRecursionExpansion:
                builder.Append("Left recursion expansion: ");
                break;

            case ProductionTransformationReason.DuplicateProductionRemoval:
                builder.Append("Duplicate removal: ");
                break;

            case ProductionTransformationReason.UnreachableSymbolRemoval:
                builder.Append("Unreachable removal: ");
                break;

            case ProductionTransformationReason.LeftFactorization:
                builder.Append("Left factorization: ");
                break;

            case ProductionTransformationReason.UnitProductionExpansion:
                builder.Append("Unit production expansion: ");
                break;

            case ProductionTransformationReason.CommonPrefixFactorization:
                builder.Append("Common prefix factorization: ");
                break;
        }

        builder.Append($"({OriginalProduction}) ");

        switch (Type)
        {
            case ProductionTransformationType.Removal:
                builder.Append("removed");
                break;
            case ProductionTransformationType.Replacement:
                builder.Append("replaced by: ");
                builder.Append("[");
                builder.Append(string.Join(", ", Replacements.Select(x => $"({x})")));
                builder.Append("]");
                break;
        }

        return builder.ToString();
    }
}
