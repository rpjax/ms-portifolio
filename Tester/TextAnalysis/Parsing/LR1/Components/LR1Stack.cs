using ModularSystem.Core.TextAnalysis.Language.Components;

namespace ModularSystem.Core.TextAnalysis.Parsing.LR1.Components;

public class LR1Stack
{
    private Stack<int> States { get; }
    private Stack<Symbol> Symbols { get; }
    private Stack<object> Stack { get; }

    private bool UseDebug { get; }
    private Stack<object> DebugStack { get; }

    public LR1Stack(bool useDebug = false)
    {
        States = new Stack<int>();
        Symbols = new Stack<Symbol>();
        Stack = new Stack<object>();
        UseDebug = useDebug;
        DebugStack = new Stack<object>();
    }

    public int StatesCount => States.Count;

    public override string ToString()
    {
        return string.Join(" ", DebugStack.Reverse().Select(x => x.ToString()));
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

        if(UseDebug)
        {
            DebugStack.Push(state);
        }
    }

    public void PushSymbol(Symbol symbol)
    {
        Symbols.Push(symbol);

        if(UseDebug)
        {
            DebugStack.Push(symbol);
        }
    }

    public int PopState()
    {
        if(States.Count == 0)
        {
            throw new InvalidOperationException("The stack is empty.");
        }

        if(UseDebug)
        {
            DebugStack.Pop();
        }

        return States.Pop();
    }

    public Symbol PopSymbol()
    {
        if(Symbols.Count == 0)
        {
            throw new InvalidOperationException("The stack is empty.");
        }

        if(UseDebug)
        {
            DebugStack.Pop();
        }

        return Symbols.Pop();
    }
}
