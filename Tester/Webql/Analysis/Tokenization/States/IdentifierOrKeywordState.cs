using System.Text;

namespace ModularSystem.Webql.Analysis.Tokenization.Machine;

public class IdentifierOrKeywordState : IState
{
    private StringBuilder Accumulator { get; } = new StringBuilder();

    public IdentifierOrKeywordState(char c)
    {
        Accumulator.Append(c);
    }

    public ITransitionResult GetStateTransition(char? c)
    {
        if(c is null)
        {
            if (LexicalAlphabet.Keywords.Contains(Accumulator.ToString()))
            {
                return new TokenResult(new InitialState(), TokenType.Keyword);
            }

            return new TokenResult(new InitialState(), TokenType.Identifier);
        }

        var isValid_IdentifierOrKeyword_Char = char.IsLetterOrDigit(c.Value) || c == LexicalAlphabet.Underline;

        if (isValid_IdentifierOrKeyword_Char)
        {
            Accumulator.Append(c.Value);
            return new TransitionResult(this, TokenizerAction.Read);
        }
        else
        {
            if (LexicalAlphabet.Keywords.Contains(Accumulator.ToString()))
            {
                if(Accumulator.ToString() == "null")
                {
                    return new TokenResult(new InitialState(), TokenType.Null);
                }

                if(Accumulator.ToString() == "true" || Accumulator.ToString() == "false")
                {
                    return new TokenResult(new InitialState(), TokenType.Boolean);
                }

                return new TokenResult(new InitialState(), TokenType.Keyword);
            }

            return new TokenResult(new InitialState(), TokenType.Identifier);
        }
    }
}
