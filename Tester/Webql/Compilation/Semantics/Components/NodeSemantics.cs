using Microsoft.CodeAnalysis;
using ModularSystem.Core.TextAnalysis.Parsing.Components;
using Webql.DocumentSyntax.Parsing;
using Webql.DocumentSyntax.Parsing.Components;

namespace Webql.DocumentSyntax.Semantics.Components;

public class NodeSemantics
{
    
}

public interface ISemantics
{
    string? Name { get; }
}

public interface IWebqlSymbol
{
    string? Name { get; }
}

public interface ITypeSymbol : IWebqlSymbol
{
    
}

public interface IWebqlExpressionSymbol : IWebqlSymbol
{
    ITypeSymbol Type { get; }
}

public class SemanticContext
{
    private Dictionary<string, IWebqlSymbol> SymbolTable { get; }

    public SemanticContext()
    {
        SymbolTable = new Dictionary<string, IWebqlSymbol>();
    }

    void foo()
    {
        SemanticModel model = new SemanticModel();
        ITypeSymbol type = model.GetDeclaredSymbol();
    }
}
