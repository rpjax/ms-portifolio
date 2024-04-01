using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public static class LiteralExpressionAnalyser
{
    public static LiteralExpressionSemantic AnalyseLiteralExpression(
        SemanticContext context,
        LiteralExpressionSymbol symbol)
    {
        switch (symbol.LiteralType)
        {
            case LiteralType.Null:
                return AnalyseNullLiteral(context, (NullSymbol)symbol);

            case LiteralType.String:
                return AnalyseStringLiteral(context, (StringSymbol)symbol);

            case LiteralType.Bool:
                return AnalyseBoolLiteral(context, (BoolSymbol)symbol);

            case LiteralType.Number:
                return AnalyseNumberLiteral(context, (NumberSymbol)symbol);
        }

        throw new Exception();
    }

    public static LiteralExpressionSemantic AnalyseNullLiteral(SemanticContext context, NullSymbol symbol)
    {
        return new LiteralExpressionSemantic(
            type: typeof(Nullable)
        );
    }

    public static LiteralExpressionSemantic AnalyseStringLiteral(SemanticContext context, StringSymbol symbol)
    {
        return new LiteralExpressionSemantic(
            type: typeof(string)
        );
    }

    public static LiteralExpressionSemantic AnalyseBoolLiteral(SemanticContext context, BoolSymbol symbol)
    {
        return new LiteralExpressionSemantic(
            type: typeof(bool)
        );
    }

    public static LiteralExpressionSemantic AnalyseNumberLiteral(SemanticContext context, NumberSymbol symbol)
    {
        return new LiteralExpressionSemantic(
            type: typeof(int)
        );
    }

}
