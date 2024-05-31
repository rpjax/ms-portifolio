namespace ModularSystem.Core.TextAnalysis.Tokenization.Components;

/// <summary>
/// Represents the type of a character in the source text. It caracterizes <see cref="char"/> into a specific category.
/// </summary>
public enum CharType
{
    Digit,
    Letter,
    Punctuation,
    StringDelimiter,
    Whitespace,
    Control,
    Unknown
}
