using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public class LambdaSemanticsAnalyser : SemanticAnalyserBase
{
    public LambdaSemantic AnalyseLambda(SemanticContext context, LambdaExpressionSymbol symbol)
    {
        var paramsTypes = SemanticAnalyser.AnalyseDeclarations(context, symbol.Parameters)
            .Select(x => x.Type)
            .ToArray();

        var bodySemantic = SemanticAnalyser.AnalyseStatementBlock(context, symbol.Body);

        return new LambdaSemantic(
            parameterTypes: paramsTypes,
            returnType: bodySemantic.ResolvedType
        );
    }
}
