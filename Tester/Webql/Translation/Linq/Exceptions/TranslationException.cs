using ModularSystem.Core.TextAnalysis.Parsing.Components;
using Webql.Exceptions;
using Webql.Translation.Linq.Context;

namespace Webql.Translation.Linq.Exceptions;

public class TranslationException : WebqlCompilationException
{
    public TranslationException(
        string message, 
        TranslationContext context)
        : base(message, new SyntaxElementPosition())
    {
    }
}
