namespace ModularSystem.Webql.Analysis.Tokenization;

public interface ITokenResult : ITransitionResult
{
    TokenType TokenType { get; }
}
