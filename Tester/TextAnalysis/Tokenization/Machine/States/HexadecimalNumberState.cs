using ModularSystem.Core.TextAnalysis.Tokenization.Components;

namespace ModularSystem.Core.TextAnalysis.Tokenization.Machine;

public class HexadecimalNumberStartState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        switch (c)
        {
            case null:
                return new ErrorResult("Invalid hexadecimal literal, expected at least one value char.");

            default:
                if (!TokenizerAlphabet.IsHexadecimal(c.Value))
                {
                    return new ErrorResult("Invalid hexadecimal literal.");
                }

                return new TransitionResult(TokenizerState.HexadecimalNumber, TokenizerAction.Read);
        }
    }
}

public class HexadecimalNumberState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        switch (c)
        {
            case null:
                return new TokenResult(TokenizerState.Initial, TokenType.Hexadecimal);

            default:
                if (TokenizerAlphabet.IsHexadecimal(c.Value))
                {
                    return new TransitionResult(TokenizerState.None, TokenizerAction.Read);
                }

                return new TokenResult(TokenizerState.Initial, TokenType.Hexadecimal);
        }
    }
}
