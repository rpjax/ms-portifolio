using System.Linq.Expressions;

namespace ModularSystem.Core;

public class Update<T> : IUpdate<T> 
{
    public Expression? Filter { get; set; } = null;
    public List<Expression> Modifications { get; set; } = new();

    public Update(IUpdate<T>? update = null)
    {
        if (update == null) return;
        Filter = update.Filter;
        Modifications = update.Modifications;
    }

    public List<T>? ModificationsAs<T>() where T : Expression
    {
        return Modifications.ConvertAll(x => x.TypeCast<T>());  
    }

    public List<UpdateSetExpression<T>>? ModificationsAsUpdateSets()
    {
        throw new NotImplementedException();
        //return Modifications?.ConvertAll(x =>
        //{
        //    var cast = x
        //});
    }

}

public class UpdateSetExpression<T> : Expression
{
    public string FieldName { get; }
    public Type Type { get; }
    public dynamic? Value { get; }
    public override ExpressionType NodeType { get; }

    public UpdateSetExpression(string fieldName, Type type, dynamic? value)
    {
        NodeType = ExpressionType.Assign;
        FieldName = fieldName;
        Type = type;
        Value = value;
    }

    public override string ToString()
    {
        return $"{FieldName} = {Value?.ToString()}";
    }
}

public class UpdateWriter<T> : IFactory<IUpdate<T>>
{
    private Update<T> Update { get; }

    public UpdateWriter(IUpdate<T>? update = null)
    {
        Update = new();
        if (update == null) return;
        Update.Filter = update.Filter;
        Update.Modifications = update.Modifications;
    }

    public IUpdate<T> Create()
    {
        return Update;
    }

    public UpdateWriter<T> SetFilter(Expression<Func<T, bool>>? expression)
    {
        Update.Filter = expression;
        return this;
    }

    public UpdateWriter<T> AddModification<TField>(Expression<Func<T, TField>> selector, TField value)
    {
        var analyser = new ModificationAnalyser<T, TField>(selector)
            .Execute();
        var modification = 
            new UpdateSetExpression<T>(analyser.GetFieldName(), analyser.GetFieldType(), value);

        Update.Modifications.Add(modification);
        return this;
    }

    internal class ModificationAnalyser<TEntity, TField> : ExpressionVisitor
    {
        Expression<Func<TEntity, TField>> SelectorExpression { get; }
        Expression? RootParameter { get; set; }
        Expression? FluentParameter { get; set; }

        MemberExpression? LeafMemberAccess { get; set; }

        public ModificationAnalyser(Expression<Func<TEntity, TField>> expression)
        {
            SelectorExpression = expression;
            RootParameter = null;
            FluentParameter = null;
            LeafMemberAccess = null;
        }
        
        public ModificationAnalyser<TEntity, TField> Execute()
        {
            Visit(SelectorExpression);
            return this;
        }

        public string GetFieldName()
        {
            return GetMemberExpression().Member.Name;
        }

        public Type GetFieldType()
        {
            return GetMemberExpression().Type;
        }

        private MemberExpression GetMemberExpression()
        {
            if(LeafMemberAccess != null)
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

        protected override Expression VisitLambda<TDelegate>(Expression<TDelegate> node)
        {
            if(RootParameter != null)
            {
                return base.Visit(node);
            }

            var lambda = node as Expression<Func<TEntity, TField>>;

            if (lambda == null || lambda.Parameters.IsEmpty())
            {
                throw new InvalidOperationException("Invalid update modification expression.");
            }

            RootParameter = lambda.Parameters[0];
            FluentParameter = lambda.Parameters[0];

            return base.VisitLambda(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if(node.Expression != FluentParameter)
            {
                throw new InvalidOperationException("Invalid update modification expression.");
            }

            FluentParameter = node;

            return base.VisitMember(node);
        }
    }
}

public class UpdateReader<T>
{
    private Update<T> Update { get; }

    public UpdateReader(Update<T> update)
    {
        Update = update;
    }

    public UpdateReader(IUpdate<T> update)
    {
        Update = new(update);
    }

    public Expression<Func<T, bool>>? GetFilterExpression()
    {
        return VisitExpression(Update.Filter as Expression<Func<T, bool>>);
    }

    public IEnumerable<UpdateSetExpression<T>> GetUpdateSetExpressions()
    {
        foreach (var expression in Update.Modifications)
        {
            var cast = expression as UpdateSetExpression<T>;

            if(cast == null)
            {
                throw new InvalidOperationException();
            }

            yield return cast;
        }
    }

    protected ExpressionVisitor CreateExpressionVisitor()
    {
        return new ParameterExpressionUniformityVisitor();
    }

    protected TResult? VisitExpression<TResult>(TResult? expression) where TResult : Expression
    {
        if (expression == null)
        {
            return null;
        }

        return CreateExpressionVisitor().Visit(expression).TypeCast<TResult>();
    }
}