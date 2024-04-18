namespace ModularSystem.Core.TextAnalysis.Tokenization.Machine;

public class NumberZeroState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        switch (c)
        {
            case null:
                return new TokenResult(TokenizerState.Initial, TokenType.Integer);

            case '.':
                return new TransitionResult(TokenizerState.FloatNumber, TokenizerAction.Read);

            case 'x':
            case 'X':
                return new TransitionResult(TokenizerState.HexadecimalNumberStart, TokenizerAction.Read);

            //case 'b':
            //case 'B':
            //    return new TransitionResult(TokenizerState.BinaryNumber, TokenizerAction.Read);

            default:
                if (char.IsDigit(c.Value))
                {
                    return new TransitionResult(TokenizerState.IntegerNumber, TokenizerAction.Read);
                }

                return new TokenResult(TokenizerState.Initial, TokenType.Integer);
        }
    }
}
