namespace ModularSystem.Core.TextAnalysis.Tokenization.Machine;

public class StringEndState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        return new TokenResult(TokenizerState.Initial, TokenType.String);
    }
}