using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public class LambdaSemanticsAnalyser : SemanticAnalyserBase
{
    public LambdaSemantics AnalyseLambda(SemanticContext context, LambdaSymbol symbol)
    {
        var argsSemantics = SemanticsAnalyser.AnalyseLambdaArguments(context, symbol.Arguments);
        var bodySemantics = SemanticsAnalyser.AnalyseStatementBlock(context, symbol.Body);

        return new LambdaSemantics(
            parameterTypes: argsSemantics.Types,
            returnType: bodySemantics.ResolvedType
        );
    }
}
