namespace ModularSystem.Core.TextAnalysis.Tokenization;

/// <summary>
/// Represents the type of a token. This enumeration is used by the tokenizer to classify tokens.
/// </summary>
public enum TokenType : int
{
    /// <summary>
    /// Represents an unknown token type.
    /// </summary>
    Unknown,

    /// <summary>
    /// Represents the end of input (<c>$</c>).
    /// </summary>
    Eoi,

    /// <summary>
    /// Identifiers. Ex: <c>name</c>, <c>age</c>.
    /// </summary>
    Identifier,

    /// <summary>
    /// Special identifier that carries a specific meaning in the context of the language. Ex: <c>if</c>, <c>else</c>, <c>for</c>, <c>while</c> etc...
    /// </summary>
    Keyword,

    /// <summary>
    /// Single-character punctuators. Ex: <c>,</c> <c>:</c> <c>(</c> <c>[</c> <c>{</c> etc...
    /// </summary>
    Punctuation,

    /// <summary>
    /// Represents a comment sequence supported by the tokenizer. Currently, the tokenizer supports the following comment styles:
    /// <list type="bullet">
    /// <item>
    /// <description>C++ style: <c>/* */</c> and <c>// \n</c></description>
    /// </item>
    /// <item>
    /// <description>EBNF style: <c>(* *)</c></description>
    /// </item>
    /// </list>
    /// </summary>
    Comment,

    /*
     * literals.
     */

    /// <summary>
    /// String literal denoted by single or double quotes. Ex: <c>'hello'</c>, <c>"world"</c>.
    /// </summary>
    String,

    /// <summary>
    /// Integer literal. Ex: <c>50</c>, <c>-50</c>.
    /// </summary>
    Integer,

    /// <summary>
    /// Float literal. Ex: <c>0.25</c>, <c>-0.25</c>.
    /// </summary>
    Float,

    /// <summary>
    /// Hexadecimal literal. Ex: <c>0x1A</c>
    /// </summary>
    Hexadecimal,

    /// <summary>
    /// Binary literal. Ex: <c>0b1010</c>
    /// </summary>
    Binary,
}
