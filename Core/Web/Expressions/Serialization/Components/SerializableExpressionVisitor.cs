using ModularSystem.Core;
using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Web.Expressions;

public abstract class SerializableExpressionVisitor
{
    [return: NotNullIfNotNull("node")]
    public virtual SerializableExpression? Visit(SerializableExpression? node)
    {
        if (node == null)
        {
            return null;
        }

        return Dispatch(node);
    }

    private SerializableExpression Dispatch(SerializableExpression expression)
    {
        switch (expression.NodeType)
        {
            case Core.Expressions.ExtendedExpressionType.Add:
                // TODO: Implementar a lógica de tratamento para Add
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.AddChecked:
                // TODO: Implementar a lógica de tratamento para AddChecked
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.And:
                // TODO: Implementar a lógica de tratamento para And
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.AndAlso:
                // TODO: Implementar a lógica de tratamento para AndAlso
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.ArrayLength:
                // TODO: Implementar a lógica de tratamento para ArrayLength
                return VisitUnary((SerializableUnaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.ArrayIndex:
                // TODO: Implementar a lógica de tratamento para ArrayIndex
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Call:
                // TODO: Implementar a lógica de tratamento para Call
                return VisitMethodCall((SerializableMethodCallExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Coalesce:
                // TODO: Implementar a lógica de tratamento para Coalesce
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Conditional:
                // TODO: Implementar a lógica de tratamento para Conditional
                return VisitConditional((SerializableConditionalExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Constant:
                // TODO: Implementar a lógica de tratamento para Constant
                return VisitConstant((SerializableConstantExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Convert:
                // TODO: Implementar a lógica de tratamento para Convert
                return VisitUnary((SerializableUnaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.ConvertChecked:
                // TODO: Implementar a lógica de tratamento para ConvertChecked
                return VisitUnary((SerializableUnaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Divide:
                // TODO: Implementar a lógica de tratamento para Divide
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Equal:
                // TODO: Implementar a lógica de tratamento para Equal
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.ExclusiveOr:
                // TODO: Implementar a lógica de tratamento para ExclusiveOr
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.GreaterThan:
                // TODO: Implementar a lógica de tratamento para GreaterThan
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.GreaterThanOrEqual:
                // TODO: Implementar a lógica de tratamento para GreaterThanOrEqual
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Invoke:
                // TODO: Implementar a lógica de tratamento para Invoke
                return VisitInvocation((SerializableInvocationExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Lambda:
                // TODO: Implementar a lógica de tratamento para Lambda
                return VisitLambda((SerializableLambdaExpression)expression);

            case Core.Expressions.ExtendedExpressionType.LeftShift:
                // TODO: Implementar a lógica de tratamento para LeftShift
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.LessThan:
                // TODO: Implementar a lógica de tratamento para LessThan
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.LessThanOrEqual:
                // TODO: Implementar a lógica de tratamento para LessThanOrEqual
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.ListInit:
                // TODO: Implementar a lógica de tratamento para ListInit
                return VisitListInit((SerializableListInitExpression)expression);

            case Core.Expressions.ExtendedExpressionType.MemberAccess:
                // TODO: Implementar a lógica de tratamento para MemberAccess
                return VisitMember((SerializableMemberExpression)expression);

            case Core.Expressions.ExtendedExpressionType.MemberInit:
                // TODO: Implementar a lógica de tratamento para MemberInit
                return VisitMemberInit((SerializableMemberInitExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Modulo:
                // TODO: Implementar a lógica de tratamento para Modulo
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Multiply:
                // TODO: Implementar a lógica de tratamento para Multiply
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.MultiplyChecked:
                // TODO: Implementar a lógica de tratamento para MultiplyChecked
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Negate:
                // TODO: Implementar a lógica de tratamento para Negate
                return VisitUnary((SerializableUnaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.UnaryPlus:
                // TODO: Implementar a lógica de tratamento para UnaryPlus
                return VisitUnary((SerializableUnaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.NegateChecked:
                // TODO: Implementar a lógica de tratamento para NegateChecked
                return VisitUnary((SerializableUnaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.New:
                // TODO: Implementar a lógica de tratamento para New
                return VisitNew((SerializableNewExpression)expression);

            case Core.Expressions.ExtendedExpressionType.NewArrayInit:
                // TODO: Implementar a lógica de tratamento para NewArrayInit
                return VisitNewArray((SerializableNewArrayExpression)expression);

            case Core.Expressions.ExtendedExpressionType.NewArrayBounds:
                // TODO: Implementar a lógica de tratamento para NewArrayBounds
                return VisitNewArray((SerializableNewArrayExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Not:
                // TODO: Implementar a lógica de tratamento para Not
                return VisitUnary((SerializableUnaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.NotEqual:
                // TODO: Implementar a lógica de tratamento para NotEqual
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Or:
                // TODO: Implementar a lógica de tratamento para Or
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.OrElse:
                // TODO: Implementar a lógica de tratamento para OrElse
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Parameter:
                // TODO: Implementar a lógica de tratamento para Parameter
                break;

            case Core.Expressions.ExtendedExpressionType.Power:
                // TODO: Implementar a lógica de tratamento para Power
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Quote:
                // TODO: Implementar a lógica de tratamento para Quote
                return VisitUnary((SerializableUnaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.RightShift:
                // TODO: Implementar a lógica de tratamento para RightShift
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Subtract:
                // TODO: Implementar a lógica de tratamento para Subtract
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.SubtractChecked:
                // TODO: Implementar a lógica de tratamento para SubtractChecked
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.TypeAs:
                // TODO: Implementar a lógica de tratamento para TypeAs
                return VisitUnary((SerializableUnaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.TypeIs:
                // TODO: Implementar a lógica de tratamento para TypeIs
                return VisitTypeBinary((SerializableTypeBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Assign:
                // TODO: Implementar a lógica de tratamento para Assign
                return VisitBinary((SerializableBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Block:
                // Assume que há um método VisitBlock disponível
                return VisitBlock((SerializableBlockExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Decrement:
                // Se Decrement é uma operação unária, você pode usar o método VisitUnary
                return VisitUnary((SerializableUnaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Increment:
                // Se Increment é uma operação unária, você pode usar o método VisitUnary
                return VisitUnary((SerializableUnaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Index:
                // Assume que há um método VisitIndex disponível
                return VisitIndex((SerializableIndexExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Default:
                // Default pode ser tratado como uma constante ou uma expressão separada
                // Se você tem um método VisitDefault, use-o aqui
                // return VisitDefault((SerializableDefaultExpression)expression);
                break;

            case Core.Expressions.ExtendedExpressionType.TypeEqual:
                // TypeEqual é um tipo de operação TypeBinary
                return VisitTypeBinary((SerializableTypeBinaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.IsTrue:
            case Core.Expressions.ExtendedExpressionType.IsFalse:
                // IsTrue e IsFalse podem ser tratados como operações unárias especiais
                return VisitUnary((SerializableUnaryExpression)expression);

            case Core.Expressions.ExtendedExpressionType.UpdateSet:
                // Assume que há um método VisitUpdateSet disponível
                return VisitUpdateSet((SerializableUpdateSetExpression)expression);

            case Core.Expressions.ExtendedExpressionType.Ordering:
                // Assume que há um método VisitOrdering disponível
                return VisitOrdering((SerializableOrderingExpression)expression);

            case Core.Expressions.ExtendedExpressionType.ComplexOrdering:
                // Assume que há um método VisitComplexOrdering disponível
                return VisitComplexOrdering((SerializableComplexOrderingExpression)expression);

            case Core.Expressions.ExtendedExpressionType.DebugInfo:
                // TODO: Implementar a lógica de tratamento para DebugInfo
                break;

            case Core.Expressions.ExtendedExpressionType.Dynamic:
                // TODO: Implementar a lógica de tratamento para Dynamic
                break;

            case Core.Expressions.ExtendedExpressionType.Extension:
                // TODO: Implementar a lógica de tratamento para Extension
                break;

            case Core.Expressions.ExtendedExpressionType.Goto:
                // TODO: Implementar a lógica de tratamento para Goto
                break;

            case Core.Expressions.ExtendedExpressionType.Label:
                // TODO: Implementar a lógica de tratamento para Label
                break;

            case Core.Expressions.ExtendedExpressionType.RuntimeVariables:
                // TODO: Implementar a lógica de tratamento para RuntimeVariables
                break;

            case Core.Expressions.ExtendedExpressionType.Loop:
                // TODO: Implementar a lógica de tratamento para Loop
                break;

            case Core.Expressions.ExtendedExpressionType.Switch:
                // TODO: Implementar a lógica de tratamento para Switch
                break;

            case Core.Expressions.ExtendedExpressionType.Try:
                // TODO: Implementar a lógica de tratamento para Try
                break;

            // Assinaturas de atribuição
            case Core.Expressions.ExtendedExpressionType.AddAssign:
            case Core.Expressions.ExtendedExpressionType.AndAssign:
            case Core.Expressions.ExtendedExpressionType.DivideAssign:
            case Core.Expressions.ExtendedExpressionType.ExclusiveOrAssign:
            case Core.Expressions.ExtendedExpressionType.LeftShiftAssign:
            case Core.Expressions.ExtendedExpressionType.ModuloAssign:
            case Core.Expressions.ExtendedExpressionType.MultiplyAssign:
            case Core.Expressions.ExtendedExpressionType.OrAssign:
            case Core.Expressions.ExtendedExpressionType.PowerAssign:
            case Core.Expressions.ExtendedExpressionType.RightShiftAssign:
            case Core.Expressions.ExtendedExpressionType.SubtractAssign:
            case Core.Expressions.ExtendedExpressionType.AddAssignChecked:
            case Core.Expressions.ExtendedExpressionType.MultiplyAssignChecked:
            case Core.Expressions.ExtendedExpressionType.SubtractAssignChecked:
                // Supondo que você tem um método VisitAssign para atribuições
                return VisitBinary((SerializableBinaryExpression)expression);

            // Operações incrementais e decrementais
            case Core.Expressions.ExtendedExpressionType.Throw:
            case Core.Expressions.ExtendedExpressionType.Unbox:
            case Core.Expressions.ExtendedExpressionType.PreIncrementAssign:
            case Core.Expressions.ExtendedExpressionType.PreDecrementAssign:
            case Core.Expressions.ExtendedExpressionType.PostIncrementAssign:
            case Core.Expressions.ExtendedExpressionType.PostDecrementAssign:
            case Core.Expressions.ExtendedExpressionType.OnesComplement:
                // Supondo que você tem um método VisitIncrementDecrement para incrementos e decrementos
                return VisitUnary((SerializableUnaryExpression)expression);


            default:
                return expression; // Para expressões não manipuladas explicitamente, retorne a expressão original
        }

        return expression;
    }

    [return: NotNullIfNotNull("node")]
    protected T? As<T>(SerializableExpression? node) where T : SerializableExpression
    {
        var cast = node.TryTypeCast<T>();

        if (cast == null)
        {
            throw new Exception();
        }

        return cast;
    }

    [return: NotNullIfNotNull("binding")]
    protected SerializableMemberBinding? VisitMemberBindings(SerializableMemberBinding? binding)
    {
        if (binding == null)
        {
            return null;
        }

        binding.Expression = Visit(binding.Expression);
        binding.Bindings = binding.Bindings
            .Transform(x => VisitMemberBindings(x))
            .ToArray();
        binding.Initializers = binding.Initializers
            .Transform(x => VisitElementInit(x))
            .ToArray();

        return binding;
    }

    [return: NotNullIfNotNull("elementInit")]
    protected SerializableElementInit? VisitElementInit(SerializableElementInit? elementInit)
    {
        if (elementInit == null)
        {
            return null;
        }

        elementInit.Arguments = elementInit.Arguments
            .Transform(x => Visit(x))
            .ToArray();

        return elementInit;
    }

    protected virtual SerializableExpression VisitUnary(SerializableUnaryExpression node)
    {
        node.Operand = Visit(node.Operand);
        return node;
    }

    protected virtual SerializableExpression VisitBinary(SerializableBinaryExpression node)
    {
        node.Left = Visit(node.Left);
        node.Right = Visit(node.Right);
        return node;
    }

    protected virtual SerializableExpression VisitMethodCall(SerializableMethodCallExpression node)
    {
        node.Object = Visit(node.Object);
        node.Arguments = node.Arguments
            .Transform(x => Visit(x))
            .ToArray();
        return node;
    }

    protected virtual SerializableExpression VisitConditional(SerializableConditionalExpression node)
    {
        node.Test = Visit(node.Test);
        node.IfTrue = Visit(node.IfTrue);
        node.IfFalse = Visit(node.IfFalse);
        return node;
    }

    protected virtual SerializableExpression VisitConstant(SerializableConstantExpression node)
    {
        return node;
    }

    protected virtual SerializableExpression VisitInvocation(SerializableInvocationExpression node)
    {
        node.Expression = Visit(node.Expression);
        node.Arguments = node.Arguments
            .Transform(x => Visit(x))
            .ToArray();
        return node;
    }

    protected virtual SerializableExpression VisitLambda(SerializableLambdaExpression node)
    {
        node.Parameters = node.Parameters
            .Transform(x => As<SerializableParameterExpression>(Visit(x)))
            .ToArray();
        node.Body = Visit(node.Body);
        return node;
    }

    protected virtual SerializableExpression VisitListInit(SerializableListInitExpression node)
    {
        node.NewExpression = As<SerializableNewExpression>(Visit(node.NewExpression));

        foreach (var item in node.Initializers)
        {
            item.Arguments = item.Arguments
                .Transform(x => Visit(x))
                .ToArray();
        }

        return node;
    }

    protected virtual SerializableExpression VisitMember(SerializableMemberExpression node)
    {
        node.Expression = Visit(node.Expression);
        return node;
    }

    protected virtual SerializableExpression VisitMemberInit(SerializableMemberInitExpression node)
    {
        node.NewExpression = As<SerializableNewExpression>(Visit(node.NewExpression));
        node.Bindings = node.Bindings
            .Transform(x => VisitMemberBindings(x))
            .ToArray();
        return node;
    }

    protected virtual SerializableExpression VisitNew(SerializableNewExpression node)
    {
        node.Arguments = node.Arguments
            .Transform(x => Visit(x))
            .ToArray();
        return node;
    }

    protected virtual SerializableExpression VisitNewArray(SerializableNewArrayExpression node)
    {
        node.Initializers = node.Initializers
            .Transform(x => Visit(x))
            .ToArray();
        return node;
    }

    protected virtual SerializableExpression VisitTypeBinary(SerializableTypeBinaryExpression node)
    {
        node.Expression = Visit(node.Expression);
        return node;
    }

    protected virtual SerializableExpression VisitBlock(SerializableBlockExpression node)
    {
        node.Expressions = node.Expressions
            .Transform(x => Visit(x))
            .ToArray();
        return node;
    }

    protected virtual SerializableExpression VisitIndex(SerializableIndexExpression node)
    {
        node.Object = Visit(node.Object);
        node.Arguments = node.Arguments
            .Transform(x => Visit(x))
            .ToArray();
        return node;
    }

    protected virtual SerializableExpression VisitUpdateSet(SerializableUpdateSetExpression node)
    {
        node.FieldSelector = Visit(node.FieldSelector);
        node.Value = Visit(node.Value);
        return node;
    }

    protected virtual SerializableExpression VisitOrdering(SerializableOrderingExpression node)
    {
        node.FieldSelector = Visit(node.FieldSelector);
        return node;
    }

    protected virtual SerializableExpression VisitComplexOrdering(SerializableComplexOrderingExpression node)
    {
        node.Expressions = node.Expressions
            .Transform(x => Visit(x))
            .ToArray();
        return node;
    }

}
