namespace ModularSystem.Webql.Analysis.Tokenization;

public interface ITransitionResult
{
    IState NextState { get; }
    TokenizerAction Action { get; }
}
