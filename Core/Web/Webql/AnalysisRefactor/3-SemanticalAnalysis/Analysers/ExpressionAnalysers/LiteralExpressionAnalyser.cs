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
        //*
        // 'void' because there's no value and no type information.
        //*
        return new LiteralExpressionSemantic(
            type: typeof(void)
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
        var type = typeof(int);
        var number = symbol.Value;
        var isFloat = symbol.Value.Contains('.');

        if (isFloat)
        {
            var parts = number.Split('.');
            var fractionPart = parts.Length > 1 ? parts[1] : "";

            if (fractionPart.Length > 15)
            {
                type = typeof(decimal);
            }
            else if (fractionPart.Length > 7)
            {
                type = typeof(double);
            }
            else
            {
                type = typeof(float);
            }
        }
        else
        {
            if (number.Length > 18)
            {
                type = typeof(long);
            }
            else
            {
                type = typeof(int);
            }
        }

        return new LiteralExpressionSemantic(
            type: type
        );
    }

}
