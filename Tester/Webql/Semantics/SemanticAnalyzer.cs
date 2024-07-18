using Webql.Semantics.Extensions;
using Webql.Parsing.Ast;
using Webql.Semantics.Definitions;
using Webql.Core;
using Webql.Semantics.Exceptions;
using Webql.Core.Analysis;
using Webql.Semantics.Symbols;
using Webql.Core.Extensions;
using ModularSystem.Core.Reflection;
using System.Reflection;

namespace Webql.Semantics.Analysis;

public static class SemanticAnalyzer
{
    /*
     * Main API for using the semantic analyzer.
     */

    //*
    public static void BindSemanticsToAst(WebqlCompilationContext context, WebqlSyntaxNode node)
    {
        node.BindCompilationContext(context);
        BindScopes(node);
        DeclareSymbols(node);
    }

    private static void BindScopes(WebqlSyntaxNode node)
    {
        new ScopeBinderAnalyzer()
            .ExecuteAnalysis(node);
    }

    private static void DeclareSymbols(WebqlSyntaxNode node)
    {
        new SymbolDeclaratorAnalyzer()
            .ExecuteAnalysis(node);
    }

    //*
    public static void ExecuteSemanticalAnalysis(ref WebqlSyntaxNode node)
    {
        ExecutePreValidationRewrites(ref node);
        ValidateSemantics(node);
        ExecutePostValidationsRewrites(ref node);
    }

    private static void ValidateSemantics(WebqlSyntaxNode node)
    {
        ValidateOperatorArity(node);
        ValidateOperatorTypes(node);
    }

    private static void ValidateOperatorArity(WebqlSyntaxNode node)
    {
        // TODO: Implement this
        //new OperandArityValidatorAnalyzer()
        //    .ExecuteAnalysis(node);
    }

    private static void ValidateOperatorTypes(WebqlSyntaxNode node)
    {
        new TypeValidatorAnalyzer()
            .ExecuteAnalysis(node);
    }

    private static void ExecutePreValidationRewrites(ref WebqlSyntaxNode node)
    {
        var context = node.GetCompilationContext();

        foreach (var visitor in context.Settings.PreValidationVisitors)
        {
            node = visitor.Visit(node);
        }

        // Adds type conversions where necessary. E.g. `1 + 1.0` => `1 + (int)1.0`
        node = new ImplicitTypeConversionRewriter().Visit(node);
    }

    private static void ExecutePostValidationsRewrites(ref WebqlSyntaxNode node)
    {
        var context = node.GetCompilationContext();

        foreach (var visitor in context.Settings.PostValidationVisitors)
        {
            node = visitor.Visit(node);
        }
    }

    /*
     * Main entry point for the creation of semantics.
     * 
     * NOTE: 'Semantics' refers to the type information and other metadata that is bound to the AST nodes. 
     * It is used to perform assertions and transformations on the AST, and to generate the final output.
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

            case WebqlNodeType.AnonymousObjectProperty:
                return CreateAnonymousObjectPropertySemantics(context, (WebqlAnonymousObjectProperty)node);

            default:
                throw new InvalidOperationException();
        }
    }

    /*
     * QUERY SEMANTICS
     */

    private static IQuerySemantics CreateQuerySemantics(
        WebqlCompilationContext context,
        WebqlQuery query)
    {
        if (query.Expression is null)
        {
            return new QuerySemantics(typeof(void));
        }

        var expressionSemantics = query.Expression.GetSemantics<IExpressionSemantics>();

        return new QuerySemantics(
            type: expressionSemantics.Type
        );
    }

    /*
     * EXPRESSION SEMANTICS
     */

    private static IExpressionSemantics CreateExpressionSemantics(
        WebqlCompilationContext context,
        WebqlExpression expression)
    {
        switch (expression.ExpressionType)
        {
            case WebqlExpressionType.Literal:
                return CreateLiteralExpressionSemantics(context, (WebqlLiteralExpression)expression);

            case WebqlExpressionType.Reference:
                return CreateReferenceExpressionSemantics(context, (WebqlReferenceExpression)expression);

            case WebqlExpressionType.MemberAccess:
                return CreateMemberAccessExpressionSemantics(context, (WebqlMemberAccessExpression)expression);

            case WebqlExpressionType.TemporaryDeclaration:
                return CreateTemporaryDeclarationExpressionSemantics(context, (WebqlTemporaryDeclarationExpression)expression);

            case WebqlExpressionType.Operation:
                return CreateOperationExpressionSemantics(context, (WebqlOperationExpression)expression);

            case WebqlExpressionType.TypeConversion:
                return CreateTypeConversionExpressionSemantics(context, (WebqlTypeConversionExpression)expression);

            case WebqlExpressionType.AnonymousObject:
                return CreateAnonymousObjectSemantics(context, (WebqlAnonymousObjectExpression)expression);

            default:
                throw new InvalidOperationException();
        }
    }

    /*
     * LITERAL EXPRESSION SEMANTICS
     */

    private static IExpressionSemantics CreateLiteralExpressionSemantics(
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

    private static IExpressionSemantics CreateBoolSemantics(
        WebqlCompilationContext context,
        WebqlLiteralExpression expression)
    {
        return new ExpressionSemantics(
            type: typeof(bool)
        );
    }

    private static IExpressionSemantics CreateNullSemantics(
        WebqlCompilationContext context,
        WebqlLiteralExpression expression)
    {
        if (expression.Parent is not WebqlOperationExpression operationNode)
        {
            throw new SemanticException("Null literal can only be used as an operand to an operator.", expression);
        }

        if (operationNode.Operands.Length != 2)
        {
            throw new SemanticException("Null literal can only be used as an operand to a binary operator.", expression);
        }

        var otherOperand = operationNode.Operands.First(x => x != expression);
        var otherSemantics = otherOperand.GetSemantics<IExpressionSemantics>();

        //var type = SemanticsTypeHelper.NormalizeNullableType(otherSemantics.Type);
        //var nullableType = null as Type;

        //if (otherSemantics.Type.IsValueType)
        //{
        //    nullableType = typeof(Nullable<>).MakeGenericType(type);
        //}
        //else
        //{
        //    nullableType = type; // If it's not a value type, don't attempt to make it nullable
        //}

        return new ExpressionSemantics(
            type: otherSemantics.Type
        );
    }

    private static IExpressionSemantics CreateIntLiteralSemantics(
        WebqlCompilationContext context,
        WebqlLiteralExpression expression)
    {
        return new ExpressionSemantics(
            type: typeof(int)
        );
    }

    private static IExpressionSemantics CreateFloatLiteralSemantics(
        WebqlCompilationContext context,
        WebqlLiteralExpression expression)
    {
        return new ExpressionSemantics(
            type: typeof(float)
        );
    }

    private static IExpressionSemantics CreateHexLiteralSemantics(
        WebqlCompilationContext context,
        WebqlLiteralExpression expression)
    {
        throw new NotImplementedException();
    }

    private static IExpressionSemantics CreateStringLiteralSemantics(
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

    private static IExpressionSemantics CreateReferenceExpressionSemantics(
        WebqlCompilationContext context,
        WebqlReferenceExpression referenceExpression)
    {
        var symbol = referenceExpression.ResolveSymbol<ITypedSymbol>(referenceExpression.Identifier);

        return new ExpressionSemantics(
            type: symbol.Type
        );
    }

    /*
     * SCOPE ACCESS EXPRESSION SEMANTICS
     */

    private static IMemberAccessSemantics CreateMemberAccessExpressionSemantics(
        WebqlCompilationContext context,
        WebqlMemberAccessExpression memberAccessExpression)
    {
        var childSemantics = memberAccessExpression.Expression.GetSemantics<IExpressionSemantics>();
        var childType = childSemantics.Type;
        var memberName = memberAccessExpression.MemberName;

        var propertyInfo = SemanticsTypeHelper.TryGetPropertyFromType(childType, memberName);

        if (propertyInfo is null)
        {
            throw memberAccessExpression.CreatePropertyNotFoundException(childType, memberName);
        }

        return new MemberAccessSemantics(
            type: propertyInfo.PropertyType,
            propertyInfo: propertyInfo
        );
    }

    /*
     * TEMPORARY DECLARATION EXPRESSION SEMANTICS
     */

    private static IExpressionSemantics CreateTemporaryDeclarationExpressionSemantics(
        WebqlCompilationContext context,
        WebqlTemporaryDeclarationExpression temporaryDeclarationExpression)
    {
        throw new NotImplementedException();
        var valueSemantics = temporaryDeclarationExpression.Value.GetExpressionSemantics();

        return new ExpressionSemantics(
            type: valueSemantics.Type
        );
    }

    /*
     * OPERATION EXPRESSION SEMANTICS
     */

    private static IExpressionSemantics CreateOperationExpressionSemantics(
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

    private static IExpressionSemantics CreateArithmeticOperationExpressionSemantics(
        WebqlCompilationContext context,
        WebqlOperationExpression operationExpression)
    {
        operationExpression.EnsureOperandCount(2);

        var lhs = operationExpression.Operands[0];
        var lhsSemantics = lhs.GetSemantics<IExpressionSemantics>();

        return new ExpressionSemantics(
            type: lhsSemantics.Type
        );
    }

    private static IExpressionSemantics CreateRelationalOperationExpressionSemantics(
        WebqlCompilationContext context,
        WebqlOperationExpression operationExpression)
    {
        return new ExpressionSemantics(
            type: typeof(bool)
        );
    }

    private static IExpressionSemantics CreateStringRelationalOperationExpressionSemantics(
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

    private static IExpressionSemantics CreateLogicalOperationExpressionSemantics(
        WebqlCompilationContext context,
        WebqlOperationExpression operationExpression)
    {
        return new ExpressionSemantics(
            type: typeof(bool)
        );
    }

    private static IExpressionSemantics CreateSemanticOperationExpressionSemantics(
        WebqlCompilationContext context,
        WebqlOperationExpression operationExpression)
    {
        switch (WebqlOperatorAnalyzer.GetSemanticOperator(operationExpression.Operator))
        {
            case WebqlSemanticOperator.Aggregate:
                return CreateAggregateExpressionSemantics(context, operationExpression);

            default:
                break;
        }

        throw new NotImplementedException();
    }

    /*
     * SEMANTIC OPERATION EXPRESSION SEMANTICS
     */

    private static IExpressionSemantics CreateAggregateExpressionSemantics(
        WebqlCompilationContext context,
        WebqlOperationExpression operationExpression)
    {
        operationExpression.EnsureAtLeastOneOperand();

        var lastOperand = operationExpression.Operands.Last();
        var lastOperandSemantics = lastOperand.GetSemantics<IExpressionSemantics>();

        return new ExpressionSemantics(
            type: lastOperandSemantics.Type
        );
    }

    /*
     * COLLECTION MANIPULATION OPERATION EXPRESSION SEMANTICS
     */

    private static IExpressionSemantics CreateCollectionManipulationOperationExpressionSemantics(
        WebqlCompilationContext context,
        WebqlOperationExpression operationExpression)
    {
        operationExpression.EnsureOperandCount(2);

        var lhs = operationExpression.Operands[0];
        var lhsSemantics = lhs.GetSemantics<IExpressionSemantics>();
        var lhsType = lhsSemantics.Type;

        lhs.EnsureIsQueryable();

        var elementType = lhsType.GetQueryableElementType();

        var type = operationExpression.GetQueryableType(context)
            .MakeGenericType(elementType);

        return new ExpressionSemantics(
            type: type
        );
    }

    /*
     * COLLECTION AGGREGATION OPERATION EXPRESSION SEMANTICS
     */

    private static IExpressionSemantics CreateCollectionAggregationOperationExpressionSemantics(
        WebqlCompilationContext context,
        WebqlOperationExpression node)
    {
        var operatorType = node.GetCollectionAggregationOperator();

        switch (operatorType)
        {
            case WebqlCollectionAggregationOperator.Count:
                return CreateCountExpressionSemantics(node);

            case WebqlCollectionAggregationOperator.Index:
                return CreateIndexExpressionSemantics(node);

            case WebqlCollectionAggregationOperator.Contains:
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

                node.EnsureOperandCount(2);

                var lhs = node.Operands[0];
                var rhs = node.Operands[1];

                var lhsSemantics = lhs.GetSemantics<IExpressionSemantics>();
                var rhsSemantics = rhs.GetSemantics<IExpressionSemantics>();

                return new ExpressionSemantics(
                    type: rhsSemantics.Type
                );

            default:
                throw new InvalidOperationException("Invalid collection aggregation operator.");
        }
    }

    private static IExpressionSemantics CreateCountExpressionSemantics(
        WebqlOperationExpression node)
    {
        return new ExpressionSemantics(
            type: typeof(int)
        );
    }

    private static IExpressionSemantics CreateIndexExpressionSemantics(
        WebqlOperationExpression node)
    {
        node.EnsureOperandCount(2);

        var expression = node.Operands[0];
        expression.EnsureIsQueryable();

        var expressionSemantics = node.GetSemantics<IExpressionSemantics>();
        var elementType = expressionSemantics.Type.GetQueryableElementType();

        return new ExpressionSemantics(
            type: elementType
        );
    }

    /*
     * TYPE CONVERSION EXPRESSION SEMANTICS
     */

    private static IExpressionSemantics CreateTypeConversionExpressionSemantics(
        WebqlCompilationContext context,
        WebqlTypeConversionExpression expression)
    {
        return new ExpressionSemantics(
            type: expression.TargetType
        );
    }

    /*
     * ANONYMOUS OBJECT EXPRESSION SEMANTICS
     */

    private static IAnonymousObjectSemantics CreateAnonymousObjectSemantics(
        WebqlCompilationContext context,
        WebqlAnonymousObjectExpression expression)
    {
        var propertyDefinitions = new List<AnonymousPropertyDefinition>();

        foreach (var property in expression.Properties)
        {
            var valueSemantics = property.Value.GetExpressionSemantics();

            var propertyDefinition = new AnonymousPropertyDefinition(
                name: property.Name,
                type: valueSemantics.Type
            );

            propertyDefinitions.Add(propertyDefinition);
        }

        var typeCreaionOptions = new AnonymousTypeCreationOptions(
            name: null,
            properties: propertyDefinitions,
            createDefaultConstructor: true,
            createSetters: true
        );

        var type = TypeCreator.CreateAnonymousType(typeCreaionOptions);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        return new AnonymousObjectSemantics(
            type: type
        );
    }

    private static IAnonymousObjectPropertySemantics CreateAnonymousObjectPropertySemantics(
        WebqlCompilationContext context,
        WebqlAnonymousObjectProperty expression)
    {
        var parent = (expression.Parent as WebqlAnonymousObjectExpression)!;
        var parentSemantics = parent.GetAnonymousObjectSemantics();
        var expressionSemantics = expression.Value.GetExpressionSemantics();

        var propertyInfo = parentSemantics.Type.GetProperty(expression.Name);

        if(propertyInfo is null)
        {
            throw new SemanticException($"Property '{expression.Name}' not found in the projected type '{parentSemantics.Type.Name}'. Ensure the property name is correctly defined in the projection.", expression);
        }

        return new AnonymousObjectPropertySemantics(
            name: expression.Name,
            type: propertyInfo.PropertyType,
            propertyInfo: propertyInfo
        );
    }

}
