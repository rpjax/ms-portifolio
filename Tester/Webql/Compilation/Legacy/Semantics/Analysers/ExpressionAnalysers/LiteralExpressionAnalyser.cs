using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analyzers;

public static class LiteralExpressionAnalyzer
{
    public static LiteralExpressionSemantic AnalyzeLiteralExpression(
        SemanticContextOld context,
        LiteralExpressionSymbol symbol)
    {
        switch (symbol.LiteralType)
        {
            case LiteralType.Null:
                return AnalyzeNullLiteral(context, (NullSymbol)symbol);

            case LiteralType.String:
                return AnalyzeStringLiteral(context, (StringSymbol)symbol);

            case LiteralType.Bool:
                return AnalyzeBoolLiteral(context, (BoolSymbol)symbol);

            case LiteralType.Number:
                return AnalyzeNumberLiteral(context, (NumberSymbol)symbol);
        }

        throw new Exception();
    }

    public static LiteralExpressionSemantic AnalyzeNullLiteral(SemanticContextOld context, NullSymbol symbol)
    {
        //*
        // 'void' because there's no value and no type information.
        //*
        return new LiteralExpressionSemantic(
            type: typeof(void)
        );
    }

    public static LiteralExpressionSemantic AnalyzeStringLiteral(SemanticContextOld context, StringSymbol symbol)
    {
        return new LiteralExpressionSemantic(
            type: typeof(string)
        );
    }

    public static LiteralExpressionSemantic AnalyzeBoolLiteral(SemanticContextOld context, BoolSymbol symbol)
    {
        return new LiteralExpressionSemantic(
            type: typeof(bool)
        );
    }

    public static LiteralExpressionSemantic AnalyzeNumberLiteral(SemanticContextOld context, NumberSymbol symbol)
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
