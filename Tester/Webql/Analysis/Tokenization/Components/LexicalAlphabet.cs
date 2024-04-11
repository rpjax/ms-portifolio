using System.Text;

namespace ModularSystem.Webql.Analysis.Tokenization;

public static class LexicalAlphabet
{
    public static char Underline { get; } = '_';
    public static char Dollar { get; } = '$';
    public static char Dot { get; } = '.';
    public static char Comma { get; } = ',';
    public static char Colon { get; } = ':';
    public static char Semicolon { get; } = ';';
    public static char OpenSquareBracket { get; } = '[';
    public static char CloseSquareBracket { get; } = ']';
    public static char OpenCurlyBracket { get; } = '{';
    public static char CloseCurlyBracket { get; } = '}';
    public static char OpenParenthesis { get; } = '(';
    public static char CloseParenthesis { get; } = ')';
    public static char SingleQuote { get; } = '\'';
    public static char DoubleQuote { get; } = '"';
    public static char Backslash { get; } = '\\';
    public static char ForwardSlash { get; } = '/';
    public static char Minus { get; } = '-';
    public static char Plus { get; } = '+';
    public static char Asterisk { get; } = '*';
    public static char Asign { get; } = '=';

    public static char Escape { get; } = Backslash;

    public static string Equal { get; } = "==";
    public static string NotEqual { get; } = "!=";

    public static string Null { get; } = "null";
    public static string True { get; } = "true";
    public static string False { get; } = "false";

    public static char[] Letters { get; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    public static char[] Digits { get; } = "0123456789".ToCharArray();
    public static char[] StringDelimiters { get; } = "'\"".ToCharArray();

    public static char[] Punctuations { get; } = new char[] 
    { 
        Dot,
        Comma,
        Colon, 
        Semicolon, 
        OpenSquareBracket, 
        CloseSquareBracket, 
        OpenCurlyBracket, 
        CloseCurlyBracket, 
        OpenParenthesis, 
        CloseParenthesis, 
        SingleQuote,
        DoubleQuote,
        Backslash, 
        ForwardSlash, 
        Dollar
    };

    public static string[] Operators { get; } = new string[]
    {
        Plus.ToString(),
        Minus.ToString(),
        Asterisk.ToString(),
        ForwardSlash.ToString(),
        Asign.ToString(),
        Equal,
        NotEqual,
        "<",
        "<=",
        ">",
        ">=",
        "&&",
        "||",
        "?"
    };

    public static string[] Keywords { get; } = new string[]
    {
        Null,
        True,
        False,
    };

    public static Dictionary<string, IStateTransition> CreateTransitionTable()
    {
        var table = new Dictionary<string, IStateTransition>();

        AddInitialStateTransitions(table);
        AddKeywordOrIdentifierTransitions(table);

        return table;
    }

    private static void AddInitialStateTransitions(Dictionary<string, IStateTransition> table)
    {
        return;
    }

    static int counter = 0;

    private static void AddKeywordOrIdentifierTransitions(Dictionary<string, IStateTransition> table)
    {
        var sequences = Keywords
            .Select(x => x.ToCharArray())
            .ToArray();

        foreach (var sequence in sequences)
        {
            var transitions = GetTransitionsForKeyword(sequence);
            Console.WriteLine();
        }

        foreach (var item in table)
        {
            
        }

        throw new NotImplementedException();
    }

    private static IEnumerable<IStateTransition> GetTransitionsForKeyword(char[] values)
    {
        // "if" -> 'i', 'f'
        // "else" -> 'e', 'l', 's', 'e'
        var baseState = new string(values);
        var accumulator = new StringBuilder(baseState.Length);

        for (var i = 0; i < values.Length; i++)
        {
            var inputChar = values[i];

            /*
             * | State                         | NextState                      | Input           | Action   
             * 
             * | initial "" -> 'i'             | keyword('if')         | 'i'             | None   
             * | keyword('if') "" -> 'i'       | keyword('if') "i" -> 'f'       | 'i'             | Read   
             * | keyword('if') "i" -> 'f'      | keyword('if') "if" -> ''       | 'f'             | None   
             * | keyword('if') "i" -> '{punc}' | initial_state                  | '{punc}'        | None   
             */

            var nextState = $"keyword(\"{baseState}\") \"{accumulator}\" -> '{inputChar}'";
         
            accumulator.Append(inputChar);

            if (i == values.Length - 1)
            {
                yield return new AcceptingStateTransition(nextState, TokenizerAction.Emit, TokenType.Keyword);
            }
            else
            {
                yield return new StateTransition(nextState, TokenizerAction.Read);
            }
        }
    }
}
