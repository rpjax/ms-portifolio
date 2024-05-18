using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Core.TextAnalysis.Parsing.LR1.Components;

/// <summary>
/// Represents an action type in a LR(1) parsing table.
/// </summary>
public enum LR1ActionType
{
    Shift,
    Reduce,
    Goto,
    Accept,
}

/// <summary>
/// Defines an action in a LR(1) parsing table. Concrete implementations are <see cref="LR1ShiftAction"/>, <see cref="LR1ReduceAction"/>, <see cref="LR1GotoAction"/> and <see cref="LR1AcceptAction"/>.
/// </summary>
public abstract class LR1Action : 
    IEquatable<LR1Action>, 
    IEqualityComparer<LR1Action>
{
    public LR1ActionType Type { get; init; }

    public abstract override string ToString();

    public abstract bool Equals(LR1Action? other);

    public bool Equals(LR1Action? x, LR1Action? y)
    {
        return x is not null && y is not null 
            && x.Equals(y);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as LR1Action);
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    public int GetHashCode([DisallowNull] LR1Action obj)
    {
        throw new NotImplementedException();
    }

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

/// <summary>
/// Represents a SHIFT action in a LR(1) parsing table.
/// </summary>
public class LR1ShiftAction : LR1Action
{
    public int NextState { get; }

    public LR1ShiftAction(int nextState)
    {
        Type = LR1ActionType.Shift;
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

/// <summary>
/// Represents a REDUCE action in a LR(1) parsing table.
/// </summary>
public class LR1ReduceAction : LR1Action
{
    public int ProductionIndex { get; }

    public LR1ReduceAction(int productionIndex)
    {
        Type = LR1ActionType.Reduce;
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

/// <summary>
/// Represents a GOTO action in a LR(1) parsing table.
/// </summary>
public class LR1GotoAction : LR1Action
{
    public int NextState { get; }

    public LR1GotoAction(int nextState)
    {
        Type = LR1ActionType.Goto;
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

/// <summary>
/// Represents an ACCEPT action in a LR(1) parsing table.
/// </summary>
public class LR1AcceptAction : LR1Action
{
    public LR1AcceptAction()
    {
        Type = LR1ActionType.Accept;
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
