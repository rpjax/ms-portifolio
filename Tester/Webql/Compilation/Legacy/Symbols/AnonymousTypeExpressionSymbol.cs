using ModularSystem.Webql.Analysis.Components;
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
    /// <param name="bindings">The type bindings of the anonymous type.</param>
    public AnonymousTypeExpressionSymbol(TypeBindingSymbol[] bindings)
    {
        Bindings = bindings;
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

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteAnonymousTypeExpression(this);
    }

    protected override IEnumerable<Symbol> GetChildren()
    {
        foreach (var item in Bindings)
        {
            yield return item;
        }
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
    /// <param name="name">The name of the type binding.</param>
    /// <param name="value">The value of the type binding.</param>
    public TypeBindingSymbol(string name, ExpressionSymbol value)
    {
        Name = name;
        Value = value;
    }

    public override string ToString()
    {
        return $"{{ {Name}: {Value} }}";
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteTypeBinding(this);
    }

    protected override IEnumerable<Symbol> GetChildren()
    {
        yield return Value;
    }
}
