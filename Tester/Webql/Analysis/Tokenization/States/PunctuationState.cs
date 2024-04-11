namespace ModularSystem.Webql.Analysis.Tokenization.Machine;

public class PunctuationState : IState
{
    public ITransitionResult GetStateTransition(char? c)
    {
        return new TokenResult(new InitialState(), TokenType.Punctuation);
    }
}