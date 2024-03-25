using ModularSystem.Webql.Analysis.Semantics.Analysers;
using ModularSystem.Webql.Analysis.Semantics.Extensions;
using ModularSystem.Webql.Analysis.Symbols;
using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Webql.Analysis.Semantics;

public static class SemanticsAnalyser
{
    public static SymbolSemantics? TryAnalyse(SemanticContext context, Symbol symbol)
    {
        return null;
    }

    public static LambdaSemantics AnalyseLambda(SemanticContext context, LambdaSymbol symbol)
    {
        if (GetCachedSemantics<LambdaSemantics>(context, symbol, out var cached))
        {
            return cached;
        }

        return new LambdaSemanticsAnalyser()
            .AnalyseLambda(context, symbol);
    }

    public static LambdaArgumentSemantics AnalyseLambdaArgument(SemanticContext context, LambdaArgumentSymbol symbol)
    {
        if (GetCachedSemantics<LambdaArgumentSemantics>(context, symbol, out var cached))
        {
            return cached;
        }

        return new LambdaArgumentAnalyser()
            .AnalyseLambdaArgument(context, symbol);
    }

    public static LambdaArgumentsSemantics AnalyseLambdaArguments(SemanticContext context, LambdaArgumentsSymbol symbol)
    {
        return new LambdaArgumentsAnalyser()
            .AnalyseLambdaArguments(context, symbol);
    }

    public static StatementBlockSemantics AnalyseStatementBlock(SemanticContext context, StatementBlockSymbol symbol)
    {
        return new StatementBlockAnalyser()
            .AnalyseStatementBlock(context, symbol);
    }

    private static bool GetCachedSemantics<T>(
        SemanticContext context,
        Symbol symbol,
        [NotNullWhen(true)] out T? value) where T : SymbolSemantics
    {
        var semantics = symbol.TryGetSemantics<T>(context);

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
