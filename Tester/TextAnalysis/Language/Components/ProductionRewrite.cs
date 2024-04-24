using System.Text;

namespace ModularSystem.Core.TextAnalysis.Language.Components;

public enum RewriteType
{
    Removal,
    Replacement
}

public enum RewriteReason
{
    MacroExpansion,
    LeftRecursionExpansion,
    DuplicateProductionRemoval,
    UnreachableSymbolRemoval,
    LeftFactorization,
    UnitProductionExpansion
}

public class ProductionRewrite
{
    public ProductionRule OriginalProduction { get; }
    public ProductionSet Replacements { get; }
    public RewriteReason Reason { get; }

    public ProductionRewrite(ProductionRule originalProduction, ProductionSet? replacements, RewriteReason reason)
    {
        OriginalProduction = originalProduction;
        Replacements = replacements ?? new ProductionSet();
        Reason = reason;
    }

    public RewriteType RewriteType
    {
        get
        {
            if(Replacements.Length == 0)
            {
                return RewriteType.Removal;
            }

            return RewriteType.Replacement;
        }
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        switch(Reason)
        {
            case RewriteReason.MacroExpansion:
                builder.Append("Macro expansion: ");
                break;
            case RewriteReason.LeftRecursionExpansion:
                builder.Append("Left recursion expansion: ");
                break;
            case RewriteReason.DuplicateProductionRemoval:
                builder.Append("Duplicate removal: ");
                break;
            case RewriteReason.UnreachableSymbolRemoval:
                builder.Append("Unreachable removal: ");
                break;
            case RewriteReason.LeftFactorization:
                builder.Append("Left factorization: ");
                break;
            case RewriteReason.UnitProductionExpansion:
                builder.Append("Unit production expansion: ");
                break;
        }

        builder.Append($"({OriginalProduction}) ");

        switch(RewriteType)
        {
            case RewriteType.Removal:
                builder.Append("removed");
                break;
            case RewriteType.Replacement:
                builder.Append("replaced by: ");
                builder.Append("[");
                builder.Append(string.Join(", ", Replacements.Select(x => $"({x})")));
                builder.Append("]");
                break;
        }

        return builder.ToString();
    }
}
