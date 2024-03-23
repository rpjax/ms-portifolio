using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class ProjectionObjectParser : Parser
{
    public ProjectionObjectSymbol ParseProjectionObject(ParsingContext context, ObjectToken token)
    {
        var exprs = new List<ProjectionObjectExprSymbol>();

        foreach (var item in token)
        {
            var expr = new ProjectionObjectExprParser()
                .ParseProjectionObject(context, item);

            exprs.Add(expr);
        }

        return new ProjectionObjectSymbol(exprs.ToArray());
    }
}

public class ProjectionObjectExprParser : Parser
{
    public ProjectionObjectExprSymbol ParseProjectionObject(ParsingContext context, ObjectProperty property)
    {
        var key = property.Key;

        //* case expr
        if (IsOperator(key))
        {
            var expr = new ExprParser()
                .ParseExpr(context, property);
            var value = new ProjectionObjectExprValueSymbol(expr);

            return new ProjectionObjectExprSymbol(key, value);
        }

        //* case reference
        if (property.Value is StringToken stringToken)
        {
            var reference = TokenParser.ParseReference(context, stringToken);
            var value = new ProjectionObjectExprValueSymbol(reference);

            return new ProjectionObjectExprSymbol(key, value);
        }

        //* case projection object
        if (property.Value is ObjectToken objectToken)
        {
            var projectionObj = new ProjectionObjectParser()
                .ParseProjectionObject(context, objectToken);
            var value = new ProjectionObjectExprValueSymbol(projectionObj);

            return new ProjectionObjectExprSymbol(key, value);
        }

        throw new ParsingException("", context);
    }
}
