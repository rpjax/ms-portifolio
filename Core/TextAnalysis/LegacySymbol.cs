using System.Text;

namespace ModularSystem.TextAnalysis;

[Obsolete("Use ModularSystem.TextAnalysis.Language.Components.Symbol instead")]
public abstract class LegacySymbol : IEquatable<LegacySymbol>
{
    public abstract bool IsTerminal { get; }
    public abstract bool IsEpsilon { get; }
    public abstract string Name { get; }

    public abstract bool Equals(LegacySymbol? other);
    public abstract ReadOnlySpan<char> GetValue();
}

[Obsolete("Use ModularSystem.TextAnalysis.Language.Components.Terminal instead")]
public class LegacyTerminalSymbol : LegacySymbol
{
    public override bool IsTerminal => true;
    public override bool IsEpsilon => false;
    public override string Name { get; }
    private string Value { get; }

    public LegacyTerminalSymbol(string productionName, string value)
    {
        Name = productionName;
        Value = value;
    }

    public override ReadOnlySpan<char> GetValue()
    {
        return Value;
    }

    public override bool Equals(LegacySymbol? other)
    {
        if (other == null)
        {
            return false;
        }

        return Name == other.Name
            && other is LegacyTerminalSymbol terminal
            && Value == terminal.Value;
    }
}

[Obsolete("Use ModularSystem.TextAnalysis.Language.Components.NonTerminal instead")]
public class LegacyNonTerminalSymbol : LegacySymbol
{
    public override bool IsTerminal => false;
    public override bool IsEpsilon => false;
    public override string Name { get; }
    private LegacySymbol[] Children { get; }

    public LegacyNonTerminalSymbol(string productionName, LegacySymbol[] children)
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

    public override bool Equals(LegacySymbol? other)
    {
        if (other == null)
        {
            return false;
        }
        if (other is not LegacyNonTerminalSymbol nonTerminal)
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

[Obsolete("Use ModularSystem.TextAnalysis.Language.Components.Epsilon instead")]
public class LegacyEpsilonSymbol : LegacySymbol
{
    public override bool IsTerminal => true;
    public override bool IsEpsilon => true;
    public override string Name => string.Empty;

    public override bool Equals(LegacySymbol? other)
    {
        return other?.IsEpsilon == true;
    }

    public override ReadOnlySpan<char> GetValue()
    {
        return string.Empty;
    }
}