using System.Globalization;

namespace ModularSystem.Core.TextAnalysis.Tokenization.Components;

public static class TokenizerAlphabet
{
    public static char Underline { get; } = '_';
    public static char Escape { get; } = '\\';
    public static char[] StringDelimiters { get; } = "'\"".ToCharArray();

    public static bool IsDigit(char c) => char.IsDigit(c);
    public static bool IsLetter(char c) => char.IsLetter(c);
    public static bool IsDigitOrLetter(char c) => char.IsLetterOrDigit(c);
    /// <summary>
    /// Determines whether the specified character is a punctuation. <br/>
    /// All special characters normally used in programming languages are considered punctuation, this includes operators and delimiters.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static bool IsPunctuation(char c)
    {
        switch (char.GetUnicodeCategory(c))
        {
            // Punctuation categories
            case UnicodeCategory.ConnectorPunctuation:
            case UnicodeCategory.DashPunctuation:
            case UnicodeCategory.OpenPunctuation:
            case UnicodeCategory.ClosePunctuation:
            case UnicodeCategory.InitialQuotePunctuation:
            case UnicodeCategory.FinalQuotePunctuation:
            case UnicodeCategory.OtherPunctuation:
                return true;

            // Symbol categories that are commonly used as operators
            case UnicodeCategory.MathSymbol:
            case UnicodeCategory.CurrencySymbol:
            case UnicodeCategory.ModifierSymbol:
            case UnicodeCategory.OtherSymbol:
                return true;

            default:
                return false;
        }
    }
    public static bool IsWhiteSpace(char c) => char.IsWhiteSpace(c);
    public static bool IsControl(char c) => char.IsControl(c);

    public static bool IsStringDelimiter(char c) => StringDelimiters.Contains(c);
    public static bool IsEscape(char c) => c == Escape;
    public static bool IsUnderline(char c) => c == Underline;

    public static bool IsHexadecimal(char c) => char.IsDigit(c) || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');    

    public static CharType GetCharType(char c)
    {
        if (IsDigit(c))
        {
            return CharType.Digit;
        }

        if (IsLetter(c))
        {
            return CharType.Letter;
        }

        /*
         * check for string delimiters must be done before checking for punctuation, because string delimiters are also punctuation.
         */

        if (c == '\'' || c == '"')
        {
            return CharType.StringDelimiter;
        }

        if (IsPunctuation(c))
        {
            return CharType.Punctuation;
        }

        if (IsWhiteSpace(c))
        {
            return CharType.Whitespace;
        }

        if (IsControl(c))
        {
            return CharType.Control;
        }

        throw new Exception($"Unknown character type: '{c}'.");
    }

}
