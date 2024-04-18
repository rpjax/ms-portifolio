using ModularSystem.Core.TextAnalysis.Tokenization.Components;

namespace ModularSystem.Core.TextAnalysis.Tokenization.Machine;

public class SignState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        if(c is null)
        {
            return new TransitionResult(TokenizerState.Punctuation, TokenizerAction.None);
        }

        if (TokenizerAlphabet.IsDigit(c.Value))
        {
            return new TransitionResult(TokenizerState.IntegerNumber, TokenizerAction.Read);
        }

        return new TransitionResult(TokenizerState.Punctuation, TokenizerAction.None);
    }
}
