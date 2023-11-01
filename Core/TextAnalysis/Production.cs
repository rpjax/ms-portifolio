namespace ModularSystem.Core.TextAnalysis;

/// <summary>
/// A context-free-grammar, type 2, production. <br/>
/// Represents a rule of how tokens can ge grouped together to form another token. <br/>
/// The production follows the formation rule: '<c>α→β</c>', where <br/>
/// <list type="bullet">
/// <item>α is the LHS.</item>
/// <item>α is a single non-terminal, described as: α∈N (α is contained in N)</item>
/// <item>β is the RHS.</item>
/// <item>β is a sequence of, one or more, terminals or non-terminals, described as: β = N∪Σ (non-terminal union terminals)</item>
/// </list>
/// </summary>
public abstract class Production
{
    /// <summary>
    /// Gets the name of the symbol at the left hand side of the production, as defined by the grammar. <br/>
    /// Ex: &lt;digit&gt;, &lt;string&gt;, &lt;epsilon&gt; etc...
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets the symbol at the left hand side(LHS) of the production rule.
    /// </summary>
    /// <returns></returns>
    public abstract Symbol LeftHandSide();

    /// <summary>
    /// Gets the symbols at the right hand side(RHS) of the production rule.
    /// </summary>
    /// <returns></returns>
    public abstract Symbol[] RightHandSide();
}

// <S> ::= <expr>
// <expr> ::= <term> | <expr><term>
// <term> ::= <number> 
// <number> ::= <digit> | <number><digit>
// <digit> ::= 