namespace ModularSystem.Webql.Analysis.Tokenization;

public class TransitionResult : ITransitionResult
{
    public IState NextState { get; }
    public TokenizerAction Action { get; }

    public TransitionResult(IState nextState, TokenizerAction action)
    {
        NextState = nextState;
        Action = action;
    }
}
