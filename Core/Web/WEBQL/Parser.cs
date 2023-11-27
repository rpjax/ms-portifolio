using ModularSystem.Core;
using ModularSystem.Core.Expressions;
using ModularSystem.Webql.Analysis;
using ModularSystem.Webql.Synthesis;
using System.Linq.Expressions;
using System.Text.Json.Nodes;

namespace ModularSystem.Webql;

public static class Parser
{
    /// <summary>
    /// Performs a syntax analysis and returns the syntax tree with no further transformations.
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static Node Parse(string json)
    {
        try
        {
            var jsonNode = JsonNode.Parse(json);

            if (jsonNode == null)
            {
                throw new Exception();
            }

            return SyntaxAnalyser.Parse(jsonNode);
        }
        catch (Exception e)
        {
            throw HandleError(null, null, e);
        }
    }

    public static PaginationIn ParsePagination(Node root)
    {
        try
        {
            if (root is not ScopeDefinitionNode scope)
            {
                throw new Exception();
            }

            var pagination = new PaginationIn();
            var limitExpression = scope["limit"];
            var offsetExpression = scope["offset"];

            if (limitExpression != null)
            {
                if (limitExpression.Rhs.Value is not LiteralNode literal)
                {
                    throw new Exception();
                }
                if (literal.Value == null)
                {
                    throw new Exception();
                }

                pagination.Limit = int.Parse(literal.Value);
            }

            if (offsetExpression != null)
            {
                if (offsetExpression.Rhs.Value is not LiteralNode literal)
                {
                    throw new Exception();
                }
                if (literal.Value == null)
                {
                    throw new Exception();
                }

                pagination.Offset = int.Parse(literal.Value);
            }

            return pagination;
        }
        catch (Exception e)
        {
            throw HandleError(root, null, e);
        }
    }

    public static Expression? ParseFilter(Node root, Type type)
    {
        try
        {
            if (root is not ScopeDefinitionNode scope)
            {
                throw new Exception();
            }

            var whereExpression = scope["where"];

            if (whereExpression == null || scope.Expressions.IsEmpty())
            {
                return null;
            }

            var whereExpressionRhs = whereExpression.Rhs.Value;
            var syntaxTree = FilterSemanticsAnalyser.Parse(type, whereExpressionRhs);

            var parameter = Expression.Parameter(type, "x");
            var context = new GeneratorContext(type, parameter);
            var lambdaParamType = context.Type;

            var parameters = new ParameterExpression[] { Expression.Parameter(lambdaParamType, "x") };
            var body = FilterGenerator.Translate(context, syntaxTree);
            var expression = Expression.Lambda(body, parameters);

            var visitor = new ParameterExpressionUniformityVisitor();

            return visitor.Visit(expression);
        }
        catch (Exception e)
        {
            throw HandleError(root, type, e);
        }
    }

    public static Expression<Func<T, bool>>? ParseToLambdaFilter<T>(Node root)
    {
        var expression = ParseFilter(root, typeof(T));

        if (expression == null)
        {
            return null;
        }

        return expression.TypeCast<Expression<Func<T, bool>>>();
    }

    public static OrderExpression[]? ParseOrder<T>(Node root)
    {
        var type = typeof(T);

        try
        {
            if (root is not ScopeDefinitionNode scope)
            {
                throw new Exception();
            }

            var orderExpression = scope["order"];

            if (orderExpression == null || scope.Expressions.IsEmpty())
            {
                return null;
            }

            var syntaxTree = OrderSemanticsAnalyser.Parse(type, orderExpression.Rhs.Value);
            var parameter = Expression.Parameter(type, "x");
            var context = new GeneratorContext(type, parameter);

            return OrderGenerator
                .Translate(context, syntaxTree)
                .ToArray();
        }
        catch (Exception e)
        {
            throw HandleError(root, type, e);
        }
    }

    private static Exception HandleError(Node? root, Type? type, Exception e)
    {
        if (e is SyntaxException syntaxException)
        {
            return new AppException(syntaxException.GetMessage(), ExceptionCode.InvalidInput, e, root);
        }

        if (e is SemanticException semanticException)
        {
            return new AppException(semanticException.GetMessage(), ExceptionCode.InvalidInput, e, root);
        }

        if (e is GeneratorException generatorException)
        {
            return new AppException(generatorException.GetMessage(), ExceptionCode.InvalidInput, e, root);
        }

        return e;
    }

}