using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class ExpressionArrayParser
{
    //*
    //* So, currently expression_array can only contain a literal_expression or a reference_expression.
    //*
    public ExpressionSymbol[] ParseExpressionArray(ParsingContext context, ArrayToken token)
    {
        var expressions = new List<ExpressionSymbol>(token.Values.Length);

        foreach (var item in token)
        {
            var expr = null as ExpressionSymbol;

            //* case literal_expression and reference_expression
            if (item is ValueToken valueToken)
            {
                expr = SyntaxParser.ParseExpression(context, valueToken);
            }

            //* case operator_expression
            if (item is ObjectPropertyToken objectProperty)
            {
                //* this is impossible to happen
                expr = SyntaxParser.ParseExpression(context, objectProperty);
            }

            //* case lambda_expression
            if (item is ArrayToken arrayToken)
            {
                //* this is valid syntax, but its too big of a headache to handle
                throw new Exception();
                expr = SyntaxParser.ParseLambda(context, arrayToken);
            }

            //* case type_projection_expression
            if (item is ObjectToken objectToken)
            {
                //* this is valid syntax, but its too big of a headache to handle
                throw new Exception();
                expr = SyntaxParser.ParseTypeProjection(context, objectToken);
            }

            if(expr == null)
            {
                throw new Exception();
            }

            expressions.Add(expr);
        }

        return expressions.ToArray();
    }
}
