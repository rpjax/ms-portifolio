namespace ModularSystem.Webql.Analysis.Tokenization.Machine;

public class IntegerNumberState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        if(c is null)
        {
            return new TokenResult(new InitialState(), TokenType.Integer);
        }

        if(char.IsDigit(c.Value))
        {
            return new TransitionResult(this, TokenizerAction.Read);
        }
        if (c == '.')
        {
            return new TransitionResult(new FloatNumberState(), TokenizerAction.Read);
        }

        return new TokenResult(new InitialState(), TokenType.Integer);
    }
}
