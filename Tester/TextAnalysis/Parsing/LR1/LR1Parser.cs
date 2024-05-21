using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Language.Extensions;
using ModularSystem.Core.TextAnalysis.Parsing.Components;
using ModularSystem.Core.TextAnalysis.Parsing.LR1.Components;
using ModularSystem.Core.TextAnalysis.Tokenization;

namespace ModularSystem.Core.TextAnalysis.Parsing;

/// <summary>
/// Represents a LR(1) parser. 
/// </summary>
public class LR1Parser
{
    private static readonly TokenType[] DefaultIgnoreSet = new TokenType[]
    {
        TokenType.Comment
    };

    private LR1ParsingTable ParsingTable { get; }
    private TokenType[]? TokenIgnoreSet { get; }

    /// <summary>
    /// Creates a new instance of <see cref="LR1Parser"/>. It automatically transforms the grammar to LR(1) and creates a parsing table.
    /// </summary>
    /// <param name="grammar"> The grammar to parse. </param>
    /// <param name="tokenIgnoreSet"> The set of tokens to ignore. </param>
    public LR1Parser(Grammar grammar, TokenType[]? tokenIgnoreSet = null)
    {
        grammar.AutoTransformLR1();
        ParsingTable = LR1ParsingTable.Create(grammar);
        TokenIgnoreSet = tokenIgnoreSet ?? DefaultIgnoreSet;
    }

    /// <summary>
    /// Creates a new instance of <see cref="LR1Parser"/>. It uses the provided parsing table.
    /// </summary>
    /// <param name="parsingTable"> The parsing table to use. </param>
    /// <param name="tokenIgnoreSet"> The set of tokens to ignore. </param>
    public LR1Parser(LR1ParsingTable parsingTable, TokenType[]? tokenIgnoreSet = null)
    {
        ParsingTable = parsingTable;
        TokenIgnoreSet = tokenIgnoreSet ?? DefaultIgnoreSet;
    }

    public CstNode Parse(string text)
    {
        using var inputStream = new InputStream(
            input: text, 
            tokenizer: Tokenizer.Instance,
            ignoreSet: TokenIgnoreSet
        );

        var stack = new LR1Stack(
            useDebug: true
        );

        var context = new LR1Context(
            inputStream: inputStream, 
            stack: stack
        );
        
        //* pushes the initial state onto the stack
        stack.PushState(0);

        while (true)
        {
            LR1Action action = GetNextAction(context);

            ExecuteAction(context, action);

            if (action.Type == LR1ActionType.Accept)
            {
                break;
            }            
        }

        return context.CstBuilder.Build();
    }

    /// <summary>
    /// Gets the next action to execute by performing a lookup in the parsing parsingTable using the current state and the lookahead token.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private LR1Action GetNextAction(LR1Context context)
    {
        var currentState = context.Stack.PeekState();
        var lookahead = context.InputStream.Lookahead;

        if(lookahead is null)
        {
            throw context.UnexpectedEndOfTokens();
        }

        var action = ParsingTable.Lookup(currentState, lookahead);

        if(action is null)
        {
            throw context.SyntaxError();
        }

        return action;
    }

    /// <summary>
    /// Dynamically executes the action based on its type.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="action"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private void ExecuteAction(LR1Context context,  LR1Action action)
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

    private void Shift(LR1Context context, LR1ShiftAction action)
    {
        if(context.InputStream.Lookahead is null)
        {
            throw context.UnexpectedEndOfTokens();
        }

        var terminal = context.InputStream.Lookahead;

        context.Stack.PushSymbol(terminal);
        context.Stack.PushState(action.NextState);
        context.InputStream.Consume();
        context.CstBuilder.AddTerminal(terminal);
    }

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

    private void NormalReduce(LR1Context context, ProductionRule production)
    {
        for (int i = 0; i < production.Body.Length; i++)
        {
            context.Stack.PopState();
            context.Stack.PopSymbol();
        }

        var nonTerminal = production.Head;

        context.Stack.PushSymbol(nonTerminal);

        var currentState = context.Stack.PeekState();
        var gotoAction = ParsingTable.LookupGoto(currentState, nonTerminal);

        if (gotoAction is null)
        {
            throw context.SyntaxError();
        }

        context.Stack.PushState(gotoAction.NextState);
        context.CstBuilder.Reduce(nonTerminal, production.Body.Length);
    }

    private void EpsilonReduce(LR1Context context, ProductionRule production)
    {
        var nonTerminal = production.Head;

        context.Stack.PushSymbol(nonTerminal);

        var currentState = context.Stack.PeekState();
        var gotoAction = ParsingTable.LookupGoto(currentState, nonTerminal);

        if (gotoAction is null)
        {
            throw context.SyntaxError();
        }

        context.Stack.PushState(gotoAction.NextState);
        context.CstBuilder.AddEpsilon();
        //context.CstBuilder.Reduce(nonTerminal, production.Body.Length);
    }

    private void Goto(LR1Context context, LR1GotoAction gotoAction)
    {
        context.Stack.PushState(gotoAction.NextState);
    }

    private void Accept(LR1Context context, LR1AcceptAction action)
    {
        context.Stack.PopState();
        context.Stack.PopSymbol();
    }

}