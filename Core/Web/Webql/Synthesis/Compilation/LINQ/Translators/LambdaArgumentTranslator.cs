using ModularSystem.Webql.Analysis.Semantics;
using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Synthesis.Compilation.LINQ.Extensions;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis.Compilation.LINQ;

public class LambdaArgumentTranslator
{
    public ParameterExpression[] TranslateLambdaArguments(TranslationContext context, LambdaArgumentSymbol[] args)
    {
        var expressions = new List<ParameterExpression>();

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            var semantics = arg.GetSemantics<LambdaArgumentSemantic>(context);

            var identifier = arg.Identifier;
            var type = semantics.Type;

            var expr = Expression.Parameter(type, identifier);

            expressions.Add(expr);
            context.CreateTranslationTableEntry(identifier, expr, SymbolAccessMode.ReadOnly);
        }

        return expressions.ToArray();
    }
}
