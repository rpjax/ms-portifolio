namespace Webql.Semantics.Components;

/*
 * Symbols
 */

public interface ISymbol
{
 
}

/*
 * Concrete implementations
 */

public class LhsSymbol : ISymbol
{
    Type Type { get; }

    public LhsSymbol(Type type)
    {
        Type = type;
    }

}

public class ScopePropertySymbol : ISymbol
{
    Type Type { get; }

    public ScopePropertySymbol(Type type)
    {
        Type = type;
    }

}
