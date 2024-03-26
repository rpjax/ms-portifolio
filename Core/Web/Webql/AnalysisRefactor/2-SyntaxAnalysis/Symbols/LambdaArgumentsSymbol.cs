namespace ModularSystem.Webql.Analysis.Symbols;

public class LambdaArgumentsSymbol : Symbol
{
    public LambdaArgumentSymbol[] Arguments { get; }

    public LambdaArgumentsSymbol(LambdaArgumentSymbol[] args)
    {
        Arguments = args;
    }

    public override string ToString()
    {
        return $"{string.Join(", ", Arguments.Select(x => x.ToString()))}";
    }

}
