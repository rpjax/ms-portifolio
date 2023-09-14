using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.EntityFramework;

/// <summary>
/// Entity Framework Entity base class.
/// </summary>
public abstract class EFEntity<T> : Entity<T> where T : class, IEFModel
{
    public override IValidator<T>? Validator { get; init; }
    public override IValidator<T>? UpdateValidator { get; init; }
    public override IValidator<IQuery<T>>? QueryValidator { get; init; }

    protected EFEntity()
    {
        Validator = null;
        UpdateValidator = null;
        QueryValidator = null;
    }

    protected override MemberExpression CreateIdSelectorExpression(ParameterExpression parameter)
    {
        return Expression.Property(parameter, nameof(IEFModel.Id));
    }

    protected override object? TryParseId(string id)
    {
        if (long.TryParse(id, out long value))
        {
            return value;
        }

        return null;
    }
}