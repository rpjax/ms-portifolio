using Webql.Exceptions;
using Webql.Parsing.Ast;

namespace Webql.Translation.Linq.Exceptions;

public class TranslationException : WebqlCompilationException
{
    public TranslationException(
        string message, 
        WebqlSyntaxNode node)
        : base(message, node.Metadata.StartPosition)
    {
    }
}
