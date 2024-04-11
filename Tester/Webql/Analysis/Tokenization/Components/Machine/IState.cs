namespace ModularSystem.Webql.Analysis.Tokenization;

public interface IState
{
    ITransitionResult GetStateTransition(char? c);
}
