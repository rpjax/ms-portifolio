namespace ModularSystem.Webql.Analysis.Tokenization.Machine;

public class StringState : IState
{
    bool IsCloseCharFound { get; set; }
    char LastChar { get; set; }
    char Delimiter { get; set; }

    public StringState(char delimiter)
    {
        Delimiter = delimiter;
    }

    public ITransitionResult GetStateTransition(char? c)
    {
        if(c is null)
        {
            return new TransitionResult(this, TokenizerAction.Error);
        }
        if (IsCloseCharFound)
        {
            return new TokenResult(new InitialState(), TokenType.String);
        }

        var isDelimiter = c == Delimiter;
        var isEscaped = LastChar == LexicalAlphabet.Escape;

        if (isDelimiter && !isEscaped)
        {
            IsCloseCharFound = true;
            return new TransitionResult(this, TokenizerAction.Read);
        }

        else
        {
            LastChar = c.Value;
            return new TransitionResult(this, TokenizerAction.Read);
        }
    }
}