using System.Text;

namespace ModularSystem.Core.TextAnalysis;

public abstract class Symbol : IEquatable<Symbol>
{
    public abstract bool IsTerminal { get; }
    public abstract bool IsEpsilon { get; }
    public abstract string Name { get; }

    public abstract bool Equals(Symbol? other);
    public abstract ReadOnlySpan<char> GetValue();
}

public class TerminalSymbol : Symbol
{
    public override bool IsTerminal => true;
    public override bool IsEpsilon => false;
    public override string Name { get; }
    private string Value { get; }

    public TerminalSymbol(string productionName, string value)
    {
        Name = productionName;
        Value = value;
    }

    public override ReadOnlySpan<char> GetValue()
    {
        return Value;
    }

    public override bool Equals(Symbol? other)
    {
        if (other == null)
        {
            return false;
        }

        return Name == other.Name
            && other is TerminalSymbol terminal
            && Value == terminal.Value;
    }
}

public class NonTerminalSymbol : Symbol
{
    public override bool IsTerminal => false;
    public override bool IsEpsilon => false;
    public override string Name { get; }
    private Symbol[] Children { get; }

    public NonTerminalSymbol(string productionName, Symbol[] children)
    {
        Name = productionName;
        Children = children;
    }
    public override ReadOnlySpan<char> GetValue()
    {
        var builder = new StringBuilder();

        foreach (var child in Children)
        {
            builder.Append(child.GetValue());
        }

        return builder.ToString();
    }

    public override bool Equals(Symbol? other)
    {
        if (other == null)
        {
            return false;
        }
        if (other is not NonTerminalSymbol nonTerminal)
        {
            return false;
        }
        if (Children.Length != nonTerminal.Children.Length)
        {
            return false;
        }
        if (Name != nonTerminal.Name)
        {
            return false;
        }

        var childrenMatch = true;

        for (int i = 0; i < Children.Length; i++)
        {
            childrenMatch = childrenMatch && Children[i].Equals(nonTerminal.Children[i]);

            if (!childrenMatch)
            {
                break;
            }
        }

        return childrenMatch;
    }
}

public class EpsilonSymbol : Symbol
{
    public override bool IsTerminal => true;
    public override bool IsEpsilon => true;
    public override string Name => string.Empty;

    public override bool Equals(Symbol? other)
    {
        return other?.IsEpsilon == true;
    }

    public override ReadOnlySpan<char> GetValue()
    {
        return string.Empty;
    }
}