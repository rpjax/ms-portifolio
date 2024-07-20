namespace ModularSystem.Core.TextAnalysis.Tokenization.Machine;

public interface ITokenResult : ITransitionResult
{
    TokenType TokenType { get; }
}
