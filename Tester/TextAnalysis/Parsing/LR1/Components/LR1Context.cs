using ModularSystem.Core.TextAnalysis.Parsing.Components;

namespace ModularSystem.Core.TextAnalysis.Parsing.LR1.Components;

public class LR1Context
{
    public InputStream InputStream { get; }
    public LR1Stack Stack { get; }
    public CstBuilder CstBuilder { get; }

    public LR1Context(InputStream inputStream, LR1Stack stack)
    {
        InputStream = inputStream;
        Stack = stack;
        CstBuilder = new CstBuilder();
    }

    public Exception UnexpectedEndOfTokens()
    {
        return new InvalidOperationException("Unexpected end of tokens.");
    }

    public Exception SyntaxError()
    {
        return new InvalidOperationException("Syntax error.");
    }
}

