using Webql.Semantics.Extensions;
using Webql.Parsing.Ast;
using Webql.Semantics.Definitions;
using Webql.Core;
using Webql.Semantics.Context;

namespace Webql.Semantics.Analysis;

public static class SemanticAnalyzer
{
    public static void ExecuteAnalysisPipeline(SemanticContext context, WebqlSyntaxNode node)
    {
        node = RewriteTree(context, node);

        AnnotateTree(context, node);
        DeclareSymbols(context, node);
        ValidateTypes(context, node);
    }

    public static WebqlSyntaxNode RewriteTree(SemanticContext context, WebqlSyntaxNode node)
    {
        return new DocumentSyntaxTreeRewriter()
            .ExecuteRewrite(node);
    }

    public static void AnnotateTree(SemanticContext context, WebqlSyntaxNode node)
    {
        new SemanticContextBinderAnalyzer(context)
            .ExecuteAnalysis(node);
    }

    public static void DeclareSymbols(SemanticContext context, WebqlSyntaxNode node)
    {
        new SymbolDeclaratorAnalyzer(context)    
            .ExecuteAnalysis(node);
    }

    public static void ValidateTypes(SemanticContext context, WebqlSyntaxNode node)
    {
        new TypeValidatorAnalyzer(context)
            .ExecuteAnalysis(node);
    }

    /*
     * Main entry point for the semantic analysis.
     */

    public static ISemantics CreateSemantics(
        WebqlCompilationContext context,
        WebqlSyntaxNode node)
    {
        switch (node.NodeType)
        {
            case WebqlNodeType.Query:
                return CreateQuerySemantics(context, (WebqlQuery)node);

            case WebqlNodeType.Expression:
                return CreateExpressionSemantics(context, (WebqlExpression)node);

            default:
                throw new InvalidOperationException();
        }
    }

    /*
     * QUERY SEMANTICS
     */

    public static IQuerySemantics CreateQuerySemantics(
        WebqlCompilationContext context,
        WebqlQuery query)
    {
        if (query.Expression is null)
        {
            return new QuerySemantics(typeof(void));
        }

        return new QuerySemantics(
            type: query.Expression.GetSemantics<IExpressionSemantics>().Type
        );
    }

    /*
     * EXPRESSION SEMANTICS
     */
    public static IExpressionSemantics CreateExpressionSemantics(
        WebqlCompilationContext context,
        WebqlExpression expression)
    {
        switch (expression.ExpressionType)
        {
            case WebqlExpressionType.Literal:
                return CreateLiteralExpressionSemantics(context, (WebqlLiteralExpression)expression);

            case WebqlExpressionType.Reference:
                return CreateReferenceExpressionSemantics(context, (WebqlReferenceExpression)expression);

            case WebqlExpressionType.ScopeAccess:
                return CreateScopeAccessExpressionSemantics(context, (WebqlScopeAccessExpression)expression);

            case WebqlExpressionType.TemporaryDeclaration:
                return CreateTemporaryDeclarationExpressionSemantics(context, (WebqlTemporaryDeclarationExpression)expression);

            case WebqlExpressionType.Block:
                return CreateBlockExpressionSemantics(context, (WebqlBlockExpression)expression);

            case WebqlExpressionType.Operation:
                return CreateOperationExpressionSemantics(context, (WebqlOperationExpression)expression);

            default:
                throw new InvalidOperationException();
        }
    }

    /*
     * LITERAL EXPRESSION SEMANTICS
     */

    public static IExpressionSemantics CreateLiteralExpressionSemantics(
        WebqlCompilationContext context,
        WebqlLiteralExpression expression)
    {
        switch (expression.LiteralType)
        {
            case WebqlLiteralType.Bool:
                return CreateBoolSemantics(context, expression);

            case WebqlLiteralType.Null:
                return CreateNullSemantics(context, expression);

            case WebqlLiteralType.Int:
                return CreateIntLiteralSemantics(context, expression);

            case WebqlLiteralType.Float:
                return CreateFloatLiteralSemantics(context, expression);

            case WebqlLiteralType.Hex:
                return CreateHexLiteralSemantics(context, expression);

            case WebqlLiteralType.String:
                return CreateStringLiteralSemantics(context, expression);

            default:
                throw new InvalidOperationException();
        }
    }

    public static IExpressionSemantics CreateBoolSemantics(
        WebqlCompilationContext context,
        WebqlLiteralExpression expression)
    {
        return new ExpressionSemantics(
            type: typeof(bool)
        );
    }

    public static IExpressionSemantics CreateNullSemantics(
        WebqlCompilationContext context,
        WebqlLiteralExpression expression)
    {
        var semanticContext = expression.GetSemanticContext();
        var lhsSemantics = semanticContext.GetLeftHandSideSymbol();
        var lhsType = SemanticsTypeHelper.NormalizeNullableType(lhsSemantics.Type);
        var nullableType = typeof(Nullable<>).MakeGenericType(lhsType);

        return new ExpressionSemantics(
            type: nullableType
        );
    }

    public static IExpressionSemantics CreateIntLiteralSemantics(
        WebqlCompilationContext context,
        WebqlLiteralExpression expression)
    {
        return new ExpressionSemantics(
            type: typeof(int)
        );
    }

    public static IExpressionSemantics CreateFloatLiteralSemantics(
        WebqlCompilationContext context,
        WebqlLiteralExpression expression)
    {
        return new ExpressionSemantics(
            type: typeof(float)
        );
    }

    public static IExpressionSemantics CreateHexLiteralSemantics(
        WebqlCompilationContext context,
        WebqlLiteralExpression expression)
    {
        throw new NotImplementedException();
    }

    public static IExpressionSemantics CreateStringLiteralSemantics(
        WebqlCompilationContext context,
        WebqlLiteralExpression expression)
    {
        return new ExpressionSemantics(
            type: typeof(string)
        );
    }

    /*
     * REFERENCE EXPRESSION SEMANTICS
     */

    public static IExpressionSemantics CreateReferenceExpressionSemantics(
        WebqlCompilationContext context,
        WebqlReferenceExpression referenceExpression)
    {
        var semanticContext = referenceExpression.GetSemanticContext();

        return new ExpressionSemantics(
            type: semanticContext.GetSymbolType(referenceExpression.Identifier)
        );
    }

    /*
     * SCOPE ACCESS EXPRESSION SEMANTICS
     */

    public static IExpressionSemantics CreateScopeAccessExpressionSemantics(
        WebqlCompilationContext context,
        WebqlScopeAccessExpression scopeAccessExpression)
    {
        return scopeAccessExpression.Expression.GetSemantics<IExpressionSemantics>();
    }

    /*
     * TEMPORARY DECLARATION EXPRESSION SEMANTICS
     */

    public static IExpressionSemantics CreateTemporaryDeclarationExpressionSemantics(
        WebqlCompilationContext context,
        WebqlTemporaryDeclarationExpression temporaryDeclarationExpression)
    {
        throw new NotImplementedException();
    }

    /*
     * BLOCK EXPRESSION SEMANTICS
     */

    public static IExpressionSemantics CreateBlockExpressionSemantics(
        WebqlCompilationContext context,
        WebqlBlockExpression blockExpression)
    {
        return blockExpression.Expressions
            .Last()
            .GetSemantics<IExpressionSemantics>();
    }

    /*
     * OPERATION EXPRESSION SEMANTICS
     */

    public static IExpressionSemantics CreateOperationExpressionSemantics(
        WebqlCompilationContext context,
        WebqlOperationExpression operationExpression)
    {
        switch (operationExpression.GetOperatorCategory())
        {
            case WebqlOperatorCategory.Arithmetic:
                return CreateArithmeticOperationExpressionSemantics(context, operationExpression);

            case WebqlOperatorCategory.Relational:
                return CreateRelationalOperationExpressionSemantics(context, operationExpression);

            case WebqlOperatorCategory.StringRelational:
                return CreateStringRelationalOperationExpressionSemantics(context, operationExpression);

            case WebqlOperatorCategory.Logical:
                return CreateLogicalOperationExpressionSemantics(context, operationExpression);

            case WebqlOperatorCategory.Semantic:
                return CreateSemanticOperationExpressionSemantics(context, operationExpression);

            case WebqlOperatorCategory.CollectionManipulation:
                return CreateCollectionManipulationOperationExpressionSemantics(context, operationExpression);

            case WebqlOperatorCategory.CollectionAggregation:
                return CreateCollectionAggregationOperationExpressionSemantics(context, operationExpression);

            default:
                throw new InvalidOperationException("Invalid operator category.");
        }
    }

    public static IExpressionSemantics CreateArithmeticOperationExpressionSemantics(
        WebqlCompilationContext context,
        WebqlOperationExpression operationExpression)
    {
        var semanticContext = operationExpression.GetSemanticContext();
        var lhsType = semanticContext.GetLeftHandSideType();

        return new ExpressionSemantics(
            type: lhsType
        );
    }

    public static IExpressionSemantics CreateRelationalOperationExpressionSemantics(
        WebqlCompilationContext context,
        WebqlOperationExpression operationExpression)
    {
        return new ExpressionSemantics(
            type: typeof(bool)
        );
    }

    public static IExpressionSemantics CreateStringRelationalOperationExpressionSemantics(
        WebqlCompilationContext context,
        WebqlOperationExpression operationExpression)
    {
        /*
         * As of now, the language only supports logical operations on strings. $like and $regexMatch
         */
        return new ExpressionSemantics(
            type: typeof(bool)
        );
    }

    public static IExpressionSemantics CreateLogicalOperationExpressionSemantics(
        WebqlCompilationContext context,
        WebqlOperationExpression operationExpression)
    {
        return new ExpressionSemantics(
            type: typeof(bool)
        );
    }

    public static IExpressionSemantics CreateSemanticOperationExpressionSemantics(
        WebqlCompilationContext context,
        WebqlOperationExpression operationExpression)
    {
        throw new NotImplementedException();
    }

    public static IExpressionSemantics CreateCollectionManipulationOperationExpressionSemantics(
        WebqlCompilationContext context,
        WebqlOperationExpression operationExpression)
    {
        var semanticContext = operationExpression.GetSemanticContext();
        var type = context.QueryableType.MakeGenericType(semanticContext.GetLeftHandSideType());

        return new ExpressionSemantics(
            type: type
        );
    }

    public static IExpressionSemantics CreateCollectionAggregationOperationExpressionSemantics(
        WebqlCompilationContext context,
        WebqlOperationExpression operationExpression)
    {
        if (operationExpression.Operands.Length != 1)
        {
            throw new InvalidOperationException("Collection aggregation operators must have exactly one operand.");
        }

        var semanticContext = operationExpression.GetSemanticContext();
        var operatorType = WebqlOperatorClassifier.GetCollectionAggregationOperator(operationExpression.Operator);

        var rhsNode = operationExpression.Operands[0];
        var rhsSemantics = rhsNode.GetSemantics<IExpressionSemantics>();

        switch (operatorType)
        {
            case WebqlCollectionAggregationOperator.Count:
                return new ExpressionSemantics(
                    type: typeof(int)
                );

            case WebqlCollectionAggregationOperator.Index:
                return new ExpressionSemantics(
                    type: semanticContext.GetLeftHandSideType()
                );

            case WebqlCollectionAggregationOperator.Any:
            case WebqlCollectionAggregationOperator.All:
                return new ExpressionSemantics(
                    type: typeof(bool)
                );

            /*
            * Aggregation in wich the return type varies depending on the predicate.
            */
            case WebqlCollectionAggregationOperator.Min:
            case WebqlCollectionAggregationOperator.Max:
            case WebqlCollectionAggregationOperator.Sum:
            case WebqlCollectionAggregationOperator.Average:
                return new ExpressionSemantics(
                    type: rhsSemantics.Type
                );

            default:
                throw new InvalidOperationException("Invalid collection aggregation operator.");
        }
    }

}
