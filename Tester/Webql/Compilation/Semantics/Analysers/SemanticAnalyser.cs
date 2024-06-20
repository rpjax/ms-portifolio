using ModularSystem.Webql.Analysis.Semantics.Analyzers;
using ModularSystem.Webql.Analysis.Semantics.Extensions;
using ModularSystem.Webql.Analysis.Symbols;
using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Webql.Analysis.Semantics;

public static class SemanticAnalyzer
{
    public static Type GetType(SemanticContextOld context, string identifier)
    {
        return Type.GetType(identifier)
           ?? throw new Exception();
    }

    public static LambdaExpressionSemantic AnalyzeLambda(SemanticContextOld context, LambdaExpressionSymbol symbol)
    {
        if (GetCachedSemantics<LambdaExpressionSemantic>(context, symbol, out var cached))
        {
            return cached;
        }

        return ExpressionAnalyzer.AnalyzeLambdaExpression(context, symbol);
    }

    public static DeclarationStatementSemantic AnalyzeDeclaration(SemanticContextOld context, IDeclarationSymbol symbol)
    {
        if (GetCachedSemantics<DeclarationStatementSemantic>(context, symbol, out var cached))
        {
            return cached;
        }

        return DeclarationAnalyzer.AnalyzeDeclaration(context, symbol);
    }

    public static DeclarationStatementSemantic[] AnalyzeDeclarations(SemanticContextOld context, IDeclarationSymbol[] symbol)
    {
        return DeclarationAnalyzer.AnalyzeDeclarations(context, symbol);
    }

    public static StatementBlockSemantic AnalyzeStatementBlock(SemanticContextOld context, StatementBlockSymbol symbol)
    {
        if (GetCachedSemantics<StatementBlockSemantic>(context, symbol, out var cached))
        {
            return cached;
        }

        return StatementBlockAnalyzer.AnalyzeStatementBlock(context, symbol);
    }

    public static ExpressionSemantic AnalyzeExpression(SemanticContextOld context, ExpressionSymbol symbol)
    {
        if (GetCachedSemantics<ExpressionSemantic>(context, symbol, out var cached))
        {
            return cached;
        }

        return ExpressionAnalyzer.Analyze(context, symbol);
    }

    //*
    //* private stuff.
    //*

    private static bool GetCachedSemantics<T>(
        SemanticContextOld context,
        ISymbol symbol,
        [NotNullWhen(true)] out T? value) where T : SymbolSemantic
    {
        value = null;
        return false;
    }

}
