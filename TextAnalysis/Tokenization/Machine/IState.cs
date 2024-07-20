namespace ModularSystem.TextAnalysis.Tokenization.Machine;

public interface IState
{
    ITransitionResult GetStateTransition(char? c);
}
