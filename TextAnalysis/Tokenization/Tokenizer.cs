using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Tokenization.Components;
using ModularSystem.Core.TextAnalysis.Tokenization.Machine;
using System.Runtime.CompilerServices;

namespace ModularSystem.Core.TextAnalysis.Tokenization;

public class Tokenizer
{
    /*
     * Properties Section: Defines the various states used within the tokenizer.
     * 
     * Design Principles of Tokenizer States:
     * - Each currentState is encapsulated in a class that implements the IState interface.
     * - States are stateless, meaning they do not hold or manage any internal currentState between method invocations.
     * - Data required by states must be transient, passed only via method parameters. For managing currentState transitions and complex data flows, a 'context object' is utilized, which is propagated through the states.
     * - The tokenizer dynamically transitions between states based on the character input stream. Each currentState is designed to handle specific segments of the parsing process, such as numbers, identifiers, or string literals.
     * - States should primarily perform simple checks on the input stream, such as querying whether the current character is a digit or a letter.
     * - For each check, the currentState returns a transition object detailing the next currentState and the action required. This approach resembles a lookup table where the current currentState and input character determine the next currentState and action.
     * - Complex token patterns should be decomposed into simpler, more manageable states. For instance, a string currentState may include sub-states for managing escape sequences or varying types of string delimiters (single or double quotes).
     * 
     * All token patterns should be deterministic within two characters. This ensures that the tokenizer can predict the next currentState and action based on the current currentState and the upcoming character in the input stream.
     * For example, comment tokens are often initiated by sequences like "//" or "/*".
     * Consider that the tokenizer classifies "/" as punctuation.
     * To derive the string "/#" from an initial currentState S, the tokenizer transitions to an intermediary currentState A upon encountering "/", and then either to a comment currentState C if the next character is another "/" or "*", or to a different currentState B if not. TargetState B, which handles punctuation, emits a token for the "/" character and then returns to the initial currentState S with "#" as the next character.
     * 
     *      S -> A (initial currentState -> intermediary comment currentState)
     *      A -> C (intermediary comment currentState -> comment currentState)
     *      A -> B (intermediary comment currentState -> punctuation currentState)
     *      B -> S (punctuation currentState -> initial currentState) *emits punctuation token
     *      
     * Observation: The information that the previous character was a "/" is not required for the transition from currentState A to currentState B, as the tokenizer only needs to know the current currentState and the next character to determine the next currentState and action. TargetState B did not consume the subsequent character "#" but instead emitted a token for the "/" character and returned to the initial currentState S.
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
        string source,
        bool includeEoi = true)
    {
        var context = new LexicalContext(source);
        var currentState = InitialState as IState;

        while (true)
        {
            var transition = currentState.GetStateTransition(context.CurrentChar);

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
                        yield return new Token(
                            type: TokenType.Eoi, 
                            value: Eoi.SententialRepresentation.AsMemory(), 
                            metadata: context.GetMetadata()
                        );
                    }
                    yield break;
            }

            switch (transition.NextState)
            {
                case TokenizerState.None:
                    break;

                case TokenizerState.Initial:
                    currentState = InitialState;
                    break;

                case TokenizerState.NumberZero:
                    currentState = NumberZeroState;
                    break;

                case TokenizerState.IntegerNumber:
                    currentState = IntegerNumberState;
                    break;

                case TokenizerState.FloatNumber:
                    currentState = FloatNumberState;
                    break;

                case TokenizerState.HexadecimalNumberStart:
                    currentState = HexadecimalNumberStartState;
                    break;

                case TokenizerState.HexadecimalNumber:
                    currentState = HexadecimalNumberState;
                    break;

                case TokenizerState.Sign:
                    currentState = SignState;
                    break;

                case TokenizerState.Identifier:
                    currentState = IdentifierState;
                    break;

                case TokenizerState.Punctuation:
                    currentState = PunctuationState;
                    break;

                case TokenizerState.SingleQuoteString:
                    currentState = SingleQuoteStringState;
                    break;

                case TokenizerState.SingleQuoteStringEscape:
                    currentState = SingleQuoteStringEscapeState;
                    break;

                case TokenizerState.DoubleQuoteString:
                    currentState = DoubleQuoteStringState;
                    break;

                case TokenizerState.DoubleQuoteStringEscape:
                    currentState = DoubleQuoteStringEscapeState;
                    break;

                case TokenizerState.StringEnd:
                    currentState = StringEndState;
                    break;

                case TokenizerState.CppStyleCommentStart:
                    currentState = CppStyleCommentStartState;
                    break;

                case TokenizerState.CppStyleSingleLineComment:
                    currentState = CppStyleSingleLineCommentState;
                    break;

                case TokenizerState.CppStyleMultiLineComment:
                    currentState = CppStyleMultiLineCommentState;
                    break;

                case TokenizerState.CppStyleMultiLineCommentEndConfirm:
                    currentState = CppStyleMultiLineCommentEndConfirmState;
                    break;

                case TokenizerState.EbnfStyleMultiLineCommentStart:
                    currentState = EbnfStyleMultiLineCommentStartState;
                    break;

                case TokenizerState.EbnfStyleMultiLineComment:
                    currentState = EbnfStyleMultiLineCommentState;
                    break;

                case TokenizerState.EbnfStyleMultiLineCommentEndConfirm:
                    currentState = EbnfStyleMultiLineCommentEndConfirmState;
                    break;

                case TokenizerState.CommentEnd:
                    currentState = CommentEndState;
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnRead(LexicalContext context)
    {
        context.Consume();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnSkip(LexicalContext context)
    {
        context.Skip();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Token OnEmit(LexicalContext context, ITransitionResult transition)
    {
        if (transition is not ITokenResult acceptingState)
        {
            throw new Exception($"Unexpected transition type '{transition.GetType().Name}'. Expected ITokenResult.");
        }

        var token = new Token(
            type: acceptingState.TokenType,
            value: context.AccumulatorValue,
            metadata: context.GetMetadata()
        );

        context.ResetAccumulator();
        return token;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnError(LexicalContext context)
    {
        throw new Exception($"Unexpected character '{context.CurrentChar}' at line {context.Line}, column {context.Column}.");
    }
}
