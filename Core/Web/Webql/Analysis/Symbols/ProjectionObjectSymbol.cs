using System.Collections;

namespace ModularSystem.Webql.Analysis.Symbols;

public class ProjectionObjectSymbol : Symbol, IEnumerable<ProjectionObjectExprSymbol>
{
    public ProjectionObjectExprSymbol[] Expressions { get; }

    public ProjectionObjectSymbol(ProjectionObjectExprSymbol[] expressions)
    {
        Expressions = expressions;
    }

    public IEnumerator<ProjectionObjectExprSymbol> GetEnumerator()
    {
        return Expressions.AsEnumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Expressions.GetEnumerator();
    }

    public override string ToString()
    {
        var exprs = string.Join(Environment.NewLine, Expressions.Select(x => x.ToString() + ";"));

        return $"{{ {exprs} }}";
    }

}

public class ProjectionObjectExprSymbol : Symbol
{
    public string Key { get; }
    public ProjectionValueSymbol Value { get; }

    public ProjectionObjectExprSymbol(string key, ProjectionValueSymbol value)
    {
        Key = key;
        Value = value;
        
    }

    public override string ToString()
    {
        return $"{{ {Key}: {Value} }}";
    }
}

public abstract class ProjectionValueSymbol : Symbol
{
    public enum ValueType
    {
        Reference,
        ProjectionObject,
        Expr
    }  

    public ValueType Type { get; }

    public ProjectionValueSymbol()
    {

    }

    public ReferenceSymbol GetReference()
    {

    }
}

public class ProjectionReferenceValueSymbol : ProjectionValueSymbol
{
    public ReferenceSymbol Reference { get; }

    public ProjectionReferenceValueSymbol(ReferenceSymbol reference)
    {
        Reference = reference;
    }

    public override string ToString()
    {
        return Reference.ToString();
    }
}

public class ProjectionObjectValueSymbol : ProjectionValueSymbol
{
    public ProjectionObjectSymbol Object { get; }

    public ProjectionObjectValueSymbol(ProjectionObjectSymbol obj)
    {
        Object = obj;
    }

    public override string ToString()
    {
        return Object.ToString();
    }
}
