namespace ModularSystem.Core.TextAnalysis.Tokenization.Machine;

public class IntegerNumberState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        switch (c)
        {
            case null:
                return new TokenResult(TokenizerState.Initial, TokenType.Integer);

            case '.':
                return new TransitionResult(TokenizerState.FloatNumber, TokenizerAction.Read);

            default:
                if (char.IsDigit(c.Value))
                {
                    return new TransitionResult(TokenizerState.None, TokenizerAction.Read);
                }

                return new TokenResult(TokenizerState.Initial, TokenType.Integer);
        }
    }
}
