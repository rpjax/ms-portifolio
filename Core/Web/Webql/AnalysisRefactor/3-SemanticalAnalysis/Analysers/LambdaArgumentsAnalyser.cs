using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public class LambdaArgumentsAnalyser : SemanticAnalyserBase
{
    public LambdaArgumentSemantic[] AnalyseLambdaArguments(SemanticContext context, LambdaArgumentSymbol[] symbols)
    {
        return symbols
            .Select(x => SemanticAnalyser.AnalyseLambdaArgument(context, x))
            .ToArray();
    }

}
