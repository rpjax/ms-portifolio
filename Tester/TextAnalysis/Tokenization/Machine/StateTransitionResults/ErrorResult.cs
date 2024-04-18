namespace ModularSystem.Core.TextAnalysis.Tokenization.Machine;

public class ErrorResult : ITransitionResult
{
    public TokenizerState NextState { get; }
    public TokenizerAction Action { get; }
    public Error Error { get; }

    public ErrorResult(Error error)
    {
        NextState = TokenizerState.None;
        Action = TokenizerAction.Error;
        Error = error;
    }
}
