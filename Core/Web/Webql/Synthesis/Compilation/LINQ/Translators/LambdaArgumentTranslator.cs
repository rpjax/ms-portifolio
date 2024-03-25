using ModularSystem.Webql.Analysis.Semantics;
using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Synthesis.Compilation.LINQ.Extensions;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis.Compilation.LINQ;

public class LambdaArgumentTranslator
{
    public ParameterExpression[] TranslateLambdaArguments(TranslationContext context, LambdaArgumentsSymbol args)
    {
        var expressions = new List<ParameterExpression>();
        var semantics = args.GetSemantics<LambdaArgumentsSemantics>(context);

        if (args.Arguments.Length != semantics.Types.Length)
        {
            throw new Exception();
        }

        for (int i = 0; i < args.Arguments.Length; i++)
        {
            var arg = args.Arguments[i];
            var identifier = arg.Identifier;
            var type = semantics.Types[i];

            var expr = Expression.Parameter(type, identifier);

            expressions.Add(expr);
            context.CreateTranslationTableEntry(identifier, expr, SymbolAccessMode.ReadOnly);
        }

        return expressions.ToArray();
    }
}
