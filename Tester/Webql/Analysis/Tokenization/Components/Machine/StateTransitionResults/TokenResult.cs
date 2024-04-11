namespace ModularSystem.Webql.Analysis.Tokenization;

public class TokenResult : ITokenResult
{
    public IState NextState { get; }
    public TokenizerAction Action { get; }
    public TokenType TokenType { get; }

    public TokenResult(IState nextState, TokenType tokenType)
    {
        NextState = nextState;
        Action = TokenizerAction.Emit;
        TokenType = tokenType;
    }
}
