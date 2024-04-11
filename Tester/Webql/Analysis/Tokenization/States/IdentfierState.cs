namespace ModularSystem.Webql.Analysis.Tokenization.Machine;

public class IdentfierState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        if(c is null)
        {
            return new TokenResult(new InitialState(), TokenType.Identifier);
        }

        if (char.IsLetterOrDigit(c.Value) || c == LexicalAlphabet.Underline)
        {
            return new TransitionResult(this, TokenizerAction.Read);
        }
        else
        {
            return new TokenResult(new InitialState(), TokenType.Identifier);
        }
    }
}
