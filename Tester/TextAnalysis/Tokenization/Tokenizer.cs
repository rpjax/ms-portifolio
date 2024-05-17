using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Tokenization.Components;
using ModularSystem.Core.TextAnalysis.Tokenization.Machine;

namespace ModularSystem.Core.TextAnalysis.Tokenization;

public class Tokenizer
{
    /*
     * Properties Section: Defines the various states used within the tokenizer.
     * 
     * Design Principles of Tokenizer States:
     * - Each state is encapsulated in a class that implements the IState interface.
     * - States are stateless, meaning they do not hold or manage any internal state between method invocations.
     * - Data required by states must be transient, passed only via method parameters. For managing state transitions and complex data flows, a 'context object' is utilized, which is propagated through the states.
     * - The tokenizer dynamically transitions between states based on the character input stream. Each state is designed to handle specific segments of the parsing process, such as numbers, identifiers, or string literals.
     * - States should primarily perform simple checks on the input stream, such as querying whether the current character is a digit or a letter.
     * - For each check, the state returns a transition object detailing the next state and the action required. This approach resembles a lookup table where the current state and input character determine the next state and action.
     * - Complex token patterns should be decomposed into simpler, more manageable states. For instance, a string state may include sub-states for managing escape sequences or varying types of string delimiters (single or double quotes).
     * 
     * All token patterns should be deterministic within two characters. This ensures that the tokenizer can predict the next state and action based on the current state and the upcoming character in the input stream.
     * For example, comment tokens are often initiated by sequences like "//" or "/*".
     * Consider that the tokenizer classifies "/" as punctuation.
     * To derive the string "/#" from an initial state S, the tokenizer transitions to an intermediary state A upon encountering "/", and then either to a comment state C if the next character is another "/" or "*", or to a different state B if not. TargetState B, which handles punctuation, emits a token for the "/" character and then returns to the initial state S with "#" as the next character.
     * 
     *      S -> A (initial state -> intermediary comment state)
     *      A -> C (intermediary comment state -> comment state)
     *      A -> B (intermediary comment state -> punctuation state)
     *      B -> S (punctuation state -> initial state) *emits punctuation token
     *      
     * Observation: The information that the previous character was a "/" is not required for the transition from state A to state B, as the tokenizer only needs to know the current state and the next character to determine the next state and action. TargetState B did not consume the subsequent character "#" but instead emitted a token for the "/" character and returned to the initial state S.
     */

    public static Tokenizer Instance { get; } = new();

    private static InitialState InitialState { get; } = new();
    private static NumberZeroState NumberZeroState { get; } = new();
    private static IntegerNumberState IntegerNumberState { get; } = new();
    private static FloatNumberState FloatNumberState { get; } = new();
    private static HexadecimalNumberStartState HexadecimalNumberStartState { get; } = new();
    private static HexadecimalNumberState HexadecimalNumberState { get; } = new();

    private static SignState SignState { get; } = new();

    private static IdentifierState IdentifierState { get; } = new();
    private static PunctuationState PunctuationState { get; } = new();

    private static SingleQuoteStringState SingleQuoteStringState { get; } = new();
    private static SingleQuoteStringEscapeState SingleQuoteStringEscapeState { get; } = new();

    private static DoubleQuoteStringState DoubleQuoteStringState { get; } = new();
    private static DoubleQuoteStringEscapeState DoubleQuoteStringEscapeState { get; } = new();

    private static StringEndState StringEndState { get; } = new();

    private static CppStyleCommentStartState CppStyleCommentStartState { get; } = new();

    private static CppStyleSingleLineCommentState CppStyleSingleLineCommentState { get; } = new();

    private static CppStyleMultiLineCommentState CppStyleMultiLineCommentState { get; } = new();

    private static CppStyleMultiLineCommentEndConfirmState CppStyleMultiLineCommentEndConfirmState { get; } = new();

    private static EbnfStyleMultiLineCommentStartState EbnfStyleMultiLineCommentStartState { get; } = new();

    private static EbnfStyleMultiLineCommentState EbnfStyleMultiLineCommentState { get; } = new();

    private static EbnfStyleMultiLineCommentEndConfirmState EbnfStyleMultiLineCommentEndConfirmState { get; } = new();

    private static CommentEndState CommentEndState { get; } = new();

    public Tokenizer()
    {

    }

    public IEnumerable<Token> Tokenize(
        IEnumerable<char> source,
        bool includeEoi = true)
    {
        var context = new LexicalContext(source)
            .Init();

        var state = InitialState as IState;

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
                    if (includeEoi)
                    {
                        yield return new Token(TokenType.Eoi, "EOI", context.GetMetadata());
                    }
                    yield break;
            }

            switch (transition.NextState)
            {
                case TokenizerState.None:
                    break;

                case TokenizerState.Initial:
                    state = InitialState;
                    break;

                case TokenizerState.NumberZero:
                    state = NumberZeroState;
                    break;

                case TokenizerState.IntegerNumber:
                    state = IntegerNumberState;
                    break;

                case TokenizerState.FloatNumber:
                    state = FloatNumberState;
                    break;

                case TokenizerState.HexadecimalNumberStart:
                    state = HexadecimalNumberStartState;
                    break;

                case TokenizerState.HexadecimalNumber:
                    state = HexadecimalNumberState;
                    break;

                case TokenizerState.Sign:
                    state = SignState;
                    break;

                case TokenizerState.Identifier:
                    state = IdentifierState;
                    break;

                case TokenizerState.Punctuation:
                    state = PunctuationState;
                    break;

                case TokenizerState.SingleQuoteString:
                    state = SingleQuoteStringState;
                    break;

                case TokenizerState.SingleQuoteStringEscape:
                    state = SingleQuoteStringEscapeState;
                    break;

                case TokenizerState.DoubleQuoteString:
                    state = DoubleQuoteStringState;
                    break;

                case TokenizerState.DoubleQuoteStringEscape:
                    state = DoubleQuoteStringEscapeState;
                    break;

                case TokenizerState.StringEnd:
                    state = StringEndState;
                    break;

                case TokenizerState.CppStyleCommentStart:
                    state = CppStyleCommentStartState;
                    break;

                case TokenizerState.CppStyleSingleLineComment:
                    state = CppStyleSingleLineCommentState;
                    break;

                case TokenizerState.CppStyleMultiLineComment:
                    state = CppStyleMultiLineCommentState;
                    break;

                case TokenizerState.CppStyleMultiLineCommentEndConfirm:
                    state = CppStyleMultiLineCommentEndConfirmState;
                    break;

                case TokenizerState.EbnfStyleMultiLineCommentStart:
                    state = EbnfStyleMultiLineCommentStartState;
                    break;

                case TokenizerState.EbnfStyleMultiLineComment:
                    state = EbnfStyleMultiLineCommentState;
                    break;

                case TokenizerState.EbnfStyleMultiLineCommentEndConfirm:
                    state = EbnfStyleMultiLineCommentEndConfirmState;
                    break;

                case TokenizerState.CommentEnd:
                    state = CommentEndState;
                    break;

                default:
                    throw new InvalidOperationException();
            }
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
            throw new Exception($"Unexpected transition type '{transition.GetType().Name}'. Expected ITokenResult.");
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
        throw new Exception($"Unexpected character '{context.InputChar}' at line {context.Line}, column {context.Column}.");
    }
}
