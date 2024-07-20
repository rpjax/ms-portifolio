namespace ModularSystem.Core.TextAnalysis.Tokenization.Machine;

public interface ITransitionResult
{
    TokenizerState NextState { get; }
    TokenizerAction Action { get; }
}
