namespace ModularSystem.Webql;

/// <summary>
/// Defines the syntax rules for the language.
/// <br/><br/>
/// &lt;literal&gt; ::= &lt;string&gt; | &lt;number&gt; | &lt;bool&gt; | "null"
/// <br/>
/// &lt;array&gt; ::= "[" &lt;array-content&gt; "]"
/// <br/>
/// &lt;array-content&gt; ::=  &lt;literal&gt; | &lt;scope-declaration&gt; | ε
/// <br/>
/// &lt;LHS&gt; ::= &lt;identifier&gt; | &lt;operator&gt;
/// <br/>
/// &lt;RHS&gt; ::= &lt;literal&gt; | &lt;expression&gt; | &lt;array&gt;
/// <br/>
/// &lt;expression&gt; ::= &lt;LHS&gt; ":" &lt;RHS&gt;
/// <br/>
/// &lt;scope-definition&gt; ::= "{" &lt;scope-content&gt; "}"
/// <br/>
/// &lt;scope-content&gt; ::=&lt;expression-list&gt; | ε
/// <br/>
/// &lt;expression-list&gt; ::= &lt;expression&gt; &lt;expression-list-tail&gt;
/// <br/>
/// &lt;expression-list-tail&gt; ::= "," &lt;expression&gt; &lt;expression-list-tail&gt; | ε
/// </summary>
public static class Grammar
{

}
