namespace Core.TextAnalysis;

// NOTE:
// - strings should be pre-allocated always.
// - source code generation overhead is prefered over regexes.
// - the parser should be totally deterministic and context free.
public abstract class TokenProduction
{
    public string JoinedFormationStrings => string.Join(", ", GetFormationStrings());

    public abstract string[] GetFormationStrings();
    public abstract LexerToken GetToken(LexerSentense sentense);
}

// <digit> ::= ...
// <letter> ::= ...
// <identifier> ::= ...
// <number> ::=
// <operator> ::= ...
// <term> ::= <identifier> | <number>
// <expression> ::= <term> | <expression> "+" <term>
// "5 + 5" => "<digit><ignore><operator><ignore><digit>"
// 4 * sizeof(char) | (4 * sizeof(char)) + (4 * sizeof(object))
// "1024" => "<digit><digit><digit><digit>" => "<number>"
