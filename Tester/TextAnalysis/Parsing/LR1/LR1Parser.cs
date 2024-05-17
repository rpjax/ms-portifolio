using ModularSystem.Core.TextAnalysis.Language.Components;
using ModularSystem.Core.TextAnalysis.Parsing.Components;
using ModularSystem.Core.TextAnalysis.Parsing.LR1.Components;
using ModularSystem.Core.TextAnalysis.Tokenization;
using System.Security.Cryptography.X509Certificates;

namespace ModularSystem.Core.TextAnalysis.Parsing;

public class LR1Stack
{
    private Stack<int> States { get; }
    private Stack<Symbol> Symbols { get; }

    public LR1Stack()
    {
        States = new Stack<int>();
        Symbols = new Stack<Symbol>();
    }

    public int PeekState()
    {
        if(States.Count == 0)
        {
            throw new InvalidOperationException("The stack is empty.");
        }

        return States.Peek();
    }

    public Symbol? PeekSymbol()
    {
        if(Symbols.Count == 0)
        {
            return null;
        }

        return Symbols.Peek();
    }

    public void PushState(int state)
    {
        States.Push(state);
    }

    public void PushSymbol(Symbol symbol)
    {
        Symbols.Push(symbol);
    }

    public int PopState()
    {
        if(States.Count == 0)
        {
            throw new InvalidOperationException("The stack is empty.");
        }

        return States.Pop();
    }

    public Symbol PopSymbol()
    {
        if(Symbols.Count == 0)
        {
            throw new InvalidOperationException("The stack is empty.");
        }

        return Symbols.Pop();
    }
}

public class LR1Context
{
    public InputStream Input { get; }
    public LR1Stack Stack { get; }

    public LR1Context(InputStream input, LR1Stack stack)
    {
        Input = input;
        Stack = stack;
    }

    public Exception UnexpectedEndOfTokens()
    {
        return new InvalidOperationException("Unexpected end of tokens.");
    }

    public Exception SyntaxError()
    {
        return new InvalidOperationException("Syntax error.");
    }
}

public class LR1Parser
{
    private LR1ParsingTable Table { get; }

    public LR1Parser(Grammar grammar)
    {
        Table = LR1ParsingTable.Create(grammar.Productions);
    }

    public void Parse(string text)
    {
        using var input = new InputStream(text, Tokenizer.Instance);
        var stack = new LR1Stack();
        var context = new LR1Context(input, stack);

        while (true)
        {
            var action = GetNextAction(context);

            ExecuteAction(context, action);

            if (action.Type == LR1ParserActionType.Accept)
            {
                break;
            }            
        }
    }

    private LR1Action GetNextAction(LR1Context context)
    {
        var currentState = context.Stack.PeekState();
        var lookahead = context.Input.Lookahead;

        if(lookahead is null)
        {
            throw context.UnexpectedEndOfTokens();
        }

        var action = Table.Lookup(currentState, lookahead);

        if(action is null)
        {
            throw context.SyntaxError();
        }

        return action;
    }

    private void ExecuteAction(LR1Context context,  LR1Action action)
    {
        switch (action.Type)
        {
            case LR1ParserActionType.Shift:
                Shift(context, action.AsShift());
                return;

            case LR1ParserActionType.Reduce:
                Reduce(context, action.AsReduce());
                return;

            case LR1ParserActionType.Goto:
                Goto(context, action.AsGoto());
                break;

            case LR1ParserActionType.Accept:
                Accept(context, action.AsAccept());
                break;

            default:
                throw new InvalidOperationException();
        }
    }

    private void Shift(LR1Context context, LR1ShiftAction action)
    {
        if(context.Input.Lookahead is null)
        {
            throw context.UnexpectedEndOfTokens();
        }

        context.Stack.PushSymbol(context.Input.Lookahead);
        context.Stack.PushState(action.NextState);
        context.Input.Consume();
    }

    private void Reduce(LR1Context context, LR1ReduceAction action)
    {
        var production = Table.GetProduction(action.ProductionIndex);

        for (int i = 0; i < production.Body.Length; i++)
        {
            context.Stack.PopState();
            context.Stack.PopSymbol();
        }
    }

    private void Goto(LR1Context context, LR1GotoAction action)
    {
        context.Stack.PushState(action.NextState);
    }

    private void Accept(LR1Context context, LR1AcceptAction action)
    {
        context.Stack.PopState();
    }
}
