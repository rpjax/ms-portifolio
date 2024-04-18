using ModularSystem.Core.TextAnalysis.Tokenization;
using System.Collections;

namespace ModularSystem.Core.TextAnalysis.Language;

/// <summary>
/// Definition of a context-free grammar. (Chomsky hierarchy type 2) <br/>
/// </summary>
/// <remarks>
/// The grammar is defined as a 4-tuple G = (N, T, P, S), where: <br/>
/// - N is a set of non-terminal symbols. <br/>
/// - T is a set of terminal symbols. <br/>
/// - P is a set of production rules. <br/>
/// - S is the start symbol. <br/>
/// </remarks>
public class GrammarDefinition
{
    public ProductionSet Productions { get; private set; }
    public NonTerminal Start { get; }

    public GrammarDefinition(ProductionRule[] productions, NonTerminal? start = null)
    {
        Productions = productions;
        Start = start ?? productions.First().Head;

        if (Start is null)
        {
            throw new ArgumentException("The start symbol cannot be null.");
        }

        ExpandMacros();
    }

    public IEnumerable<NonTerminal> GetNonTerminals()
    {
        return Productions
            .Select(x => x.Head)
            .Distinct();
    }

    public IEnumerable<Terminal> GetTerminals()
    {
        return Productions
            .SelectMany(x => x.Body)
            .OfType<Terminal>()
            .Distinct();
    }

    public IEnumerable<ProductionRule> GetProductions(NonTerminal nonTerminal)
    {
        return Productions.Where(x => x.Head.Name == nonTerminal.Name);
    }

    public override string ToString()
    {
        var productionGroups = Productions
            .GroupBy(x => x.Head.Name);

        var productionsStr = productionGroups
            .Select(x => string.Join(Environment.NewLine, x.Select(y => y.ToString())))
            .ToArray()
            ;

        return string.Join(Environment.NewLine, productionsStr);
    }

    private void ExpandMacros()
    {
        while (Productions.Any(x => x.ContainsMacro()))
        {
            var production = Productions.First(x => x.ContainsMacro());
            var macro = production.GetLeftmostMacro();

            if (macro is OptionMacro optionMacro)
            {
                var resolvedProductions = ResolveOptionMacro(production);

                Productions = Productions
                    .RemoveWhere(x => x == production)
                    .Concat(resolvedProductions)
                    .ToArray();

                continue;
            }

            if (macro is RepetitionMacro repetitionMacro)
            {
                var resolvedProductions = ResolveRepetitionMacro(production);

                Productions = Productions
                    .RemoveWhere(x => x == production)
                    .Concat(resolvedProductions)
                    .ToArray();

                continue;
            }

            throw new Exception($"The production rule contains an unsupported macro. {macro} is not a valid macro.");
        }
    }

    private ProductionRule[] ResolveOptionMacro(ProductionRule production)
    {
        var firstMacro = production.Body
            .First(x => x is OptionMacro)
            .AsOptionMacro();

        if (firstMacro is null)
        {
            throw new Exception("The production rule does not contain an option macro.");
        }

        var macroIndex = production.IndexOfSymbol(firstMacro);

        var epsilonBody = production.Body.ToList();
        epsilonBody.RemoveAt(macroIndex);
        var epsilonProduction = new ProductionRule(production.Head, epsilonBody.ToArray());

        var body = production.Body.ToList();
        body.RemoveAt(macroIndex);
        body.InsertRange(macroIndex, firstMacro.Sentence);
        var newProduction = new ProductionRule(production.Head, body.ToArray());

        return new ProductionRule[] { newProduction, epsilonProduction };
    }

    private ProductionRule[] ResolveRepetitionMacro(ProductionRule production)
    {
        var firstMacro = production.Body
            .First(x => x is RepetitionMacro)
            .AsRepetitionMacro();

        if (firstMacro is null)
        {
            throw new Exception("The production rule does not contain a repetition macro.");
        }

        var macroIndex = production.IndexOfSymbol(firstMacro);

        var epsilonBody = production.Body.ToList();
        epsilonBody.RemoveAt(macroIndex);
        var epsilonProduction = new ProductionRule(production.Head, epsilonBody.ToArray());

        var body = production.Body.ToList();
        body.RemoveAt(macroIndex);
        body.InsertRange(macroIndex, firstMacro.Sentence);
        var newProduction = new ProductionRule(production.Head, body.ToArray());

        return new ProductionRule[] { newProduction, epsilonProduction };
    }

}

/// <summary>
/// A production rule is a rule that defines how a non-terminal symbol can be replaced by a sequence of other symbols.
/// </summary>
public class ProductionRule
{
    // LHS
    public NonTerminal Head { get; }

    // RHS
    public Sentence Body { get; }

    public ProductionRule(NonTerminal head, params ProductionSymbol[] body)
    {
        Head = head;
        Body = body;
    }

    public ProductionRule(string name, params ProductionSymbol[] body)
    {
        Head = new NonTerminal(name);
        Body = body;
    }

    public override string ToString()
    {
        return $"{Head.Name.ToUpper()} -> {string.Join(" ", Body.Select(x => x.ToString()))}";
    }

    public int IndexOfSymbol(ProductionSymbol symbol)
    {
        var index = -1;

        foreach (var item in Body)
        {
            index++;

            if (ReferenceEquals(item, symbol))
            {
                break;
            }
        }

        return index;
    }

    public bool ContainsMacro()
    {
        return Body.Any(x => x.IsMacro);
    }

    public bool ContainsPipeMacro()
    {
        return Body.Any(x => x is PipeMacro);
    }

    public ProductionSymbol? GetLeftmostMacro()
    {
        return Body
            .FirstOrDefault(x => x.IsMacro);
    }

}

public class ProductionSet : IEnumerable<ProductionRule>
{
    private List<ProductionRule> Productions { get; }

    public ProductionSet(params ProductionRule[] productions)
    {
        Productions = new(productions);
    }

    public ProductionRule this[int index]
    {
        get => Productions[index];
        set => Productions[index] = value;
    }

    public static bool operator ==(ProductionSet left, ProductionSet right)
    {
        return left.SequenceEqual(right);
    }

    public static bool operator !=(ProductionSet left, ProductionSet right)
    {
        return !left.SequenceEqual(right);
    }

    public static ProductionSet operator +(ProductionSet left, ProductionSet right)
    {
        return new ProductionSet(left.Productions.Concat(right.Productions).ToArray());
    }

    public static implicit operator ProductionRule[](ProductionSet set)
    {
        return set.Productions.ToArray();
    }

    public static implicit operator List<ProductionRule>(ProductionSet set)
    {
        return set.Productions;
    }

    public static implicit operator ProductionSet(ProductionRule[] productions)
    {
        return new ProductionSet(productions);
    }

    public static implicit operator ProductionSet(List<ProductionRule> productions)
    {
        return new ProductionSet(productions.ToArray());
    }

    public int Length => Productions.Count;

    public IEnumerator<ProductionRule> GetEnumerator()
    {
        return Productions.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Productions.GetEnumerator();
    }

    public override bool Equals(object? obj)
    {
        return obj is ProductionSet set
            && set.SequenceEqual(this);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            foreach (var production in Productions)
            {
                hash = (hash * 16777619) ^ production.GetHashCode();
            }

            return hash;
        }
    }
    public override string ToString()
    {
        return string.Join(Environment.NewLine, Productions.Select(x => x.ToString()));
    }

    public ProductionSet Copy()
    {
        return new ProductionSet(Productions.ToArray());
    }

    public IEnumerable<ProductionRule> Lookup(NonTerminal nonTerminal)
    {
        return Productions.Where(x => x.Head == nonTerminal);
    }

    /*
     * macro expansion helper methods section.
     */

    private void ExpandMacros()
    {
        // first expand pipe macros

        while (Productions.Any(x => x.ContainsMacro()))
        {
            var production = Productions.First(x => x.ContainsMacro());
            var macro = production.GetLeftmostMacro();

            if (macro is PipeMacro)
            {

            }

            throw new Exception($"The production rule contains an unsupported macro. {macro} is not a valid macro.");
        }
    }

    private ProductionRule[] ExpandOptionMacro(ProductionRule production)
    {
        var firstMacro = production.Body
            .First(x => x is OptionMacro)
            .AsOptionMacro();

        if (firstMacro is null)
        {
            throw new Exception("The production rule does not contain an option macro.");
        }

        var macroIndex = production.IndexOfSymbol(firstMacro);

        var epsilonBody = production.Body.ToList();
        epsilonBody.RemoveAt(macroIndex);
        var epsilonProduction = new ProductionRule(production.Head, epsilonBody.ToArray());

        var body = production.Body.ToList();
        body.RemoveAt(macroIndex);
        body.InsertRange(macroIndex, firstMacro.Sentence);
        var newProduction = new ProductionRule(production.Head, body.ToArray());

        return new ProductionRule[] { newProduction, epsilonProduction };
    }

    private ProductionRule[] ResolveRepetitionMacro(ProductionRule production)
    {
        var firstMacro = production.Body
            .First(x => x is RepetitionMacro)
            .AsRepetitionMacro();

        if (firstMacro is null)
        {
            throw new Exception("The production rule does not contain a repetition macro.");
        }

        var macroIndex = production.IndexOfSymbol(firstMacro);

        var epsilonBody = production.Body.ToList();
        epsilonBody.RemoveAt(macroIndex);
        var epsilonProduction = new ProductionRule(production.Head, epsilonBody.ToArray());

        var body = production.Body.ToList();
        body.RemoveAt(macroIndex);
        body.InsertRange(macroIndex, firstMacro.Sentence);
        var newProduction = new ProductionRule(production.Head, body.ToArray());

        return new ProductionRule[] { newProduction, epsilonProduction };
    }

}

/// <summary>
/// Abstract base class for production symbols in a context-free grammar.
/// </summary>
public abstract class ProductionSymbol : IEquatable<ProductionSymbol>
{
    /// <summary>
    /// Gets a value indicating whether the production symbol is a terminal symbol.
    /// </summary>
    public abstract bool IsTerminal { get; }

    /// <summary>
    /// Gets a value indicating whether the production symbol is a non-terminal symbol.
    /// </summary>
    public abstract bool IsNonTerminal { get; }

    /// <summary>
    /// Gets a value indicating whether the production symbol is an epsilon symbol.
    /// </summary>
    public abstract bool IsEpsilon { get; }

    /// <summary>
    /// Gets a value indicating whether this production symbol is a macro.
    /// </summary>
    public abstract bool IsMacro { get; }


    public static bool operator ==(ProductionSymbol left, ProductionSymbol right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ProductionSymbol left, ProductionSymbol right)
    {
        return !left.Equals(right);
    }

    /// <summary>
    /// Returns a string representation of the production symbol.
    /// </summary>
    /// <returns>A string representation of the production symbol.</returns>
    public abstract override string ToString();

    public abstract override bool Equals(object? obj);

    public abstract override int GetHashCode();

    public abstract bool Equals(ProductionSymbol? other);

    public Terminal AsTerminal()
    {
        if (this is not Terminal terminal)
        {
            throw new InvalidCastException("The production symbol is not a terminal symbol.");
        }

        return terminal;
    }

    public NonTerminal AsNonTerminal()
    {
        if (this is not NonTerminal nonTerminal)
        {
            throw new InvalidCastException("The production symbol is not a non-terminal symbol.");
        }

        return nonTerminal;
    }

    public OptionMacro AsOptionMacro()
    {
        if (this is not OptionMacro optionMacro)
        {
            throw new InvalidCastException("The production symbol is not an option macro.");
        }

        return optionMacro;
    }

    public RepetitionMacro AsRepetitionMacro()
    {
        if (this is not RepetitionMacro repetitionMacro)
        {
            throw new InvalidCastException("The production symbol is not a repetition macro.");
        }

        return repetitionMacro;
    }
}

/// <summary>
/// Represents a terminal symbol in a context-free grammar.
/// </summary>
public class Terminal : ProductionSymbol
{
    /// <summary>
    /// Gets a value indicating whether the production symbol is a terminal symbol.
    /// </summary>
    public override bool IsTerminal => true;

    /// <summary>
    /// Gets a value indicating whether the production symbol is a non-terminal symbol.
    /// </summary>
    public override bool IsNonTerminal => false;

    /// <summary>
    /// Gets a value indicating whether the production symbol is an epsilon symbol.
    /// </summary>
    public override bool IsEpsilon => false;

    /// <summary>
    /// Gets a value indicating whether this production symbol is a macro.
    /// </summary>
    public override bool IsMacro => false;

    /// <summary>
    /// Gets the token type associated with the terminal symbol.
    /// </summary>
    public TokenType TokenType { get; }

    /// <summary>
    /// Gets the value associated with the terminal symbol.
    /// </summary>
    public string? Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Terminal"/> class with the specified token type and value.
    /// </summary>
    /// <param name="tokenType">The token type associated with the terminal symbol.</param>
    /// <param name="value">The value associated with the terminal symbol.</param>
    public Terminal(TokenType tokenType, string? value = null)
    {
        TokenType = tokenType;
        Value = value;
    }

    /// <summary>
    /// Returns a string representation of the terminal symbol.
    /// </summary>
    /// <returns>A string representation of the terminal symbol.</returns>
    public override string ToString()
    {
        var typeStr = TokenType.ToString();

        if (!string.IsNullOrEmpty(Value))
        {
            return $"\"{Value}\"";
        }

        return typeStr.ToUpper();
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ProductionSymbol);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            hash = (hash * 16777619) ^ TokenType.GetHashCode();
            hash = (hash * 16777619) ^ (Value?.GetHashCode() ?? 0);
            return hash;
        }
    }

    public override bool Equals(ProductionSymbol? other)
    {
        return other is Terminal terminal
            && terminal.TokenType == TokenType
            && terminal.Value == Value;
    }
}

/// <summary>
/// Represents a non-terminal symbol in a context-free grammar.
/// </summary>
public class NonTerminal : ProductionSymbol
{
    /// <summary>
    /// Gets a value indicating whether the production symbol is a terminal symbol.
    /// </summary>
    public override bool IsTerminal => false;

    /// <summary>
    /// Gets a value indicating whether the production symbol is a non-terminal symbol.
    /// </summary>
    public override bool IsNonTerminal => true;

    /// <summary>
    /// Gets a value indicating whether the production symbol is an epsilon symbol.
    /// </summary>
    public override bool IsEpsilon => false;

    /// <summary>
    /// Gets a value indicating whether this production symbol is a macro.
    /// </summary>
    public override bool IsMacro => false;

    /// <summary>
    /// Gets the name of the non-terminal symbol.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonTerminal"/> class with the specified name.
    /// </summary>
    /// <param name="name">The name of the non-terminal symbol.</param>
    public NonTerminal(string name)
    {
        if (name == GreekLetters.Epsilon.ToString())
        {
            throw new ArgumentException("The name of a non-terminal symbol cannot be epsilon.");
        }

        Name = name;
    }

    /// <summary>
    /// Returns a string representation of the non-terminal symbol.
    /// </summary>
    /// <returns>A string representation of the non-terminal symbol.</returns>
    public override string ToString()
    {
        return Name.ToUpper();
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ProductionSymbol);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            hash = (hash * 16777619) ^ Name.GetHashCode();
            return hash;
        }
    }

    public override bool Equals(ProductionSymbol? other)
    {
        return other is NonTerminal nonTerminal
            && nonTerminal.Name == Name;
    }
}

/// <summary>
/// Represents an epsilon symbol in a context-free grammar.
/// </summary>
public class Epsilon : ProductionSymbol
{
    /// <summary>
    /// Gets a value indicating whether the production symbol is a terminal symbol.
    /// </summary>
    public override bool IsTerminal => true;

    /// <summary>
    /// Gets a value indicating whether the production symbol is a non-terminal symbol.
    /// </summary>
    public override bool IsNonTerminal => false;

    /// <summary>
    /// Gets a value indicating whether the production symbol is an epsilon symbol.
    /// </summary>
    public override bool IsEpsilon => true;

    /// <summary>
    /// Gets a value indicating whether this production symbol is a macro.
    /// </summary>
    public override bool IsMacro => false;

    /// <summary>
    /// Initializes a new instance of the <see cref="Epsilon"/> class.
    /// </summary>
    public Epsilon()
    {
    }

    /// <summary>
    /// Returns a string representation of the epsilon symbol.
    /// </summary>
    /// <returns>A string representation of the epsilon symbol.</returns>
    public override string ToString()
    {
        return GreekLetters.Epsilon.ToString();
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ProductionSymbol);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            hash = (hash * 16777619) ^ ToString().GetHashCode();
            return hash;
        }
    }

    public override bool Equals(ProductionSymbol? other)
    {
        return other is Epsilon;
    }
}


public enum MacroType
{
    Option,
    Repetition,
    Pipe,
    Alternation
}

/*
    Macros are expanded recursively, one at a time, left to right. The order of expansion is as follows:
    1- Pipe macros are expanded into alternation macros.
    2- All other macros are expanded, left to right, until only alternation macros, or no macros are left.
    3- Alternation macros are expanded last.
    4- The process is repeated until no macros are left in the production rules.

    # Option macros are expanded following this rule:

    S -> A [ b ] C
    S -> A C | A b C

    # Repetition macros are expanded following this rule:

    S -> A { b } C
    S' -> b S' | ε
    S -> A S' C

    # Pipe macros are expanded by transforming them into a single alternation macro, using the pipes in the sentece as split points. Each alternative specified by the pipes becomes a child sentence of a newly created alternation macro:
    
    S -> Ab | Bc | Cd
    is transformed into:
    S -> (Ab, Bc, Cd)

    Where the sentence (Ab, Bc, Cd) is a single symbol (alternation macro), and Ab, Bc, Cd are child sentences of the alternation macro.
    This ensures that nested macros are expanded correctly.

    # Alternation macros within a sentence are expanded following this rule:      
    1- Determine the prefix of the sentence that contains the alternation macro, represented here by alpha (α).
    2- Determine the suffix of the sentence that contains the alternation macro, represented here by beta (β).
    3- For each child in the alternation macro, create a new production by concatenating the prefix, the child, and the suffix.

    For example:
    S -> Fg (Ab, Bc, Cd) Ef

    Where:
    α = Fg
    β = Ef

    Is transformed into:
    S -> Fg Ab Ef 
    S -> Fg Bc Ef 
    S -> Fg Cd Ef

    # Some complex examples:

    - Repetition macro, with pipe macro inside:
    S -> A { a | b } C

    First the pipe macro is expanded:
    S -> A { (a, b) } C

    Then, the repetition macro is expanded:
    S -> A S' C
    S' -> (a, b) S' 
    S' -> ε

    Then the alternation macro is expanded:
    S' -> (a, b) S' 
    S' -> a S' 
    S' -> b S'

    Observe that in this case alpha is empty, and beta is S'. The alternation macro is expanded by concatenating the prefix, the child, and the suffix.

    The final grammar should look like this:
    S -> A S' C
    S' -> a S'
    S' -> b S'
    S' -> ε

*/

public abstract class ProductionMacro : ProductionSymbol
{
    public override bool IsTerminal => false;
    public override bool IsNonTerminal => false;
    public override bool IsEpsilon => false;
    public override bool IsMacro => true;

    public abstract MacroType MacroType { get; }
}

public abstract class SentenceMacro : ProductionMacro
{
    public Sentence Sentence { get; }

    public SentenceMacro(Sentence sentence)
    {
        Sentence = sentence;

        if (Sentence.Length == 0)
        {
            throw new ArgumentException("The sequence macro must contain at least one symbol.");
        }
    }

    public override bool Equals(object? obj)
    {
        return obj is SentenceMacro macro
            && macro.Sentence.SequenceEqual(Sentence);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            foreach (var symbol in Sentence)
            {
                hash = (hash * 16777619) ^ symbol.GetHashCode();
            }

            return hash;
        }
    }

    public override bool Equals(ProductionSymbol? other)
    {
        return other is SentenceMacro macro
            && macro.Sentence.SequenceEqual(Sentence);
    }
}

public class OptionMacro : SentenceMacro
{
    public override MacroType MacroType => MacroType.Option;

    public OptionMacro(params ProductionSymbol[] symbols) : base(symbols)
    {
    }

    public override string ToString()
    {
        return $"[ {string.Join(" ", Sentence.Select(x => x.ToString()))} ]";
    }
}

public class RepetitionMacro : SentenceMacro
{
    public override MacroType MacroType => MacroType.Repetition;

    public RepetitionMacro(ProductionSymbol[] symbols) : base(symbols)
    {
    }

    public override string ToString()
    {
        return $"{{ {string.Join(" ", Sentence.Select(x => x.ToString()))} }}";
    }
}

public class PipeMacro : ProductionMacro
{
    public override MacroType MacroType => MacroType.Pipe;

    public PipeMacro()
    {

    }

    public override string ToString()
    {
        return "|";
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(obj, this);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            hash = (hash * 16777619) ^ MacroType.GetHashCode();

            return hash;
        }
    }

    public override bool Equals(ProductionSymbol? other)
    {
        return ReferenceEquals(other, this);
    }
}

public class AlternationMacro : ProductionMacro
{
    public override MacroType MacroType => MacroType.Alternation;

    public Sentence[] Alternatives { get; }

    public AlternationMacro(params Sentence[] alternatives)
    {
        Alternatives = alternatives;

        if (alternatives.Length == 0)
        {
            throw new ArgumentException("The alternation macro must contain at least one alternative.");
        }
    }

    public override string ToString()
    {
        return $"({string.Join(" | ", Alternatives.Select(x => x.ToString()))})";
    }

    public override bool Equals(object? obj)
    {
        return obj is AlternationMacro macro
            && macro.Alternatives.SequenceEqual(Alternatives);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            foreach (var alternative in Alternatives)
            {
                hash = (hash * 16777619) ^ alternative.GetHashCode();
            }

            return hash;
        }
    }

    public override bool Equals(ProductionSymbol? other)
    {
        return other is AlternationMacro macro
            && macro.Alternatives.SequenceEqual(Alternatives);
    }
}

/*
    Helper constructs.
*/

public class Sentence : IEnumerable<ProductionSymbol>
{
    public int Length => Symbols.Count;

    private List<ProductionSymbol> Symbols { get; }

    public Sentence(params ProductionSymbol[] symbols)
    {
        Symbols = new(symbols);
        ExpandPipeMacros();
    }

    public ProductionSymbol this[int index]
    {
        get => Symbols[index];
        set => Symbols[index] = value;
    }

    public static bool operator ==(Sentence left, Sentence right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Sentence left, Sentence right)
    {
        return !left.Equals(right);
    }

    public static implicit operator ProductionSymbol[](Sentence sentence)
    {
        return sentence.Symbols.ToArray();
    }

    public static implicit operator List<ProductionSymbol>(Sentence sentence)
    {
        return sentence;
    }

    public static implicit operator Sentence(ProductionSymbol[] productions)
    {
        return new Sentence(productions);
    }

    public static implicit operator Sentence(List<ProductionSymbol> productions)
    {
        return new Sentence(productions.ToArray());
    }

    // copilot, generate implicit conversion from and to productionSymbol array and list

    public IEnumerator<ProductionSymbol> GetEnumerator()
    {
        return Symbols.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Symbols.GetEnumerator();
    }

    public override string ToString()
    {
        return string.Join(" ", this.Select(x => x.ToString()));
    }

    public override bool Equals(object? obj)
    {
        return obj is Sentence sentence
            && sentence.SequenceEqual(this);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;

            foreach (var symbol in this)
            {
                hash = (hash * 16777619) ^ symbol.GetHashCode();
            }

            return hash;
        }
    }

    public bool Equals(Sentence? other)
    {
        return other is not null
            && other.SequenceEqual(this);
    }

    public Sentence Copy()
    {
        return new Sentence(Symbols.ToArray());
    }

    /// <summary>
    /// Gets the index of the specified symbol in the sentence. If the symbol is not found, -1 is returned. <br/>
    /// Equality is determined by reference equality. So, two symbols are considered equal if they are the same object.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public int IndexOfSymbol(ProductionSymbol symbol)
    {
        var index = -1;

        foreach (var item in this)
        {
            index++;

            if (ReferenceEquals(item, symbol))
            {
                break;
            }
        }

        return index;
    }

    /*
     * read helper methods section.
     */

    public ProductionSymbol GetLeftmostSymbol()
    {
        return this.First();
    }

    public ProductionSymbol GetRightmostSymbol()
    {
        return this.Last();
    }

    public NonTerminal? GetLeftmostNonTerminal()
    {
        return this
            .OfType<NonTerminal>()
            .FirstOrDefault();
    }

    public NonTerminal? GetRightmostNonTerminal()
    {
        return this
            .OfType<NonTerminal>()
            .LastOrDefault();
    }

    public Terminal? GetLeftmostTerminal()
    {
        return this
            .OfType<Terminal>()
            .FirstOrDefault();
    }

    public Terminal? GetRightmostTerminal()
    {
        return this
            .OfType<Terminal>()
            .LastOrDefault();
    }

    /*
     * derivation helper methods section.
     */

    public Derivation Derive(int index, ProductionRule production)
    {
        if (index < 0 || index >= Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "The index is out of range.");
        }

        if (this[index] is not NonTerminal nonTerminal)
        {
            throw new InvalidOperationException("The symbol at the specified index is not a non-terminal symbol. Derivations can only be performed on non-terminal symbols.");
        }

        if (nonTerminal != production.Head)
        {
            throw new InvalidOperationException("The non-terminal at the specified index does not match the head of the production rule.");
        }

        var derivedSentence = Copy()
            .RemoveAt(index)
            .InsertAt(index, production.Body);

        return new Derivation(
            production: production,
            originalSentence: this,
            nonTerminal: nonTerminal,
            derivedSentence: derivedSentence
        );
    }

    public Derivation DeriveLeftmostNonTerminal(ProductionRule production)
    {
        var nonTerminal = GetLeftmostNonTerminal();

        if (nonTerminal is null)
        {
            throw new InvalidOperationException("There are no non-terminals in the sentence.");
        }

        if (nonTerminal != production.Head)
        {
            throw new InvalidOperationException("The leftmost non-terminal in the sentence does not match the head of the production rule.");
        }

        var index = IndexOfSymbol(nonTerminal);

        if (index == -1)
        {
            throw new InvalidOperationException("The non-terminal was not found in the sentence.");
        }

        return Derive(index, production);
    }

    public Derivation DeriveRightmostNonTerminal(ProductionRule production)
    {
        var nonTerminal = GetRightmostNonTerminal();

        if (nonTerminal is null)
        {
            throw new InvalidOperationException("There are no non-terminals in the sentence.");
        }

        if (nonTerminal != production.Head)
        {
            throw new InvalidOperationException("The rightmost non-terminal in the sentence does not match the head of the production rule.");
        }

        var index = IndexOfSymbol(nonTerminal);

        if (index == -1)
        {
            throw new InvalidOperationException("The non-terminal was not found in the sentence.");
        }

        return Derive(index, production);
    }

    /*
     * private helper methods section.
     */

    private Sentence Add(ProductionSymbol symbol)
    {
        Symbols.Add(symbol);
        return this;
    }

    private Sentence Add(IEnumerable<ProductionSymbol> symbols)
    {
        Symbols.AddRange(symbols);
        return this;
    }

    private Sentence InsertAt(int index, ProductionSymbol symbol)
    {
        Symbols.Insert(index, symbol);
        return this;
    }

    private Sentence InsertAt(int index, IEnumerable<ProductionSymbol> symbols)
    {
        Symbols.InsertRange(index, symbols);
        return this;
    }

    private Sentence RemoveAt(int index)
    {
        Symbols.RemoveAt(index);
        return this;
    }

    private void ExpandPipeMacros()
    {
        if (Symbols.All(x => x is not PipeMacro))
        {
            return;
        }

        var pipeIndexes = Symbols
            .Select((x, i) => (x, i))
            .Where(x => x.x is PipeMacro)
            .Select(x => x.i)
            .ToList();

        var alternatives = new List<Sentence>();
        var start = 0;

        pipeIndexes.Add(Symbols.Count);

        foreach (var index in pipeIndexes)
        {
            var end = index;
            var length = end - start;

            var alternative = new Sentence(Symbols.Skip(start).Take(length).ToArray());
            alternatives.Add(alternative);

            start = end + 1;
        }

        var alternationMacro = new AlternationMacro(alternatives.ToArray());

        Symbols.Clear();
        Symbols.Add(alternationMacro);
    }

}

public class Derivation
{
    public ProductionRule Production { get; }
    public NonTerminal NonTerminal { get; }
    public Sentence OriginalSentence { get; }
    public Sentence DerivedSentence { get; }
    public int NonTerminalPosition { get; }

    public Derivation(
        ProductionRule production,
        NonTerminal nonTerminal,
        Sentence originalSentence,
        Sentence derivedSentence)
    {
        Production = production;
        NonTerminal = nonTerminal;
        OriginalSentence = originalSentence;
        DerivedSentence = derivedSentence;

        if (NonTerminalPosition >= OriginalSentence.Length)
        {
            throw new InvalidOperationException("The non-terminal position is out of bounds.");
        }
        if (OriginalSentence[NonTerminalPosition] != production.Head)
        {
            throw new InvalidOperationException("The non-terminal at the specified position does not match the head of the production rule.");
        }

        NonTerminalPosition = originalSentence.IndexOfSymbol(nonTerminal);
    }

    public override string ToString()
    {
        return $"{Production} ({OriginalSentence} -> {DerivedSentence})";
    }

}

public class LeftRecursionCicle
{
    public Derivation[] Derivations { get; }

    public LeftRecursionCicle(IEnumerable<Derivation> derivations)
    {
        Derivations = derivations.ToArray();

        if (Derivations.Length == 0)
        {
            throw new ArgumentException("The derivations array must have at least one element.");
        }
        if (derivations.Any(x => x.OriginalSentence.IsEmpty() || x.DerivedSentence.IsEmpty()))
        {
            throw new ArgumentException("The derivations must not have empty sentences.");
        }

        var start = derivations.First().OriginalSentence.GetLeftmostNonTerminal();

        if (start is null)
        {
            throw new ArgumentException("The first derivation must have a leftmost non-terminal.");
        }

        var lastNonTerminal = derivations.Last().DerivedSentence.GetLeftmostNonTerminal();

        if (lastNonTerminal is null)
        {
            throw new ArgumentException("The last derivation must have a leftmost non-terminal.");
        }

        if (lastNonTerminal != start)
        {
            throw new InvalidOperationException("The leftmost symbol of the last derivation must be the same as the start non-terminal.");
        }
    }

    public override string ToString()
    {
        return string.Join(Environment.NewLine, Derivations.Select(x => x.ToString()));
    }
}

