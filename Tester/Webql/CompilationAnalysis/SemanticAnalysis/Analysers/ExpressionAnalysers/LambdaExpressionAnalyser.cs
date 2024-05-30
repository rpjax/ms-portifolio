using ModularSystem.Webql.Analysis.Semantics.Extensions;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public static class LambdaExpressionAnalyser
{
    public static LambdaExpressionSemantic AnalyseLambdaExpression(
        SemanticContext context,
        LambdaExpressionSymbol symbol)
    {
        var paramsSemantics = SemanticAnalyser.AnalyseDeclarations(context, symbol.Parameters)
            .ToArray();

        var paramsTypes = paramsSemantics
            .Select(x => x.Type)
            .ToArray();

        //for (int i = 0; i < symbol.Parameters.Length; i++)
        //{
        //    var param = symbol.Parameters[i];
        //    var semantic = paramsSemantics[i];

        //    param.AddDeclaration(context, param.Identifier, semantic.ExpressionType);
        //}

        var bodySemantic = SemanticAnalyser.AnalyseStatementBlock(
            context: context.GetSymbolContext(symbol.Body),
            symbol: symbol.Body
        );

        return new LambdaExpressionSemantic(
            parameterTypes: paramsTypes,
            returnType: bodySemantic.ReturnType
        );
    }

}
