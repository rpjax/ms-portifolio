using ModularSystem.Webql.Analysis.Tokenization;

namespace ModularSystem.Webql.Analysis.Tokenization;

public enum TokenizerAction
{
    /// <summary>
    /// No action, just transitions to the next state.
    /// </summary>
    None,

    /// <summary>
    /// Appends the current character to the accumulator and transitions to the next state.
    /// </summary>
    Read,

    /// <summary>
    /// Skips the current character and transitions to the next state.
    /// </summary>
    Skip,

    /// <summary>
    /// Emits a token and transitions to the next state.
    /// </summary>
    Emit,

    /// <summary>
    /// Emits an error token and stops the tokenization process.
    /// </summary>
    Error,

    /// <summary>
    /// Emits a token and stops the tokenization process.
    /// </summary>
    End,
}

public enum CharType
{
    Epsilon,
    Digit,
    Letter,
    Punctuation,
    StringDelimiter,
    Operator,
    Whitespace,
    Other,
}

//*
//* first aproach
//*

public interface IStateTransition
{
    string NextState { get; }
    TokenizerAction Action { get; }
}

public interface IAcceptingStateTransition : IStateTransition
{
    TokenType TokenType { get; }
}

public class StateTransition : IStateTransition
{
    public string NextState { get; }
    public TokenizerAction Action { get; }

    public StateTransition(string nextState, TokenizerAction action)
    {
        NextState = nextState;
        Action = action;
    }
}

public class AcceptingStateTransition : IAcceptingStateTransition
{
    public string NextState { get; }
    public TokenizerAction Action { get; }
    public TokenType TokenType { get; }

    public AcceptingStateTransition(string nextState, TokenizerAction action, TokenType tokenType)
    {
        NextState = nextState;
        Action = action;
        TokenType = tokenType;
    }
}

[Obsolete("WIP. Use Tokenizer instead.")]
public class TableBasedTokenizer
{
    private Dictionary<string, IStateTransition> TransitionTable { get; }

    public TableBasedTokenizer()
    {
        TransitionTable = LexicalAlphabet.CreateTransitionTable();
    }

    public static string GetStateKey(string currentState, string? pattern)
    {
        if (pattern is null)
        {
            return $"{currentState} -> 'epsilon'";
        }

        return $"{currentState} -> '{pattern}'";
    }

    public IEnumerable<Token?> Tokenize(IEnumerable<char> source)
    {
        using var context = new LexicalContext(source)
            .Init();

        while (true)
        {
            var stateTransition = GetStateTransition(context);

            switch (stateTransition.Action)
            {
                case TokenizerAction.None:
                    break;

                case TokenizerAction.Read:
                    context.Read();
                    break;

                case TokenizerAction.Skip:
                    context.Skip();
                    break;

                case TokenizerAction.Emit:
                    if (stateTransition is not IAcceptingStateTransition acceptingState)
                    {
                        throw new Exception();
                    }

                    yield return new Token(
                        tokenType: acceptingState.TokenType,
                        value: context.Accumulator,
                        metadata: context.GetMetadata()
                    );

                    context.ResetAccumulator();
                    break;

                case TokenizerAction.Error:
                    throw new NotImplementedException();

                case TokenizerAction.End:
                    yield return null;
                    yield break;

                default:
                    throw new InvalidOperationException();
            }

            context.SetState(stateTransition.NextState);
        }
    }

    private IStateTransition GetStateTransition(LexicalContext context)
    {
        var transition = default(IStateTransition);

        if (TransitionTable.TryGetValue(GetFirstKey(context), out transition))
        {
            return transition;
        }

        if (TransitionTable.TryGetValue(GetSecondKey(context), out transition))
        {
            return transition;
        }

        throw new Exception("Unable to find state transition for the given context.");
    }

    //* explicit match key: "${currentState} -> '${input}'"
    private string GetFirstKey(LexicalContext context)
    {
        return GetStateKey(context.CurrentState, context.InputString);
    }

    private string GetSecondKey(LexicalContext context)
    {
        return context.CurrentStateInput;
    }

    private CharType GetCharType(char c)
    {
        if (char.IsDigit(c))
        {
            return CharType.Digit;
        }

        if (char.IsLetter(c))
        {
            return CharType.Letter;
        }

        if (char.IsWhiteSpace(c))
        {
            return CharType.Whitespace;
        }

        if (LexicalAlphabet.Punctuations.Contains(c))
        {
            return CharType.Punctuation;
        }

        if (LexicalAlphabet.StringDelimiters.Contains(c))
        {
            return CharType.StringDelimiter;
        }

        return CharType.Other;
    }

    private string[] GetInputPatterns(LexicalContext context)
    {
        var patterns = new List<string>();

        var state = context.CurrentState;
        var c = context.InputChar;
        var input = context.InputString;

        if(c is null)
        {
            patterns.Add(GetStateKey(state, null));
            return patterns.ToArray();  
        }

        if (char.IsDigit(c.Value))
        {
            patterns.Add(GetStateKey(state, "digit"));
            patterns.Add(GetStateKey(state, "not letter"));
        }

        throw new NotImplementedException();
    }

}
