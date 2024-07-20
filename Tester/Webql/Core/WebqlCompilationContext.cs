using ModularSystem.Core;
using Webql.Parsing.Ast;

namespace Webql.Core;

public class WebqlCompilationContext
{
    public WebqlCompilerSettings Settings { get; }
    public Type ElementType { get; }

    private List<Error> Errors { get; } 

    public WebqlCompilationContext(
        WebqlCompilerSettings settings,
        Type elementType)
    {
        Settings = settings;
        ElementType = elementType;
        Errors = new List<Error>();
    }

    public WebqlLinqProvider LinqProvider => Settings.LinqProvider;

    public Type GetQueryableType(WebqlSyntaxNode node)
    {
        return LinqProvider.GetQueryableType(node);
    }

}
