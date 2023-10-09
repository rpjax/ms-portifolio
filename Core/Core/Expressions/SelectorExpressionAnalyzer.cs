using System.Linq.Expressions;

namespace ModularSystem.Core;

/// <summary>
/// Analyzes and extracts information from a selector expression.
/// </summary>
/// <typeparam name="TEntity">The type of the entity being selected from.</typeparam>
/// <typeparam name="TField">The type of the field being selected.</typeparam>
public class SelectorExpressionAnalyzer<TEntity, TField> : ExpressionVisitor
{
    /// <summary>
    /// Gets the selector expression being analyzed.
    /// </summary>
    private Expression<Func<TEntity, TField>> SelectorExpression { get; }

    /// <summary>
    /// Gets the root parameter of the selector expression.
    /// </summary>
    private Expression? RootParameter { get; set; }

    /// <summary>
    /// Gets the fluent parameter used in the selector expression.
    /// </summary>
    private Expression? FluentParameter { get; set; }

    /// <summary>
    /// Gets the leaf member access within the selector expression.
    /// </summary>
    private MemberExpression? LeafMemberAccess { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectorExpressionAnalyzer{TEntity, TField}"/> class.
    /// </summary>
    /// <param name="expression">The selector expression to be analyzed.</param>
    public SelectorExpressionAnalyzer(Expression<Func<TEntity, TField>> expression)
    {
        SelectorExpression = expression;
        RootParameter = null;
        FluentParameter = null;
        LeafMemberAccess = null;
    }

    /// <summary>
    /// Executes the analysis on the selector expression.
    /// </summary>
    /// <returns>The current instance of the analyzer.</returns>
    public SelectorExpressionAnalyzer<TEntity, TField> Execute()
    {
        Visit(SelectorExpression);
        return this;
    }

    /// <summary>
    /// Retrieves the name of the field from the selector expression.
    /// </summary>
    /// <returns>The name of the field.</returns>
    public string GetFieldName()
    {
        return GetMemberExpression().Member.Name;
    }

    /// <summary>
    /// Retrieves the type of the field from the selector expression.
    /// </summary>
    /// <returns>The type of the field.</returns>
    public Type GetFieldType()
    {
        return GetMemberExpression().Type;
    }

    /// <summary>
    /// Retrieves the member expression from the selector expression.
    /// </summary>
    /// <returns>The member expression.</returns>
    protected MemberExpression GetMemberExpression()
    {
        if (LeafMemberAccess != null)
        {
            return LeafMemberAccess;
        }

        LeafMemberAccess = FluentParameter as MemberExpression;

        if (LeafMemberAccess == null)
        {
            throw new InvalidOperationException("Analyzer was not executed.");
        }

        return LeafMemberAccess;
    }

    /// <inheritdoc/>
    protected override Expression VisitLambda<TDelegate>(Expression<TDelegate> node)
    {
        if (RootParameter != null)
        {
            return base.Visit(node);
        }

        var lambda = node as Expression<Func<TEntity, TField>>;

        if (lambda == null || lambda.Parameters.IsEmpty())
        {
            throw new InvalidOperationException("Invalid update modification expression.");
        }

        RootParameter = lambda.Parameters[0];

        return base.VisitLambda(node);
    }

    /// <inheritdoc/>
    protected override Expression VisitMember(MemberExpression node)
    {
        if (FluentParameter == null)
        {
            FluentParameter = node;
        }

        return base.VisitMember(node);
    }
}