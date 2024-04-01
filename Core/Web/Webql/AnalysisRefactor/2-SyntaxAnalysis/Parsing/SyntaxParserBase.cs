using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public abstract class SyntaxParserBase
{
    public static IReadOnlyDictionary<string, OperatorType> OperatorTable { get; } = CreateOperatorTable();

    public static string Stringify(OperatorType op)
    {
        //return $"${op.ToString().ToCamelCase()}";
        return $"${op.ToString().ToLower()}";
    }

    private static IReadOnlyDictionary<string, OperatorType> CreateOperatorTable()
    {
        var ops = Enum
            .GetValues(typeof(OperatorType))
            .Cast<OperatorType>()
            .ToArray();

        var strs = ops
            .Select(op => Stringify(op))
            .ToArray();

        var dic = new Dictionary<string, OperatorType>();

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

    protected OperatorType? TryParseExprOp(string str)
    {
        if(OperatorTable.TryGetValue(str, out OperatorType op))
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
