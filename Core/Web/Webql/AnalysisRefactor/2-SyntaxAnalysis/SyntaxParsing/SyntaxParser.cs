using ModularSystem.Core;
using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public abstract class SyntaxParser
{
    public static IReadOnlyDictionary<string, ExprOp> OpsTable { get; } = CreateOpsTable();

    public static string Stringify(ExprOp op)
    {
        return $"${op.ToString().ToCamelCase()}";
    }

    private static IReadOnlyDictionary<string, ExprOp> CreateOpsTable()
    {
        var ops = Enum
            .GetValues(typeof(ExprOp))
            .Cast<ExprOp>()
            .ToArray();

        var strs = ops
            .Select(op => Stringify(op))
            .ToArray();

        var dic = new Dictionary<string, ExprOp>();

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

    protected ExprOp? TryParseExprOp(string str)
    {
        if(OpsTable.TryGetValue(str, out ExprOp op))
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
