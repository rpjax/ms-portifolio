namespace Webql.Semantics.Components;

/*
 * Symbols
 * 
 * Dev Note: A symbol is a named piece of semantics.
 */

public interface ISymbol : ISemantics
{
    public string Identifier { get; }
    Type Type { get; }
}

/*
 * Concrete implementations
 */

public class LhsSymbol : ISymbol
{
    public string Identifier { get; }
    public Type Type { get; }

    public LhsSymbol(string identifier, Type type)
    {
        Identifier = identifier;
        Type = type;
    }

}

public class ScopePropertySymbol : ISymbol
{
    public Type Type { get; }
    public string Identifier { get; }

    public ScopePropertySymbol(string identifier, Type type)
    {
        Identifier = identifier;
        Type = type;
    }

}
