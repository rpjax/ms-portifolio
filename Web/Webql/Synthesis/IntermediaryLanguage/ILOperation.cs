using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis.Compilation;

public enum ILOpCode
{
    LinqWhere,
}

public class ILOperation
{
    public ILOpCode OpCode { get; init; }
}

public class FilterOperation : ILOperation
{
    
}

public class ILCompiler
{
    public ILOperation Compile(Node node)
    {
        return new ILOperation();
    }
}

public class ExpressionCompiler
{
    public Expression Compile(ILOperation operation)
    {
        throw new NotImplementedException();
    }

    private Expression TranslateWhere(TranslationContextOld context, ILOperation operation)
    {



        throw new NotImplementedException();
    }
}
