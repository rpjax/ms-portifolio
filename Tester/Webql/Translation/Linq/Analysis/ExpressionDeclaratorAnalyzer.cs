using System.Linq.Expressions;
using System.Reflection;
using Webql.Core;
using Webql.Parsing.Analysis;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;
using Webql.Semantics.Symbols;
using Webql.Translation.Linq.Context;
using Webql.Translation.Linq.Extensions;

namespace Webql.Translation.Linq.Analysis;

public class ExpressionDeclaratorAnalyzer : SyntaxTreeAnalyzer
{
    private TranslationContext? LastAnalyzedContext { get; set; }

    public ExpressionDeclaratorAnalyzer()
    {
        
    }

    protected override void Analyze(WebqlSyntaxNode? node)
    {
        if (node is null)
        {
            return;
        }

        var translationContext = node.GetTranslationContext();

        if(translationContext == LastAnalyzedContext)
        {
            base.Analyze(node);
            return;
        }

        var scope = node.GetScope();

        foreach (var symbol in scope.GetSymbols())
        {
            if (translationContext.ContainsExpression(symbol.Identifier))
            {
                continue;
            }

            /*
             * Currently, all symbols are parameters. This will change in the future with the introduction of temporary variables.
             */
            if (symbol is IParameterSymbol parameterSymbol)
            {
                var expression = Expression.Parameter(
                    type: parameterSymbol.Type, 
                    name: parameterSymbol.Identifier
                );

                translationContext.DeclareExpression(symbol.Identifier, expression);
            }
            // other symbols...
        }

        LastAnalyzedContext = translationContext;
        base.Analyze(node);
    }

}
