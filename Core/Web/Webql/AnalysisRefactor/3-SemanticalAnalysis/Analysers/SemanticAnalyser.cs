﻿using ModularSystem.Webql.Analysis.Semantics.Analysers;
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

    public static SymbolSemantic? TryAnalyse(SemanticContext context, Symbol symbol)
    {
        return null;
    }

    public static LambdaSemantic AnalyseLambda(SemanticContext context, LambdaSymbol symbol)
    {
        if (GetCachedSemantics<LambdaSemantic>(context, symbol, out var cached))
        {
            return cached;
        }

        return new LambdaSemanticsAnalyser()
            .AnalyseLambda(context, symbol);
    }

    public static LambdaArgumentSemantic AnalyseLambdaArgument(SemanticContext context, LambdaArgumentSymbol symbol)
    {
        if (GetCachedSemantics<LambdaArgumentSemantic>(context, symbol, out var cached))
        {
            return cached;
        }

        return new LambdaArgumentAnalyser()
            .AnalyseLambdaArgument(context, symbol);
    }

    public static LambdaArgumentSemantic[] AnalyseLambdaArguments(SemanticContext context, LambdaArgumentSymbol[] symbol)
    {
        return new LambdaArgumentsAnalyser()
            .AnalyseLambdaArguments(context, symbol);
    }

    public static StatementBlockSemantic AnalyseStatementBlock(SemanticContext context, StatementBlockSymbol symbol)
    {
        return new StatementBlockAnalyser()
            .AnalyseStatementBlock(context, symbol);
    }

    public static ReferenceExpressionSemantic AnalyseReference(SemanticContext context, ReferenceExpressionSymbol symbol)
    {
        return new ReferenceExpressionAnalyser()
            .AnalyseReferenceExpression(context, symbol);
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
