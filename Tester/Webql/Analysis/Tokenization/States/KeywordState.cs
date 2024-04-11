using System.Text;

namespace ModularSystem.Webql.Analysis.Tokenization.Machine;

public class KeywordState : IState
{
    private StringBuilder Accumulator { get; } = new StringBuilder();

    public KeywordState(char c)
    {
        Accumulator.Append(c);
    }

    public ITransitionResult GetStateTransition(char? c)
    {
        if (c is null)
        {
            if (LexicalAlphabet.Keywords.Contains(Accumulator.ToString()))
            {
                return new TokenResult(new InitialState(), TokenType.Keyword);
            }

            return new TokenResult(new InitialState(), TokenType.Identifier);
        }

        Accumulator.Append(c.Value);

        throw new System.NotImplementedException();
    }
}