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
}

public interface ITypedSymbol : ISymbol, ITypedSemantics
{
    
}

public interface IParameterSymbol : ITypedSymbol
{
    
}

/*
 * Concrete implementations
 */

public class DeclarationSymbol : ITypedSymbol
{
    public string Identifier { get; }
    public Type Type { get; }

    public DeclarationSymbol(string identifier, Type type)
    {
        Identifier = identifier;
        Type = type;
    }
}

public class SourceSymbol : IParameterSymbol
{
    public string Identifier { get; }
    public Type Type { get; }

    public SourceSymbol(string identifier, Type type)
    {
        Identifier = identifier;
        Type = type;
    }
}

public class ParameterSymbol : IParameterSymbol
{
    public string Identifier { get; }
    public Type Type { get; }

    public ParameterSymbol(string identifier, Type type)
    {
        Identifier = identifier;
        Type = type;
    }

}

public class AccumulatorSymbol : ITypedSymbol
{
    public string Identifier { get; }
    public Type Type { get; }

    public AccumulatorSymbol(string identifier, Type type)
    {
        Identifier = identifier;
        Type = type;
    }

}