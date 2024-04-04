using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

/// <summary>
/// Provides functionality to analyse expressions.
/// </summary>
public static class ExpressionAnalyser
{
    /// <summary>
    /// Analyses an expression and returns its semantic representation.
    /// </summary>
    /// <param name="context">The semantic context in which the analysis is performed.</param>
    /// <param name="symbol">The expression symbol to be analysed.</param>
    /// <returns>The semantic representation of the expression.</returns>
    /// <exception cref="Exception">Thrown when the symbol is not a recognized expression symbol.</exception>
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

    /// <summary>
    /// Analyses a literal expression and returns its semantic representation.
    /// </summary>
    public static LiteralExpressionSemantic AnalyseLiteralExpression(
        SemanticContext context,
        LiteralExpressionSymbol symbol)
    {
        return LiteralExpressionAnalyser.AnalyseLiteralExpression(context, symbol);
    }

    /// <summary>
    /// Analyses a reference expression and returns its semantic representation.
    /// </summary>
    public static ReferenceExpressionSemantic AnalyseReferenceExpression(
        SemanticContext context,
        ReferenceExpressionSymbol symbol)
    {
        return ReferenceExpressionAnalyser.AnalyseReferenceExpression(context, symbol);
    }

    /// <summary>
    /// Analyses an operator expression and returns its semantic representation.
    /// </summary>
    public static OperatorExpressionSemantic AnalyseOperatorExpression(
        SemanticContext context,
        OperatorExpressionSymbol symbol)
    {
        return OperatorExpressionAnalyser.AnalyseOperatorExpression(context, symbol);
    }

    /// <summary>
    /// Analyses a lambda expression and returns its semantic representation.
    /// </summary>
    public static LambdaExpressionSemantic AnalyseLambdaExpression(
        SemanticContext context,
        LambdaExpressionSymbol symbol)
    {
        return LambdaExpressionAnalyser.AnalyseLambdaExpression(context, symbol);
    }

    /// <summary>
    /// Analyses an anonymous type expression and returns its semantic representation.
    /// </summary>
    public static TypeProjectionExpressionSemantic AnalyseAnonymousTypeExpression(
        SemanticContext context,
        AnonymousTypeExpressionSymbol symbol)
    {
        return AnonymousTypeExpressionAnalyser.AnalyseAnonymousTypeExpression(context, symbol);
    }
}
