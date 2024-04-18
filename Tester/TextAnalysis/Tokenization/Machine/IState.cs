namespace ModularSystem.Core.TextAnalysis.Tokenization.Machine;

public interface IState
{
    ITransitionResult GetStateTransition(char? c);
}
