using ModularSystem.Webql.Analysis.Tokenization.Machine;

namespace ModularSystem.Webql.Analysis.Tokenization;

public class Tokenizer
{
    public IState InitialState { get; }

    public Tokenizer()
    {
        InitialState = new InitialState();
    }

    public IEnumerable<Token> Tokenize(IEnumerable<char> source)
    {
        var context = new LexicalContext(source)
            .Init();

       var state = InitialState;

        while (true)
        {
            var transition = state.GetStateTransition(context.InputChar);

            switch (transition.Action)
            {
                case TokenizerAction.None:
                    break;

                case TokenizerAction.Read:
                    OnRead(context);
                    break;

                case TokenizerAction.Skip:
                    OnSkip(context);
                    break;

                case TokenizerAction.Emit:
                    yield return OnEmit(context, transition);
                    break;

                case TokenizerAction.Error:
                    OnError(context);
                    break;

                case TokenizerAction.End:
                    yield break;
            }

            state = transition.NextState;    
        }
    }

    private void OnRead(LexicalContext context)
    {
        context.Read();
    }

    private void OnSkip(LexicalContext context)
    {
        context.Skip();
    }

    private Token OnEmit(LexicalContext context, ITransitionResult transition)
    {
        if (transition is not ITokenResult acceptingState)
        {
            throw new Exception();
        }

        var token = new Token(
            tokenType: acceptingState.TokenType,
            value: context.Accumulator,
            metadata: context.GetMetadata()
        );

        context.ResetAccumulator();
        return token;
    }

    private void OnError(LexicalContext context)
    {
        var message = $"Unexpected character '{context.InputChar}' at line {context.Line}, column {context.Column}.";

        throw new Exception(message);
    }
}

