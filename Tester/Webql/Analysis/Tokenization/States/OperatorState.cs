using System.Text;

namespace ModularSystem.Webql.Analysis.Tokenization.Machine;

public class OperatorState : IState
{
    private StringBuilder Accumulator { get; }

    public OperatorState(char c)
    {
        Accumulator = new StringBuilder();
        Accumulator.Append(c);
    }

    public ITransitionResult GetStateTransition(char? c)
    {
        if(c is null)
        {
            if(LexicalAlphabet.Operators.Contains(Accumulator.ToString()))
            {
                return new TokenResult(new InitialState(), TokenType.Operator);
            }

            return new TransitionResult(this, TokenizerAction.Error);
        }

        Accumulator.Append(c.Value);

        if(LexicalAlphabet.Operators.Contains(Accumulator.ToString()))
        {
            return new TransitionResult(this, TokenizerAction.Read);         
        }
        else
        {
            return new TokenResult(new InitialState(), TokenType.Operator);
        }
    }
}
