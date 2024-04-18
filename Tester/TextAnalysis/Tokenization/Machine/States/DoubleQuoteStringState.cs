using ModularSystem.Core.TextAnalysis.Tokenization.Components;

namespace ModularSystem.Core.TextAnalysis.Tokenization.Machine;

public class DoubleQuoteStringState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        if (c is null)
        {
            return new TransitionResult(TokenizerState.None, TokenizerAction.Error);
        }

        if (c == TokenizerAlphabet.Escape)
        {
            return new TransitionResult(TokenizerState.DoubleQuoteStringEscape, TokenizerAction.Skip);
        }

        if (c == '"')
        {
            return new TransitionResult(TokenizerState.StringEnd, TokenizerAction.Read);
        }

        return new TransitionResult(TokenizerState.None, TokenizerAction.Read);
    }
}

public class DoubleQuoteStringEscapeState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        if (c is null)
        {
            return new TransitionResult(TokenizerState.None, TokenizerAction.Error);
        }

        return new TransitionResult(TokenizerState.DoubleQuoteString, TokenizerAction.Read);
    }
}
