using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public static class ExpressionAnalyser
{
    public static ExpressionSemantic Analyse(SemanticContext context, ExpressionSymbol symbol)
    {
        switch (symbol.ExpressionType)
        {
            case ExpressionType.Literal:
                return AnalyseLiteralExpression(context, (LiteralExpressionSymbol)symbol);

            case ExpressionType.Reference:
                return AnalyseReferenceExpression(context, (ReferenceExpressionSymbol)symbol);

            case ExpressionType.Operator:
                return AnalyseOperatorExpression(context, (OperatorExpressionSymbol)symbol);

            case ExpressionType.Lambda:
                return AnalyseLambdaExpression(context, (LambdaExpressionSymbol)symbol);

            case ExpressionType.AnonymousType:
                return AnalyseAnonymousTypeExpression(context, (AnonymousTypeExpressionSymbol)symbol);
        }

        throw new Exception();
    }

    public static LiteralExpressionSemantic AnalyseLiteralExpression(
        SemanticContext context,
        LiteralExpressionSymbol symbol)
    {
        return LiteralExpressionAnalyser.AnalyseLiteralExpression(context, symbol);
    }

    public static ReferenceExpressionSemantic AnalyseReferenceExpression(
        SemanticContext context,
        ReferenceExpressionSymbol symbol)
    {
        return ReferenceExpressionAnalyser.AnalyseReferenceExpression(context, symbol);
    }

    public static OperatorExpressionSemantic AnalyseOperatorExpression(
        SemanticContext context,
        OperatorExpressionSymbol symbol)
    {
        return OperatorExpressionAnalyser.AnalyseOperatorExpression(context, symbol);
    }

    public static LambdaExpressionSemantic AnalyseLambdaExpression(
        SemanticContext context,
        LambdaExpressionSymbol symbol)
    {
        return LambdaExpressionAnalyser.AnalyseLambdaExpression(context, symbol);
    }

    public static TypeProjectionExpressionSemantic AnalyseAnonymousTypeExpression(
        SemanticContext context,
        AnonymousTypeExpressionSymbol symbol)
    {
        return AnonymousTypeExpressionAnalyser.AnalyseAnonymousTypeExpression(context, symbol);
    }
}
