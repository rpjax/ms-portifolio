using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public class LambdaArgumentsAnalyser : SemanticAnalyserBase
{
    public LambdaArgumentsSemantics AnalyseLambdaArguments(SemanticContext context, LambdaArgumentsSymbol symbol)
    {
        var types = new List<Type>();

        foreach (var arg in symbol.Arguments)
        {
            var argSemanticc = SemanticsAnalyser.AnalyseLambdaArgument(context, arg);
            var type = argSemanticc.Type;

            types.Add(type);
        }

        return new LambdaArgumentsSemantics(
            types: types.ToArray()
        );
    }
}
