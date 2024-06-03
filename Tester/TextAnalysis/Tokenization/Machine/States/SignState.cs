using ModularSystem.Core.TextAnalysis.Tokenization.Components;
using System.Runtime.CompilerServices;

namespace ModularSystem.Core.TextAnalysis.Tokenization.Machine;

public class SignState : IState
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
