using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis;

/// <summary>
/// Represents a translator for WebQL operators. This class facilitates the translation of various WebQL operators <br/>
/// into their corresponding LINQ expressions.
/// </summary>
public class OperatorTranslator
{
    /// <summary>
    /// Gets the options for the translator.
    /// </summary>
    protected TranslatorOptions Options { get; }

    /// <summary>
    /// An instance of NodeTranslator used for node parsing.
    /// </summary>
    protected NodeTranslator NodeTranslator { get; }

    /// <summary>
    /// A translator for arithmetic operators.
    /// </summary>
    protected ArithmeticOperatorsTranslator ArithmeticOperatorsTranslator { get; }

    /// <summary>
    /// A translator for relational operators.
    /// </summary>
    protected RelationalOperatorsTranslator RelationalOperatorsTranslator { get; }

    /// <summary>
    /// A translator for pattern relational operators.
    /// </summary>
    protected PatternRelationalOperatorsTranslator PatternRelationalOperatorsTranslator { get; }

    /// <summary>
    /// A translator for logical operators.
    /// </summary>
    protected LogicalOperatorsTranslator LogicalOperatorTranslator { get; }

    /// <summary>
    /// A translator for semantic operators.
    /// </summary>
    protected SemanticOperatorsTranslator SemanticOperatorsTranslator { get; }

    /// <summary>
    /// A translator for queryable operators.
    /// </summary>
    protected QueryableOperatorsTranslator QueryableOperatorsTranslator { get; }

    /// <summary>
    /// Initializes a new instance of the OperatorTranslator class with the specified options and node parser.
    /// </summary>
    /// <param name="options">The translator options.</param>
    /// <param name="nodeParser">The node parser to be used.</param>
    public OperatorTranslator(TranslatorOptions options, NodeTranslator nodeParser)
    {
        Options = options;
        NodeTranslator = nodeParser;
        ArithmeticOperatorsTranslator = new(options, NodeTranslator);
        RelationalOperatorsTranslator = new(options, NodeTranslator);
        PatternRelationalOperatorsTranslator = new(options, NodeTranslator);
        LogicalOperatorTranslator = new(options, NodeTranslator);
        SemanticOperatorsTranslator = new(options, NodeTranslator);
        QueryableOperatorsTranslator = new(options, NodeTranslator);
    }

    /// <summary>
    /// Translates a WebQL operator and its associated node into a LINQ expression.
    /// </summary>
    /// <param name="context">The translation context.</param>
    /// <param name="operator">The WebQL operator to be translated.</param>
    /// <param name="node">The node associated with the operator.</param>
    /// <returns>The translated LINQ expression.</returns>
    /// <exception cref="Exception">Thrown if the operator is unknown or unsupported.</exception>
    public Expression Translate(TranslationContext context, Operator @operator, Node node)
    {
        switch (@operator)
        {
            // Arithmetic Operators
            case Operator.Add:
                return ArithmeticOperatorsTranslator.TranslateAdd(context, node);

            case Operator.Subtract:
                return ArithmeticOperatorsTranslator.TranslateSubtract(context, node);

            case Operator.Divide:
                return ArithmeticOperatorsTranslator.TranslateDivide(context, node);

            case Operator.Multiply:
                return ArithmeticOperatorsTranslator.TranslateMultiply(context, node);

            case Operator.Modulo:
                return ArithmeticOperatorsTranslator.TranslateModulo(context, node);

            // Relational Operators
            case Operator.Equals:
                return RelationalOperatorsTranslator.TranslateEquals(context, node);

            case Operator.NotEquals:
                return RelationalOperatorsTranslator.TranslateNotEquals(context, node);

            case Operator.Less:
                return RelationalOperatorsTranslator.TranslateLess(context, node);

            case Operator.LessEquals:
                return RelationalOperatorsTranslator.TranslateLessEquals(context, node);

            case Operator.Greater:
                return RelationalOperatorsTranslator.TranslateGreater(context, node);

            case Operator.GreaterEquals:
                return RelationalOperatorsTranslator.TranslateGreaterEquals(context, node);

            // Pattern Relational Operators
            case Operator.Like:
                return PatternRelationalOperatorsTranslator.TranslateLike(context, node);

            case Operator.RegexMatch:
                return PatternRelationalOperatorsTranslator.TranslateRegexMatch(context, node);

            // Logical Operators
            case Operator.Or:
                return LogicalOperatorTranslator.TranslateOr(context, node);

            case Operator.And:
                return LogicalOperatorTranslator.TranslateAnd(context, node);

            case Operator.Not:
                return LogicalOperatorTranslator.TranslateNot(context, node);

            // Semantic Operators
            case Operator.Expr:
                return SemanticOperatorsTranslator.TranslateExpr(context, node);

            case Operator.Literal:
                return SemanticOperatorsTranslator.TranslateLiteral(context, node);

            case Operator.Select:
                return SemanticOperatorsTranslator.TranslateSelect(context, node);

            // Queryable Operators
            case Operator.Filter:
                return QueryableOperatorsTranslator.TranslateFilter(context, node);

            case Operator.Project:
                return QueryableOperatorsTranslator.TranslateProject(context, node);

            case Operator.Transform:
                return QueryableOperatorsTranslator.TranslateTransform(context, node);    

            case Operator.SelectMany:
                return QueryableOperatorsTranslator.TranslateSelectMany(context, node); 

            case Operator.Limit:
                return QueryableOperatorsTranslator.TranslateLimit(context, node);

            case Operator.Skip:
                return QueryableOperatorsTranslator.TranslateSkip(context, node);

            case Operator.Count:
                return QueryableOperatorsTranslator.TranslateCount(context, node);

            case Operator.Index:
                break;

            case Operator.Any:
                return QueryableOperatorsTranslator.TranslateAny(context, node);

            case Operator.All:
                return QueryableOperatorsTranslator.TranslateAll(context, node);

            // TODO:
            // Aggregation Operators 
            case Operator.Min:
            case Operator.Max:
            case Operator.Sum:
            case Operator.Average:
                break;
        }

        throw new TranslationException("The operator encountered during translation is either unknown or not supported. Please verify the operator used in the query and ensure it aligns with the supported operators in WebQL.", context);
    }

}

public class ArithmeticOperatorsTranslator
{
    private TranslatorOptions Options { get; }
    private NodeTranslator NodeTranslator { get; }

    public ArithmeticOperatorsTranslator(TranslatorOptions options, NodeTranslator nodeTranslator)
    {
        Options = options;
        NodeTranslator = nodeTranslator;
    }

    public Expression TranslateAdd(TranslationContext context, Node node)
    {
        if (node is ArrayNode arrayNode)
        {
            if (arrayNode.Length != 2)
            {
                throw new Exception();
            }

            var elementOne = NodeTranslator.Translate(context, arrayNode[0]);
            var elementTwoContext = new TranslationContext(elementOne.Type, elementOne, context);
            var elementTwo = NodeTranslator.Translate(elementTwoContext, arrayNode[1]);

            return Expression.Add(elementOne, elementTwo);
        }

        return Expression.Add(context.Expression, NodeTranslator.Translate(context, node));
    }

    public Expression TranslateSubtract(TranslationContext context, Node node)
    {
        if (node is ArrayNode arrayNode)
        {
            if (arrayNode.Length != 2)
            {
                throw new Exception();
            }

            var elementOne = NodeTranslator.Translate(context, arrayNode[0]);
            var elementTwoContext = new TranslationContext(elementOne.Type, elementOne, context);
            var elementTwo = NodeTranslator.Translate(elementTwoContext, arrayNode[1]);

            return Expression.Subtract(elementOne, elementTwo);
        }

        return Expression.Subtract(context.Expression, NodeTranslator.Translate(context, node));
    }

    public Expression TranslateDivide(TranslationContext context, Node node)
    {
        if (node is ArrayNode arrayNode)
        {
            if (arrayNode.Length != 2)
            {
                throw new Exception();
            }

            var elementOne = NodeTranslator.Translate(context, arrayNode[0]);
            var elementTwoContext = new TranslationContext(elementOne.Type, elementOne, context);
            var elementTwo = NodeTranslator.Translate(elementTwoContext, arrayNode[1]);

            return Expression.Divide(elementOne, elementTwo);
        }

        return Expression.Divide(context.Expression, NodeTranslator.Translate(context, node));
    }

    public Expression TranslateMultiply(TranslationContext context, Node node)
    {
        if (node is ArrayNode arrayNode)
        {
            if (arrayNode.Length != 2)
            {
                throw new Exception();
            }

            var elementOne = NodeTranslator.Translate(context, arrayNode[0]);
            var elementTwoContext = new TranslationContext(elementOne.Type, elementOne, context);
            var elementTwo = NodeTranslator.Translate(elementTwoContext, arrayNode[1]);

            return Expression.Multiply(elementOne, elementTwo);
        }

        return Expression.Multiply(context.Expression, NodeTranslator.Translate(context, node));
    }

    public Expression TranslateModulo(TranslationContext context, Node node)
    {
        if (node is ArrayNode arrayNode)
        {
            if (arrayNode.Length != 2)
            {
                throw new Exception();
            }

            var elementOne = NodeTranslator.Translate(context, arrayNode[0]);
            var elementTwoContext = new TranslationContext(elementOne.Type, elementOne, context);
            var elementTwo = NodeTranslator.Translate(elementTwoContext, arrayNode[1]);

            return Expression.Modulo(elementOne, elementTwo);
        }

        return Expression.Modulo(context.Expression, NodeTranslator.Translate(context, node));
    }

}

public class RelationalOperatorsTranslator
{
    private TranslatorOptions Options { get; }
    private NodeTranslator NodeTranslator { get; }

    public RelationalOperatorsTranslator(TranslatorOptions options, NodeTranslator nodeParser)
    {
        Options = options;
        NodeTranslator = nodeParser;
    }

    public Expression TranslateEquals(TranslationContext context, Node node)
    {
        var lhs = null as Expression;
        var rhs = null as Expression;

        if (node is not ArrayNode arrayNode)
        {
            lhs = context.Expression;
            rhs = NodeTranslator.Translate(context, node);
        }
        else
        {
            if (arrayNode.Length != 2)
            {
                throw TranslationThrowHelper.ArraySyntaxBinaryExprWrongArgumentsCount(context, null);
            }

            lhs = NodeTranslator.Translate(context, arrayNode[0]);
            var rhsContext = new TranslationContext(lhs.Type, lhs, context);
            rhs = NodeTranslator.Translate(rhsContext, arrayNode[1]);
        }

        return Expression.Equal(lhs, rhs);
    }

    public Expression TranslateNotEquals(TranslationContext context, Node node)
    {
        var lhs = null as Expression;
        var rhs = null as Expression;

        if (node is not ArrayNode arrayNode)
        {
            lhs = context.Expression;
            rhs = NodeTranslator.Translate(context, node);
        }
        else
        {
            if (arrayNode.Length != 2)
            {
                throw TranslationThrowHelper.ArraySyntaxBinaryExprWrongArgumentsCount(context, null);
            }

            lhs = NodeTranslator.Translate(context, arrayNode[0]);
            var rhsContext = new TranslationContext(lhs.Type, lhs, context);
            rhs = NodeTranslator.Translate(rhsContext, arrayNode[1]);
        }

        return Expression.NotEqual(lhs, rhs);
    }

    public Expression TranslateLess(TranslationContext context, Node node)
    {
        var lhs = null as Expression;
        var rhs = null as Expression;

        if (node is not ArrayNode arrayNode)
        {
            lhs = context.Expression;
            rhs = NodeTranslator.Translate(context, node);
        }
        else
        {
            if (arrayNode.Length != 2)
            {
                throw TranslationThrowHelper.ArraySyntaxBinaryExprWrongArgumentsCount(context, null);
            }

            lhs = NodeTranslator.Translate(context, arrayNode[0]);
            var rhsContext = new TranslationContext(lhs.Type, lhs, context);
            rhs = NodeTranslator.Translate(rhsContext, arrayNode[1]);
        }

        return Expression.LessThan(lhs, rhs);
    }

    public Expression TranslateLessEquals(TranslationContext context, Node node)
    {
        var lhs = null as Expression;
        var rhs = null as Expression;

        if (node is not ArrayNode arrayNode)
        {
            lhs = context.Expression;
            rhs = NodeTranslator.Translate(context, node);
        }
        else
        {
            if (arrayNode.Length != 2)
            {
                throw TranslationThrowHelper.ArraySyntaxBinaryExprWrongArgumentsCount(context, null);
            }

            lhs = NodeTranslator.Translate(context, arrayNode[0]);
            var rhsContext = new TranslationContext(lhs.Type, lhs, context);
            rhs = NodeTranslator.Translate(rhsContext, arrayNode[1]);
        }

        return Expression.LessThanOrEqual(lhs, rhs);
    }

    public Expression TranslateGreater(TranslationContext context, Node node)
    {
        var lhs = null as Expression;
        var rhs = null as Expression;

        if (node is not ArrayNode arrayNode)
        {
            lhs = context.Expression;
            rhs = NodeTranslator.Translate(context, node);
        }
        else
        {
            if (arrayNode.Length != 2)
            {
                throw TranslationThrowHelper.ArraySyntaxBinaryExprWrongArgumentsCount(context, null);
            }

            lhs = NodeTranslator.Translate(context, arrayNode[0]);
            var rhsContext = new TranslationContext(lhs.Type, lhs, context);
            rhs = NodeTranslator.Translate(rhsContext, arrayNode[1]);
        }

        return Expression.GreaterThan(lhs, rhs);
    }

    public Expression TranslateGreaterEquals(TranslationContext context, Node node)
    {
        var lhs = null as Expression;
        var rhs = null as Expression;

        if (node is not ArrayNode arrayNode)
        {
            lhs = context.Expression;
            rhs = NodeTranslator.Translate(context, node);
        }
        else
        {
            if (arrayNode.Length != 2)
            {
                throw TranslationThrowHelper.ArraySyntaxBinaryExprWrongArgumentsCount(context, null);
            }

            lhs = NodeTranslator.Translate(context, arrayNode[0]);
            var rhsContext = new TranslationContext(lhs.Type, lhs, context);
            rhs = NodeTranslator.Translate(rhsContext, arrayNode[1]);
        }

        return Expression.GreaterThanOrEqual(lhs, rhs);
    }

    private Expression GetLeftSide(TranslationContext context, Node node)
    {
        if (node is not ArrayNode arrayNode)
        {
            return context.Expression;
        }

        if (arrayNode.Length != 2)
        {
            throw new Exception();
        }

        return NodeTranslator.Translate(context, arrayNode[0]);
    }

    private Expression GetRightSide(TranslationContext context, Node node)
    {
        if (node is not ArrayNode arrayNode)
        {
            return NodeTranslator.Translate(context, node);
        }

        if (arrayNode.Length != 2)
        {
            throw new Exception();
        }

        var lhsExpression = GetLeftSide(context, arrayNode);
        var rhsContext = new TranslationContext(lhsExpression.Type, lhsExpression, context);

        return NodeTranslator.Translate(rhsContext, arrayNode[1]);
    }

}

public class PatternRelationalOperatorsTranslator
{
    private TranslatorOptions Options { get; }
    private NodeTranslator NodeTranslator { get; }

    public PatternRelationalOperatorsTranslator(TranslatorOptions options, NodeTranslator nodeParser)
    {
        Options = options;
        NodeTranslator = nodeParser;
    }

    public Expression TranslateLike(TranslationContext context, Node node)
    {
        return Options.LinqProvider.TranslateLikeExpression(context, NodeTranslator, node);
    }

    public Expression TranslateRegexMatch(TranslationContext context, Node node)
    {
        throw new NotImplementedException();
        //return Expression.Equal(GetLeftSide(context, node), GetRightSide(context, node));
    }

}

public class LogicalOperatorsTranslator
{
    private TranslatorOptions Options { get; }
    private NodeTranslator NodeTranslator { get; }

    public LogicalOperatorsTranslator(TranslatorOptions options, NodeTranslator nodeTranslator)
    {
        Options = options;
        NodeTranslator = nodeTranslator;
    }

    public Expression TranslateOr(TranslationContext context, Node node)
    {
        if (node is not ArrayNode array)
        {
            throw new Exception();
        }

        var values = array.Values;
        var objects = new List<ObjectNode>();

        for (int i = 0; i < values.Length; i++)
        {
            var item = values[i];

            if (item is not ObjectNode objectNode)
            {
                throw new Exception();
            }

            objects.Add(objectNode);
        }

        if (objects.Count == 0)
        {
            return Expression.Constant(true, typeof(bool));
        }

        if (objects.Count == 1)
        {
            return NodeTranslator.Translate(context, objects[0]);
        }

        var expression = null as Expression;

        foreach (var item in objects)
        {
            if (expression == null)
            {
                expression = NodeTranslator.Translate(context, item);
                continue;
            }

            expression = Expression.OrElse(expression, NodeTranslator.Translate(context, item));
        }

        if (expression == null)
        {
            throw new InvalidOperationException();
        }

        return expression;
    }

    public Expression TranslateAnd(TranslationContext context, Node node)
    {
        if (node is not ArrayNode array)
        {
            throw new Exception();
        }

        var values = array.Values;
        var objects = new List<ObjectNode>();

        for (int i = 0; i < values.Length; i++)
        {
            var item = values[i];

            if (item is not ObjectNode objectNode)
            {
                throw new Exception();
            }

            objects.Add(objectNode);
        }

        if (objects.Count == 0)
        {
            return Expression.Constant(true, typeof(bool));
        }

        if (objects.Count == 1)
        {
            return NodeTranslator.Translate(context, objects[0]);
        }

        var expression = null as Expression;

        foreach (var item in objects)
        {
            if (expression == null)
            {
                expression = NodeTranslator.Translate(context, item);
                continue;
            }

            expression = Expression.AndAlso(expression, NodeTranslator.Translate(context, item));
        }

        if (expression == null)
        {
            throw new InvalidOperationException();
        }

        return expression;
    }

    public Expression TranslateNot(TranslationContext context, Node node)
    {
        var expression = NodeTranslator.Translate(context, node);

        if (expression.Type != typeof(bool))
        {
            throw new Exception();
        }

        return Expression.Not(expression);
    }

}

public class SemanticOperatorsTranslator
{
    private TranslatorOptions Options { get; }
    private NodeTranslator NodeTranslator { get; }

    public SemanticOperatorsTranslator(TranslatorOptions options, NodeTranslator nodeTranslator)
    {
        Options = options;
        NodeTranslator = nodeTranslator;
    }

    public Expression TranslateLiteral(TranslationContext context, Node node)
    {
        var type = context.Type;
        var value = JsonSerializerSingleton.Deserialize(node.ToString(), type);

        return Expression.Constant(value, type);
    }

    public Expression TranslateSelect(TranslationContext context, Node node)
    {
        if (node is not LiteralNode literal)
        {
            throw new Exception();
        }

        if (literal.Value == null)
        {
            throw new Exception();
        }
        if (!literal.IsReference)
        {
            throw new Exception();
        }

        return NodeTranslator.Translate(context, literal);
    }

    public Expression TranslateExpr(TranslationContext context, Node node)
    {
        return NodeTranslator.Translate(context.CreateChildContext(), node);
    }

}

public class QueryableOperatorsTranslator
{
    private TranslatorOptions Options { get; }
    private NodeTranslator NodeTranslator { get; }

    public QueryableOperatorsTranslator(TranslatorOptions options, NodeTranslator nodeTranslator)
    {
        Options = options;
        NodeTranslator = nodeTranslator;
    }

    public Expression TranslateFilter(TranslationContext context, Node node)
    {
        return Options.LinqProvider.TranslateFilterOperator(context, NodeTranslator, node);
    }

    public Expression TranslateProject(TranslationContext context, Node node)
    {
        return Options.LinqProvider.TranslateProjectOperator(context, NodeTranslator, node);
    }

    public Expression TranslateTransform(TranslationContext context, Node node)
    {
        return Options.LinqProvider.TranslateTransformOperator(context, NodeTranslator, node);
    }

    public Expression TranslateSelectMany(TranslationContext context, Node node)
    {
        return Options.LinqProvider.TranslateSelectManyOperator(context, NodeTranslator, node);
    }

    public Expression TranslateLimit(TranslationContext context, Node node)
    {
        return Options.LinqProvider.TranslateLimitOperator(context, NodeTranslator, node);
    }

    public Expression TranslateSkip(TranslationContext context, Node node)
    {
        return Options.LinqProvider.TranslateSkipOperator(context, NodeTranslator, node);
    }

    public Expression TranslateCount(TranslationContext context, Node node)
    {
        return Options.LinqProvider.TranslateCountOperator(context, NodeTranslator, node);
    }

    public Expression TranslateAny(TranslationContext context, Node node)
    {
        return Options.LinqProvider.TranslateAnyOperator(context, NodeTranslator, node);
    }

    public Expression TranslateAll(TranslationContext context, Node node)
    {
        return Options.LinqProvider.TranslateAllOperator(context, NodeTranslator, node);
    }

    public Expression TranslateMin(TranslationContext context, Node node)
    {
        return Options.LinqProvider.TranslateMinOperator(context, NodeTranslator, node);
    }

}
