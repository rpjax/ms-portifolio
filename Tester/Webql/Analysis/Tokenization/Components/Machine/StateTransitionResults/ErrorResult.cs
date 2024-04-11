namespace ModularSystem.Webql.Analysis.Tokenization;

public class ErrorResult : ITransitionResult
{
    public IState NextState { get; }
    public TokenizerAction Action { get; }

    public ErrorResult()
    {
        NextState = default!;
        Action = TokenizerAction.Error;
    }
}
