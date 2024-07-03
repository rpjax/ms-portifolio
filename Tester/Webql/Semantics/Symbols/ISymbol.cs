using Webql.Semantics.Definitions;

namespace Webql.Semantics.Symbols;

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

public class DeclarationSymbol : ISymbol
{
    public Type Type { get; }
    public string Identifier { get; }

    public DeclarationSymbol(string identifier, Type type)
    {
        Identifier = identifier;
        Type = type;
    }

}

public class AccumulatorSymbol : ISymbol
{
    public Type Type { get; }
    public string Identifier { get; }

    public AccumulatorSymbol(string identifier, Type type)
    {
        Identifier = identifier;
        Type = type;
    }

}