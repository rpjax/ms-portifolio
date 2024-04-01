using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class TypeExpressionParser : SyntaxParserBase
{
    public TypeProjectionExpressionSymbol ParseTypeExpression(ParsingContext context, ObjectToken token)
    {
        var bindings = new List<ProjectionBindingSymbol>();

        foreach (var item in token)
        {
            var expr = new TypeBindingParser()
                .ParseTypeBinding(context, item);

            bindings.Add(expr);
        }

        return new TypeProjectionExpressionSymbol(bindings.ToArray());
    }
}

public class TypeBindingParser : SyntaxParserBase
{
    public ProjectionBindingSymbol ParseTypeBinding(ParsingContext context, ObjectPropertyToken property)
    {
        var key = property.Key;

        ////* case expr
        //if (IsOperator(key))
        //{
        //    var expr = new OperatorExpressionParser()
        //        .ParseOperatorExpression(context, property);

        //    return new TypeBindingSymbol(key, expr);
        //}

        //* case reference
        if (property.Value is StringToken stringToken)
        {
            return new ProjectionBindingSymbol(
                key: key, 
                value: SyntaxParser.ParseReference(context, stringToken)
            );
        }

        //* case projection object
        if (property.Value is ObjectToken objectToken)
        {
            return new ProjectionBindingSymbol(
                  key: key,
                  value: SyntaxParser.ParseTypeProjection(context, objectToken)
            );
        }

        throw new ParsingException("", context);
    }
}
