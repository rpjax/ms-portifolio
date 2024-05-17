namespace ModularSystem.Core.TextAnalysis.Parsing.LR1.Components;

public enum LR1ParserActionType
{
    Shift,
    Reduce,
    Goto,
    Accept,
}

public abstract class LR1Action : IEquatable<LR1Action>
{
    public LR1ParserActionType Type { get; init; }

    public abstract override string ToString();

    public abstract bool Equals(LR1Action? other);

    public LR1ShiftAction AsShift()
    {
        return (LR1ShiftAction)this ?? throw new InvalidCastException();
    }

    public LR1ReduceAction AsReduce()
    {
        return (LR1ReduceAction)this ?? throw new InvalidCastException();
    }

    public LR1GotoAction AsGoto()
    {
        return (LR1GotoAction)this ?? throw new InvalidCastException();
    }

    public LR1AcceptAction AsAccept()
    {
        return (LR1AcceptAction)this ?? throw new InvalidCastException();
    }

}

public class LR1ShiftAction : LR1Action
{
    public int NextState { get; }

    public LR1ShiftAction(int nextState)
    {
        Type = LR1ParserActionType.Shift;
        NextState = nextState;
    }

    public override string ToString()
    {
        return $"Shift and Goto {NextState}";
    }

    public override bool Equals(LR1Action? other)
    {
        return other is LR1ShiftAction action && NextState == action.NextState;
    }
}

public class LR1ReduceAction : LR1Action
{
    public int ProductionIndex { get; }

    public LR1ReduceAction(int productionIndex)
    {
        Type = LR1ParserActionType.Reduce;
        ProductionIndex = productionIndex;
    }

    public override string ToString()
    {
        return $"Reduce using {ProductionIndex}";
    }

    public override bool Equals(LR1Action? other)
    {
        return other is LR1ReduceAction action && ProductionIndex == action.ProductionIndex;
    }
}

public class LR1GotoAction : LR1Action
{
    public int NextState { get; }

    public LR1GotoAction(int nextState)
    {
        Type = LR1ParserActionType.Goto;
        NextState = nextState;
    }

    public override string ToString()
    {
        return $"Goto {NextState}";
    }

    public override bool Equals(LR1Action? other)
    {
        return other is LR1GotoAction action && NextState == action.NextState;
    }
}

public class LR1AcceptAction : LR1Action
{
    public LR1AcceptAction()
    {
        Type = LR1ParserActionType.Accept;
    }

    public override string ToString()
    {
        return "Accept";
    }

    public override bool Equals(LR1Action? other)
    {
        return other is LR1AcceptAction;
    }
}
