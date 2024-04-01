using ModularSystem.Webql.Analysis.Semantics.Analysers;
using ModularSystem.Webql.Analysis.Semantics.Extensions;
using ModularSystem.Webql.Analysis.Symbols;
using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Webql.Analysis.Semantics;

public static class SemanticAnalyser
{
    public static Type GetType(SemanticContext context, string identifier)
    {
        return Type.GetType(identifier)
           ?? throw new Exception();
    }

    public static LambdaExpressionSemantic AnalyseLambda(SemanticContext context, LambdaExpressionSymbol symbol)
    {
        if (GetCachedSemantics<LambdaExpressionSemantic>(context, symbol, out var cached))
        {
            return cached;
        }

        return new LambdaSemanticsAnalyser()
            .AnalyseLambda(context, symbol);
    }

    public static DeclarationStatementSemantic AnalyseDeclaration(SemanticContext context, DeclarationStatementSymbol symbol)
    {
        if (GetCachedSemantics<DeclarationStatementSemantic>(context, symbol, out var cached))
        {
            return cached;
        }

        return new DeclarationSemanticAnalyser()
            .AnalyseDeclaration(context, symbol);
    }

    public static DeclarationStatementSemantic[] AnalyseDeclarations(SemanticContext context, DeclarationStatementSymbol[] symbol)
    {
        return new DeclarationSemanticAnalyser()
            .AnalyseDeclarations(context, symbol);
    }

    public static StatementBlockSemantic AnalyseStatementBlock(SemanticContext context, StatementBlockSymbol symbol)
    {
        return new StatementBlockAnalyser()
            .AnalyseStatementBlock(context, symbol);
    }

    public static ExpressionSemantic AnalyseExpression(SemanticContext context, ExpressionSymbol symbol)
    {
        return ExpressionAnalyser.Analyse(context, symbol);
    }

    //*
    //* private stuff.
    //*

    private static bool GetCachedSemantics<T>(
        SemanticContext context,
        Symbol symbol,
        [NotNullWhen(true)] out T? value) where T : SymbolSemantic
    {
        var semantics = symbol.TryGetSemantic<T>(context);

        if (semantics is not null)
        {
            value = semantics;
            return true;
        }
        else
        {
            value = null;
            return false;
        }
    }

}

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

            case ExpressionType.TypeProjection:
                return AnalyseTypeProjectionExpression(context, (TypeProjectionExpressionSymbol)symbol);
        }

        throw new Exception();
    }

    public static LiteralExpressionSemantic AnalyseLiteralExpression(
        SemanticContext context,
        LiteralExpressionSymbol symbol)
    {
        switch (symbol.LiteralType)
        {
            case LiteralType.Null:
                return AnalyseNullLiteral(context, (NullSymbol) symbol);

            case LiteralType.String:
                return AnalyseStringLiteral(context, (StringSymbol)symbol);

            case LiteralType.Bool:
                return AnalyseBoolLiteral(context, (BoolSymbol)symbol);

            case LiteralType.Number:
                return AnalyseNumberLiteral(context, (NumberSymbol)symbol);
        }

        throw new Exception();
    }

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

    public static OperatorExpressionSemantic AnalyseOperatorExpression(
        SemanticContext context, 
        OperatorExpressionSymbol symbol)
    {
        return new OperatorExpressionSemantic(
            type: null
        );
    }

    public static LambdaExpressionSemantic AnalyseLambdaExpression(
        SemanticContext context, 
        LambdaExpressionSymbol symbol
    )
    {
        return new LambdaExpressionSemantic(
            parameterTypes: null,
            returnType: null
        );
    }

    public static TypeProjectionExpressionSemantic AnalyseTypeProjectionExpression(
        SemanticContext context, 
        TypeProjectionExpressionSymbol symbol)
    {
        return new TypeProjectionExpressionSemantic(
            type: null
        );
    }

    //*
    //* literal expression sub-types semantic analysis.
    //*

    public static LiteralExpressionSemantic AnalyseNullLiteral(SemanticContext context,  NullSymbol symbol)
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