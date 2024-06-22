using System.Linq.Expressions;
using Webql.Core;
using Webql.Exceptions;
using Webql.Parsing.Ast;

namespace Webql.Translation.Linq;

public static class WebqlLinqTranslator
{
    public static Expression Translate(TranslationContext context, WebqlSyntaxNode node)
    {
        switch (node.NodeType)
        {
            case WebqlNodeType.Query:
                throw new NotImplementedException();

            case WebqlNodeType.Expression:
                throw new NotImplementedException();

            default:
                throw new TranslationException("Unknown node type", context);
        }
    }
}

public class TranslationContext
{
    public WebqlCompilationContext CompilationContext { get; }

    public TranslationContext(WebqlCompilationContext compilationContext)
    {
        CompilationContext = compilationContext;
    }
}

public class TranslationException : WebqlCompilationException
{
    public TranslationException(string message, TranslationContext context)
        : base(message, new ModularSystem.Core.TextAnalysis.Parsing.Components.SyntaxElementPosition())
    {
    }
}
