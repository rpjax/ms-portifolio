using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Language.Extensions;
using ModularSystem.Core.TextAnalysis.Parsing.Components;
using ModularSystem.Core.TextAnalysis.Parsing.LR1.Components;
using ModularSystem.Core.TextAnalysis.Tokenization;
using System.Runtime.CompilerServices;

namespace ModularSystem.Core.TextAnalysis.Parsing;

/// <summary>
/// Represents a LR(1) parser. It is capable of parsing text based on a given grammar.
/// </summary>
public class LR1Parser
{
    private static readonly TokenType[] DefaultIgnoreSet = new TokenType[]
    {
        TokenType.Comment
    };

    private Grammar Grammar { get; }
    private LR1ParsingTable ParsingTable { get; }
    private TokenType[]? TokenIgnoreSet { get; }

    /// <summary>
    /// Creates a new instance of <see cref="LR1Parser"/>. It automatically transforms the grammar to LR(1) and creates a parsing table.
    /// </summary>
    /// <param name="grammar"> The grammar to parse. </param>
    /// <param name="tokenIgnoreSet"> The set of tokens to ignore. </param>
    public LR1Parser(Grammar grammar, TokenType[]? tokenIgnoreSet = null)
    {
        Grammar = grammar.AutoTransformLR1();
        ParsingTable = LR1ParsingTable.Create(Grammar);
        TokenIgnoreSet = tokenIgnoreSet ?? DefaultIgnoreSet;
    }

    /// <summary>
    /// Parses the given text and returns the concrete syntax tree (CST) based on the grammar specified in the constructor.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public CstRoot Parse(string text)
    {
        using var inputStream = new InputStream(
            input: text,
            tokenizer: Tokenizer.Instance,
            ignoreSet: TokenIgnoreSet
        );

        var stack = new LR1Stack(
            //useDebug: false
        );

        var cstBuilder = new CstBuilder(
            useEpsilons: false
        );

        var context = new LR1Context(
            parsingTable: ParsingTable,
            inputStream: inputStream,
            stack: stack,
            cstBuilder: cstBuilder
        );

        //* pushes the initial state onto the stack
        stack.PushState(0);

        while (true)
        {
            var action = GetNextAction(context);

            ExecuteAction(context, action);

            if (action.Type == LR1ActionType.Accept)
            {
                break;
            }
        }

        return context.CstBuilder.Build();
    }

    /// <summary>
    /// Gets the next action to execute by performing a lookup in the parsing parsing table using the current state and the lookahead token.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private LR1Action GetNextAction(LR1Context context)
    {
        var currentState = context.Stack.PeekState();
        var lookahead = context.InputStream.LookaheadToken;

        if (currentState == -1)
        {
            throw new Exception("Invalid state.");
        }

        if (lookahead is null)
        {
            throw context.UnexpectedEndOfTokens();
        }

        var action = ParsingTable.Lookup(currentState, lookahead);

        if (action is null)
        {
            throw context.SyntaxException(currentState, lookahead);
        }

        return action;
    }

    /// <summary>
    /// Dynamically executes the action based on its type.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="action"></param>
    /// <exception cref="InvalidOperationException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ExecuteAction(LR1Context context, LR1Action action)
    {
        switch (action.Type)
        {
            case LR1ActionType.Shift:
                Shift(context, action.AsShift());
                return;

            case LR1ActionType.Reduce:
                Reduce(context, action.AsReduce());
                return;

            case LR1ActionType.Goto:
                Goto(context, action.AsGoto());
                break;

            case LR1ActionType.Accept:
                Accept(context, action.AsAccept());
                break;

            default:
                throw new InvalidOperationException();
        }
    }

    /// <summary>
    /// Shifts the lookahead token onto the stack and consumes it from the input stream.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="action"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Shift(LR1Context context, LR1ShiftAction action)
    {
        var token = context.InputStream.LookaheadToken;

        if (token is null)
        {
            throw context.UnexpectedEndOfTokens();
        }

        context.Stack.PushToken(token);
        context.Stack.PushState(action.NextState);
        context.CstBuilder.AddTerminal(token);
        context.InputStream.Consume();
    }

    /// <summary>
    /// Reduces the stack based on the production rule specified in the action.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="reduceAction"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Reduce(LR1Context context, LR1ReduceAction reduceAction)
    {
        var production = ParsingTable.GetProduction(reduceAction.ProductionIndex);

        if (production.IsEpsilonProduction)
        {
            EpsilonReduce(context, production);
        }
        else
        {
            NormalReduce(context, production);
        }
    }

    /// <summary>
    /// Reduces the stack based on a normal production rule.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="production"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void NormalReduce(LR1Context context, ProductionRule production)
    {
        for (int i = 0; i < production.Body.Length * 2; i++)
        {
            context.Stack.Pop();
        }

        var nonTerminal = production.Head;
        var currentState = context.Stack.PeekState();

        if (currentState == -1)
        {
            throw new Exception("Invalid state.");
        }

        context.Stack.PushNonTerminal(nonTerminal);

        var gotoAction = ParsingTable.LookupGoto(currentState, nonTerminal);

        if (gotoAction is null)
        {
            throw new InvalidOperationException();
        }

        var nextState = gotoAction.NextState;

        context.Stack.PushState(nextState);
        context.CstBuilder.Reduce(
            nonTerminal: nonTerminal,
            length: production.Body.Length,
            isRoot: nextState == 1
        );
    }

    /// <summary>
    /// Reduces the stack based on an epsilon production rule.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="production"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EpsilonReduce(LR1Context context, ProductionRule production)
    {
        var nonTerminal = production.Head;
        var currentState = context.Stack.PeekState();

        if(currentState == -1)
        {
            throw new Exception("Invalid state.");
        }

        context.Stack.PushNonTerminal(nonTerminal);

        var gotoAction = ParsingTable.LookupGoto(currentState, nonTerminal);

        if (gotoAction is null)
        {
            throw new InvalidOperationException();
        }

        context.Stack.PushState(gotoAction.NextState);
        context.CstBuilder.ReduceEpsilon(
            nonTerminal: nonTerminal
        );
    }

    /// <summary>
    /// Goes to the next state based on the goto action.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="gotoAction"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Goto(LR1Context context, LR1GotoAction gotoAction)
    {
        context.Stack.PushState(gotoAction.NextState);
    }

    /// <summary>
    /// Accepts the input.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="action"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Accept(LR1Context context, LR1AcceptAction action)
    {
        context.Stack.Pop();
    }

}