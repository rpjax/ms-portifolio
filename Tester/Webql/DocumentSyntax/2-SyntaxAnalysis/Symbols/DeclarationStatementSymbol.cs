namespace ModularSystem.Webql.Analysis.Symbols;

//*
// This could be expanded in the future to accommodate specific declarations such as:
// - Function parameters, for example: 'void myFunc(int param)'.
// - Variables, for example: 'int number;' or 'string foo = "bar";'.
//*

public class DeclarationStatementSymbol : StatementSymbol
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
        return $"{Type} {Identifier} [{string.Join("; ", Modifiers)}]";
    }
}
