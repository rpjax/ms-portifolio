namespace ModularSystem.Core.TextAnalysis.Tokenization.Machine;

public class TokenResult : ITokenResult
{
    public TokenizerState NextState { get; }
    public TokenizerAction Action { get; }
    public TokenType TokenType { get; }

    public TokenResult(TokenizerState nextState, TokenType tokenType)
    {
        NextState = nextState;
        Action = TokenizerAction.Emit;
        TokenType = tokenType;
    }
}
