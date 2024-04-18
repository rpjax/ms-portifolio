namespace ModularSystem.Core.TextAnalysis.Tokenization.Machine;

public class PunctuationState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        return new TokenResult(TokenizerState.Initial, TokenType.Punctuation);
    }
}
