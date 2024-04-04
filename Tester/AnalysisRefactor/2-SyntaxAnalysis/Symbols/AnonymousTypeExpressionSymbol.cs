using System.Collections;

namespace ModularSystem.Webql.Analysis.Symbols;

/// <summary>
/// Represents an anonymous type expression symbol in the language.
/// </summary>
public class AnonymousTypeExpressionSymbol : ExpressionSymbol, IEnumerable<TypeBindingSymbol>
{
    public override ExpressionType ExpressionType { get; } = ExpressionType.AnonymousType;

    /// <summary>
    /// Gets the bindings of the anonymous type.
    /// </summary>
    public TypeBindingSymbol[] Bindings { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AnonymousTypeExpressionSymbol"/> class.
    /// </summary>
    /// <param name="expressions">The type bindings of the anonymous type.</param>
    public AnonymousTypeExpressionSymbol(TypeBindingSymbol[] expressions)
    {
        Bindings = expressions;
    }

    public IEnumerator<TypeBindingSymbol> GetEnumerator()
    {
        return Bindings.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Bindings.GetEnumerator();
    }

    public override string ToString()
    {
        var bindings = Bindings.Select(x => x.ToString());
        var exprs = string.Join($",{Environment.NewLine}", bindings);

        return $"{{ {exprs} }}";
    }
}

/// <summary>
/// Represents a type binding symbol in the language.
/// </summary>
public class TypeBindingSymbol : Symbol
{
    /// <summary>
    /// Gets the name of the type binding.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the value of the type binding.
    /// </summary>
    public ExpressionSymbol Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeBindingSymbol"/> class.
    /// </summary>
    /// <param name="key">The name of the type binding.</param>
    /// <param name="value">The value of the type binding.</param>
    public TypeBindingSymbol(string key, ExpressionSymbol value)
    {
        Name = key;
        Value = value;
    }

    public override string ToString()
    {
        return $"{{ {Name}: {Value} }}";
    }
}
