using System.Runtime.CompilerServices;

namespace ModularSystem.Core.TextAnalysis.Tokenization.Machine;

public class PunctuationState : IState
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ITransitionResult GetStateTransition(char? c)
    {
        return new TokenResult(TokenizerState.Initial, TokenType.Punctuation);
    }
}
