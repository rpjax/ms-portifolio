namespace ModularSystem.Webql.Analysis.Symbols;

public class LambdaSymbol : Symbol
{
    public LambdaArgumentSymbol[] Arguments { get; }
    public StatementBlockSymbol Body { get; }

    public LambdaSymbol(LambdaArgumentSymbol[] arguments, StatementBlockSymbol body)
    {
        Arguments = arguments;
        Body = body;
    }

    public override string ToString()
    {
        var args = string.Join(", ", Arguments.Select(x => x.ToString()));
        var body = Body.ToString();

        return $"({args}){Environment.NewLine}{body}";
    }

}
