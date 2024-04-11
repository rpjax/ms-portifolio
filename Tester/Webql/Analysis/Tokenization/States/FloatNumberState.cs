namespace ModularSystem.Webql.Analysis.Tokenization.Machine;

public class FloatNumberState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        if (c is null)
        {
            return new TokenResult(new InitialState(), TokenType.Float);
        }

        if (char.IsDigit(c.Value))
        {
            return new TransitionResult(this, TokenizerAction.Read);
        }
       
        return new TokenResult(new InitialState(), TokenType.Float);
    }
}
