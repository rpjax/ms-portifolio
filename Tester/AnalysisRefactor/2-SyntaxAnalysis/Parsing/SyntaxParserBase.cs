using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public abstract class SyntaxParserBase
{
    public static IReadOnlyDictionary<string, ExpressionOperator> OpsTable { get; } = CreateOpsTable();

    public static string Stringify(ExpressionOperator op)
    {
        //return $"${op.ToString().ToCamelCase()}";
        return $"${op.ToString().ToLower()}";
    }

    private static IReadOnlyDictionary<string, ExpressionOperator> CreateOpsTable()
    {
        var ops = Enum
            .GetValues(typeof(ExpressionOperator))
            .Cast<ExpressionOperator>()
            .ToArray();

        var strs = ops
            .Select(op => Stringify(op))
            .ToArray();

        var dic = new Dictionary<string, ExpressionOperator>();

        for (int i = 0; i < ops.Length; i++)
        {
            var op = ops[i];
            var str = strs[i];

            dic.Add(str, op);
        }

        return dic;
    }

    protected T CastToken<T>(ParsingContext context, Token token) where T : Token
    {
        if(token is not T result)
        {
            throw new ParsingException("", context);
        }

        return result;
    }

    protected ExpressionOperator? TryParseExprOp(string str)
    {
        if(OpsTable.TryGetValue(str, out ExpressionOperator op))
        {
            return op;
        }

        return null;
    }

    protected bool IsOperator(string str)
    {
        return TryParseExprOp(str) != null;
    }
}
