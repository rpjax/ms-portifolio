using ModularSystem.Webql.Analysis.Semantics.Analysers;
using ModularSystem.Webql.Analysis.Semantics.Extensions;
using ModularSystem.Webql.Analysis.Symbols;
using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Webql.Analysis.Semantics;

public static class SemanticAnalyser
{
    public static Type GetType(SemanticContextOld context, string identifier)
    {
        return Type.GetType(identifier)
           ?? throw new Exception();
    }

    public static LambdaExpressionSemantic AnalyseLambda(SemanticContextOld context, LambdaExpressionSymbol symbol)
    {
        if (GetCachedSemantics<LambdaExpressionSemantic>(context, symbol, out var cached))
        {
            return cached;
        }

        return ExpressionAnalyser.AnalyseLambdaExpression(context, symbol);
    }

    public static DeclarationStatementSemantic AnalyseDeclaration(SemanticContextOld context, IDeclarationSymbol symbol)
    {
        if (GetCachedSemantics<DeclarationStatementSemantic>(context, symbol, out var cached))
        {
            return cached;
        }

        return DeclarationAnalyser.AnalyseDeclaration(context, symbol);
    }

    public static DeclarationStatementSemantic[] AnalyseDeclarations(SemanticContextOld context, IDeclarationSymbol[] symbol)
    {
        return DeclarationAnalyser.AnalyseDeclarations(context, symbol);
    }

    public static StatementBlockSemantic AnalyseStatementBlock(SemanticContextOld context, StatementBlockSymbol symbol)
    {
        if (GetCachedSemantics<StatementBlockSemantic>(context, symbol, out var cached))
        {
            return cached;
        }

        return StatementBlockAnalyser.AnalyseStatementBlock(context, symbol);
    }

    public static ExpressionSemantic AnalyseExpression(SemanticContextOld context, ExpressionSymbol symbol)
    {
        if (GetCachedSemantics<ExpressionSemantic>(context, symbol, out var cached))
        {
            return cached;
        }

        return ExpressionAnalyser.Analyse(context, symbol);
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
