using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public class LambdaArgumentAnalyser : SemanticAnalyserBase
{
    public LambdaArgumentSemantics AnalyseLambdaArgument(SemanticContext context, LambdaArgumentSymbol symbol)
    {
        return new LambdaArgumentSemantics(
            type: GetType(symbol.Type)
        );
    }
}
