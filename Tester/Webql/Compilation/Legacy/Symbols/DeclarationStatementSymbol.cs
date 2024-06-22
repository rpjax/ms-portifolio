using ModularSystem.Webql.Analysis.Components;

namespace ModularSystem.Webql.Analysis.Symbols;

//*
// This could be expanded in the future to accommodate specific declarations such as:
// - Function parameters, for example: 'void myFunc(int param)'.
// - Variables, for example: 'int number;' or 'string foo = "bar";'.
//*

public class DeclarationStatementSymbol : StatementSymbol, IDeclarationSymbol
{
    public override StatementType StatementType { get; } = StatementType.Declaration;
    public string? Type { get; internal set; }
    public string Identifier { get; internal set; }
    public string[] Modifiers { get; internal set; }
    public ExpressionSymbol? Value { get; internal set; }

    public DeclarationStatementSymbol(
        string? type, 
        string identifier, 
        string[]? modifiers = null, 
        ExpressionSymbol? value = null
    )
    {
        Type = type;
        Identifier = identifier;
        Modifiers = modifiers ?? Array.Empty<string>();
        Value = value;
    }

    public override string ToString()
    {
        //var type = ExpressionType;
        //var modifiers = Modifiers.Length > 0 ? $"[{string.Join(", ", Modifiers)}]" : "";
        var value = Value is not null ? $" = {Value}" : "";
        
        return $"{Identifier}{value}";
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteDeclarationStatement(this);
    }

    protected override IEnumerable<Symbol> GetChildren()
    {
        if(Value is not null)
        {
            yield return Value;
        }
    }
}
