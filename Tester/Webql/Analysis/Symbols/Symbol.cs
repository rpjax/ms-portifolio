using ModularSystem.Webql.Analysis.Components;
using System.Globalization;

namespace ModularSystem.Webql.Analysis.Symbols;

public interface ISymbol
{
    string Hash { get; }
    IEnumerable<Symbol> Children { get; }

    Symbol Accept(AstRewriter rewriter);
    string ToString();
}

/// <summary>
/// Marks a symbol produces a scope.
/// </summary>
public interface IScopeSymbol : ISymbol
{
    
}

/// <summary>
/// Marks a symbol that has an identifier.
/// </summary>
public interface IIdentifierSymbol : ISymbol
{
    string Identifier { get; }
}

/// <summary>
/// Marks a symbol that has a type.
/// </summary>
public interface ITypeSymbol : ISymbol
{
    string? Type { get; }
}

/// <summary>
/// Marks a symbol that is a declaration.
/// </summary>
public interface IDeclarationSymbol : ITypeSymbol, IIdentifierSymbol
{
    public string[] Modifiers { get; }
}

public interface IFunctionParameterSymbol : IDeclarationSymbol
{
    
}

/// <summary>
/// Marks a symbol that is an allocation.
/// </summary>
public interface IAllocationSymbol : IDeclarationSymbol
{
    public int Size { get; }
}

public abstract class Symbol : ISymbol
{
    public string Hash { get; }
    public IEnumerable<Symbol> Children { get; }

    protected Symbol()
    {
        Hash = Guid.NewGuid().ToString();
        Children = GetChildren();
    }

    public abstract override string ToString();
    public abstract Symbol Accept(AstRewriter rewriter);
    protected abstract IEnumerable<Symbol> GetChildren();

    protected static string ToCamelCase(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        // Se o valor já começa com um caractere minúsculo, retorne como está.
        if (char.IsLower(value[0]))
        {
            return value;
        }

        // Converta o primeiro caractere para minúsculo.
        string camelCase = char.ToLower(value[0], CultureInfo.CurrentCulture) + value.Substring(1);

        return camelCase;
    }

    protected string Stringify(OperatorType operatorType)
    {
        return $"${ToCamelCase(operatorType.ToString())}";
    }
}
