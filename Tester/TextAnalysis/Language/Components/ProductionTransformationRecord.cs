using System.Text;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

public enum TransformationType
{
    Removal,
    Replacement
}

public enum TransformationReason
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
    public TransformationReason Reason { get; }

    public ProductionTransformationRecord(
        ProductionRule originalProduction,
        ProductionSet? replacements,
        TransformationReason reason)
    {
        OriginalProduction = originalProduction;
        Replacements = replacements ?? new ProductionSet();
        Reason = reason;
    }

    public TransformationType Type
    {
        get
        {
            if (Replacements.Length == 0)
            {
                return TransformationType.Removal;
            }

            return TransformationType.Replacement;
        }
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        switch (Reason)
        {
            case TransformationReason.MacroExpansion:
                builder.Append("Macro expansion: ");
                break;
                
            case TransformationReason.LeftRecursionExpansion:
                builder.Append("Left recursion expansion: ");
                break;

            case TransformationReason.DuplicateProductionRemoval:
                builder.Append("Duplicate removal: ");
                break;

            case TransformationReason.UnreachableSymbolRemoval:
                builder.Append("Unreachable removal: ");
                break;

            case TransformationReason.LeftFactorization:
                builder.Append("Left factorization: ");
                break;

            case TransformationReason.UnitProductionExpansion:
                builder.Append("Unit production expansion: ");
                break;

            case TransformationReason.CommonPrefixFactorization:
                builder.Append("Common prefix factorization: ");
                break;
        }

        builder.Append($"({OriginalProduction}) ");

        switch (Type)
        {
            case TransformationType.Removal:
                builder.Append("removed");
                break;
            case TransformationType.Replacement:
                builder.Append("replaced by: ");
                builder.Append("[");
                builder.Append(string.Join(", ", Replacements.Select(x => $"({x})")));
                builder.Append("]");
                break;
        }

        return builder.ToString();
    }
}
