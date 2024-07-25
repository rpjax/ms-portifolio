using ModularSystem.Webql.Analysis.Semantics;
using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Synthesis.Compilation.LINQ.Extensions;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis.Compilation.LINQ;

public class LambdaParamsTranslator
{
    public ParameterExpression[] TranslateLambdaParams(TranslationContext context, DeclarationStatementSymbol[] parameters)
    {
        var expressions = new List<ParameterExpression>();

        for (int i = 0; i < parameters.Length; i++)
        {
            var arg = parameters[i];
            var semantics = arg.GetSemantic<DeclarationStatementSemantic>(context);

            var identifier = arg.Identifier;
            var type = semantics.Type;

            var expr = Expression.Parameter(type, identifier);

            expressions.Add(expr);
            context.CreateTranslationTableEntry(identifier, expr, SymbolAccessMode.ReadOnly);
        }

        return expressions.ToArray();
    }
}
