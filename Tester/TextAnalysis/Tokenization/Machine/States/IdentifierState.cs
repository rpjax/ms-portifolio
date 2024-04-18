using ModularSystem.Core.TextAnalysis.Tokenization.Components;

namespace ModularSystem.Core.TextAnalysis.Tokenization.Machine;

public class IdentifierState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        if (c is null)
        {
            return new TokenResult(TokenizerState.Initial, TokenType.Identifier);
        }

        var isValidChar = false 
            || TokenizerAlphabet.IsDigitOrLetter(c.Value) 
            || TokenizerAlphabet.IsUnderline(c.Value)
            || c == '′'
            ;

        if (isValidChar)
        {
            return new TransitionResult(TokenizerState.None, TokenizerAction.Read);
        }

        return new TokenResult(TokenizerState.Initial, TokenType.Identifier);
    }
}
