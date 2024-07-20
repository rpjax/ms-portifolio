using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analyzers;

/// <summary>
/// Provides functionality to Analyze expressions.
/// </summary>
public static class ExpressionAnalyzer
{
    /// <summary>
    /// Analyzes an expression and returns its semantic representation.
    /// </summary>
    /// <param name="context">The semantic context in which the analysis is performed.</param>
    /// <param name="symbol">The expression symbol to be Analyzed.</param>
    /// <returns>The semantic representation of the expression.</returns>
    /// <exception cref="Exception">Thrown when the symbol is not a recognized expression symbol.</exception>
    public static ExpressionSemantic Analyze(SemanticContextOld context, ExpressionSymbol symbol)
    {
        switch (symbol.ExpressionType)
        {
            case ExpressionType.Literal:
                return AnalyzeLiteralExpression(context, (LiteralExpressionSymbol)symbol);

            case ExpressionType.Reference:
                return AnalyzeReferenceExpression(context, (ReferenceExpressionSymbol)symbol);

            case ExpressionType.Operator:
                return AnalyzeOperatorExpression(context, (OperatorExpressionSymbol)symbol);

            case ExpressionType.Lambda:
                return AnalyzeLambdaExpression(context, (LambdaExpressionSymbol)symbol);

            case ExpressionType.AnonymousType:
                return AnalyzeAnonymousTypeExpression(context, (AnonymousTypeExpressionSymbol)symbol);
        }

        throw new Exception();
    }

    /// <summary>
    /// Analyzes a literal expression and returns its semantic representation.
    /// </summary>
    public static LiteralExpressionSemantic AnalyzeLiteralExpression(
        SemanticContextOld context,
        LiteralExpressionSymbol symbol)
    {
        return LiteralExpressionAnalyzer.AnalyzeLiteralExpression(context, symbol);
    }

    /// <summary>
    /// Analyzes a reference expression and returns its semantic representation.
    /// </summary>
    public static ReferenceExpressionSemantic AnalyzeReferenceExpression(
        SemanticContextOld context,
        ReferenceExpressionSymbol symbol)
    {
        return ReferenceExpressionAnalyzer.AnalyzeReferenceExpression(context, symbol);
    }

    /// <summary>
    /// Analyzes an operator expression and returns its semantic representation.
    /// </summary>
    public static OperatorExpressionSemantic AnalyzeOperatorExpression(
        SemanticContextOld context,
        OperatorExpressionSymbol symbol)
    {
        return OperatorExpressionAnalyzer.AnalyzeOperatorExpression(context, symbol);
    }

    /// <summary>
    /// Analyzes a lambda expression and returns its semantic representation.
    /// </summary>
    public static LambdaExpressionSemantic AnalyzeLambdaExpression(
        SemanticContextOld context,
        LambdaExpressionSymbol symbol)
    {
        return LambdaExpressionAnalyzer.AnalyzeLambdaExpression(context, symbol);
    }

    /// <summary>
    /// Analyzes an anonymous type expression and returns its semantic representation.
    /// </summary>
    public static TypeProjectionExpressionSemantic AnalyzeAnonymousTypeExpression(
        SemanticContextOld context,
        AnonymousTypeExpressionSymbol symbol)
    {
        return AnonymousTypeExpressionAnalyzer.AnalyzeAnonymousTypeExpression(context, symbol);
    }
}
