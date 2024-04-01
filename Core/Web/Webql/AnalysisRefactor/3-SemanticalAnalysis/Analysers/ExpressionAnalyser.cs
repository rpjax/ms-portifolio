using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Syntax;

namespace ModularSystem.Webql.Analysis.Semantics;

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

public static class ReferenceExpressionAnalyser
{
    public static ReferenceExpressionSemantic AnalyseReferenceExpression(
        SemanticContext context,
        ReferenceExpressionSymbol symbol)
    {
        var identifier = symbol.GetNormalizedValue();
        var semantic = context.GetDeclarationSemantic(identifier);

        return new ReferenceExpressionSemantic(
            type: semantic.Type
        );
    }
}

public static class OperatorExpressionAnalyser
{
    public static OperatorExpressionSemantic AnalyseOperatorExpression(
        SemanticContext context,
        OperatorExpressionSymbol symbol)
    {
        var operatorSemanticType = OperatorHelper.GetOperatorSemanticType(symbol.Operator);
        MemberAccessExpressionSymbol
        var foo = 1.0 + 1;

        return new OperatorExpressionSemantic(
            type: null
        );
    }
}

public static class LambdaExpressionAnalyser
{
    public static LambdaExpressionSemantic AnalyseLambdaExpression(
        SemanticContext context,
        LambdaExpressionSymbol symbol)
    {
        var paramsTypes = SemanticAnalyser.AnalyseDeclarations(context, symbol.Parameters)
                    .Select(x => x.Type)
                    .ToArray();

        var bodySemantic = SemanticAnalyser.AnalyseStatementBlock(context, symbol.Body);

        return new LambdaExpressionSemantic(
            parameterTypes: paramsTypes,
            returnType: bodySemantic.ReturnType
        );
    }

}

public static class AnonymousTypeExpressionAnalyser
{
    public static TypeProjectionExpressionSemantic AnalyseAnonymousTypeExpression(
        SemanticContext context,
        AnonymousTypeExpressionSymbol symbol)
    {
        return new TypeProjectionExpressionSemantic(
            type: null
        );
    }
}
