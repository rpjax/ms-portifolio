using ModularSystem.Core.Extensions;
using System.Linq.Expressions;

namespace ModularSystem.Core.Linq.Expressions.Visitors;

/// <summary>
/// A specialized expression visitor that binds parameter expressions to existing references. <br/>
/// </summary>
public class ParameterExpressionReferenceBinder : ExpressionVisitor
{
    /// <summary>
    /// Stores a reference table of parameter expressions.
    /// </summary>
    private ExpressionReferenceTable ReferenceTable { get; set; } = new();

    /// <summary>
    /// Visits and potentially modifies a lambda expression, managing parameter references.
    /// </summary>
    /// <typeparam name="T">The type of delegate of the lambda expression.</typeparam>
    /// <param name="node">The lambda expression to visit.</param>
    /// <returns>The modified lambda expression, with parameter references handled.</returns>
    protected override Expression VisitLambda<T>(Expression<T> node)
    {
        if (node.NodeType != ExpressionType.Lambda)
        {
            return node;
        }

        var lambda = node.TypeCast<LambdaExpression>();
        var parameters = lambda.Parameters;

        ReferenceTable = new(ReferenceTable);

        foreach (var item in parameters)
        {
            if (ReferenceTable.Contains(item))
            {
                continue;
            }

            ReferenceTable.Add(item);
        }

        return base.VisitLambda(node);
    }

    /// <summary>
    /// Visits and potentially replaces a parameter expression with a referenced one from the reference table.
    /// </summary>
    /// <param name="node">The parameter expression to visit.</param>
    /// <returns>The referenced parameter expression if it exists in the table, otherwise the original expression.</returns>
    protected override Expression VisitParameter(ParameterExpression node)
    {
        var reference = ReferenceTable.GetReference(node);

        if (reference != null)
        {
            return reference;
        }

        return base.VisitParameter(node);
    }

    /// <summary>
    /// An internal class that manages a table of parameter expressions, allowing for reference lookup and management within a given scope.
    /// </summary>
    internal class ExpressionReferenceTable
    {
        /// <summary>
        /// Optional parent scope to enable hierarchical scope management.
        /// </summary>
        private ExpressionReferenceTable? ParentScope { get; }

        /// <summary>
        /// A list of parameter expressions within the current scope.
        /// </summary>
        private List<ParameterExpression> Parameters { get; } = new();

        /// <summary>
        /// Constructs a new instance of ExpressionReferenceTable, optionally with a parent scope.
        /// </summary>
        /// <param name="parentScope">The parent scope for nested scope management (optional).</param>
        public ExpressionReferenceTable(ExpressionReferenceTable? parentScope = null)
        {
            ParentScope = parentScope;
            Parameters = new();
        }

        /// <summary>
        /// Retrieves a matching reference for a given parameter expression from the table, considering name and type.
        /// </summary>
        /// <param name="expression">The parameter expression to find a reference for.</param>
        /// <returns>The referenced parameter expression if a match is found, otherwise null.</returns>
        public ParameterExpression? GetReference(ParameterExpression expression)
        {
            var scope = this;

            while (true)
            {
                var fullMatch = scope.Parameters
                .Where(x => x.Name == expression.Name && x.Type == expression.Type)
                .ToArray();

                if (fullMatch.IsNotEmpty())
                {
                    if (fullMatch.Length > 1)
                    {
                        throw new InvalidOperationException();
                    }

                    return fullMatch.First();
                }

                var typeMatch = scope.Parameters
                    .Where(x => x.Type == expression.Type)
                    .ToArray();

                if (typeMatch.IsNotEmpty())
                {
                    if (typeMatch.Length > 1)
                    {
                        throw new InvalidOperationException();
                    }

                    return typeMatch.First();
                }

                if (scope.ParentScope == null)
                {
                    break;
                }

                scope = scope.ParentScope;
            }

            return null;
        }

        /// <summary>
        /// Checks if a given parameter expression is already contained within the reference table.
        /// </summary>
        /// <param name="expression">The parameter expression to check for.</param>
        /// <returns>True if the expression is contained within the table, false otherwise.</returns>
        public bool Contains(ParameterExpression expression)
        {
            return GetReference(expression) != null;
        }

        /// <summary>
        /// Adds a new parameter expression to the reference table.
        /// </summary>
        /// <param name="value">The parameter expression to add.</param>
        public void Add(ParameterExpression value)
        {
            if (Parameters.Contains(value))
            {
                throw new Exception();
            }

            Parameters.Add(value);
        }
    }
}
