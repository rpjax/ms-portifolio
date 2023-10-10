using ModularSystem.Core;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Represents an extended set of expression types, augmenting the standard set to encompass custom expression types derived from <see cref="CustomExpression"/>.<br/>
/// This enumeration is designed to be fully compatible with the original expression type enumeration, allowing for straightforward casting between the two.
/// </summary>
public enum ExtendedExpressionType : int
{
    /// <summary>
    /// An addition operation, such as a + b, without overflow checking, for numeric operands.
    /// </summary>
    Add = 0,

    /// <summary>
    /// An addition operation, such as (a + b), with overflow checking, for numeric operands.
    /// </summary>
    AddChecked = 1,

    /// <summary>
    /// A bitwise or logical AND operation, such as (a &amp; b) in C# and (a And b) in Visual Basic.
    /// </summary>
    And = 2,

    /// <summary>
    /// A conditional AND operation that evaluates the second operand only if the first operand evaluates to true. It corresponds to (a &amp;&amp; b) in C# and (a AndAlso b) in Visual Basic.
    /// </summary>
    AndAlso = 3,

    /// <summary>
    /// An operation that obtains the length of a one-dimensional array, such as array.Length.
    /// </summary>
    ArrayLength = 4,

    /// <summary>
    /// An indexing operation in a one-dimensional array, such as array[index] in C# or array(index) in Visual Basic.
    /// </summary>
    ArrayIndex = 5,

    /// <summary>
    /// A method call, such as in the obj.sampleMethod() expression.
    /// </summary>
    Call = 6,

    /// <summary>
    /// A node that represents a null coalescing operation, such as (a ?? b) in C# or If(a, b) in Visual Basic.
    /// </summary>
    Coalesce = 7,

    /// <summary>
    /// A conditional operation, such as a &gt; b ? a : b in C# or If(a &gt; b, a, b) in Visual Basic.
    /// </summary>
    Conditional = 8,

    /// <summary>
    /// A constant value.
    /// </summary>
    Constant = 9,

    /// <summary>
    /// A cast or conversion operation, such as (SampleType)obj in C#or CType(obj, SampleType) in Visual Basic. For a numeric conversion, if the converted value is too large for the destination type, no exception is thrown.
    /// </summary>
    Convert = 10,

    /// <summary>
    /// A cast or conversion operation, such as (SampleType)obj in C#or CType(obj, SampleType) in Visual Basic. For a numeric conversion, if the converted value does not fit the destination type, an exception is thrown.
    /// </summary>
    ConvertChecked = 11,

    /// <summary>
    /// A division operation, such as (a / b), for numeric operands.
    /// </summary>
    Divide = 12,

    /// <summary>
    /// A node that represents an equality comparison, such as (a == b) in C# or (a = b) in Visual Basic.
    /// </summary>
    Equal = 13,

    /// <summary>
    /// A bitwise or logical XOR operation, such as (a ^ b) in C# or (a Xor b) in Visual Basic.
    /// </summary>
    ExclusiveOr = 14,

    /// <summary>
    /// A "greater than" comparison, such as (a &gt; b).
    /// </summary>
    GreaterThan = 15,

    /// <summary>
    /// A "greater than or equal to" comparison, such as (a &gt;= b).
    /// </summary>
    GreaterThanOrEqual = 16,

    /// <summary>
    /// An operation that invokes a delegate or lambda expression, such as sampleDelegate.Invoke().
    /// </summary>
    Invoke = 17,

    /// <summary>
    /// A lambda expression, such as a => a + a in C# or Function(a) a + a in Visual Basic.
    /// </summary>
    Lambda = 18,

    /// <summary>
    /// A bitwise left-shift operation, such as (a &lt;&lt; b).
    /// </summary>
    LeftShift = 19,

    /// <summary>
    /// A "less than" comparison, such as (a &lt; b).
    /// </summary>
    LessThan = 20,

    /// <summary>
    /// A "less than or equal to" comparison, such as (a &lt;= b).
    /// </summary>
    LessThanOrEqual = 21,

    /// <summary>
    /// An operation that creates a new System.Collections.IEnumerable object and initializes it from a list of elements, such as new List&lt;SampleType&gt;(){ a, b, c } in C# or Dim sampleList = { a, b, c } in Visual Basic.
    /// </summary>
    ListInit = 22,

    /// <summary>
    /// An operation that reads from a field or property, such as obj.SampleProperty.
    /// </summary>
    MemberAccess = 23,

    /// <summary>
    /// An operation that creates a new object and initializes one or more of its members, such as new Point { X = 1, Y = 2 } in C# or New Point With {.X = 1, .Y = 2} in Visual Basic.
    /// </summary>
    MemberInit = 24,

    /// <summary>
    /// An arithmetic remainder operation, such as (a % b) in C# or (a Mod b) in Visual Basic.
    /// </summary>
    Modulo = 25,

    /// <summary>
    /// A multiplication operation, such as (a * b), without overflow checking, for numeric operands.
    /// </summary>
    Multiply = 26,

    /// <summary>
    /// An multiplication operation, such as (a * b), that has overflow checking, for numeric operands.
    /// </summary>
    MultiplyChecked = 27,

    /// <summary>
    /// An arithmetic negation operation, such as (-a). The object a should not be modified in place.
    /// </summary>
    Negate = 28,

    /// <summary>
    /// A unary plus operation, such as (+a). The result of a predefined unary plus operation is the value of the operand, but user-defined implementations might have unusual results.
    /// </summary>
    UnaryPlus = 29,

    /// <summary>
    /// An arithmetic negation operation, such as (-a), that has overflow checking. The object a should not be modified in place.
    /// </summary>
    NegateChecked = 30,

    /// <summary>
    /// An operation that calls a constructor to create a new object, such as new SampleType().
    /// </summary>
    New = 31,

    /// <summary>
    /// An operation that creates a new one-dimensional array and initializes it from a list of elements, such as new SampleType[]{a, b, c} in C# or New SampleType(){a, b, c} in Visual Basic.
    /// </summary>
    NewArrayInit = 32,

    /// <summary>
    /// An operation that creates a new array, in which the bounds for each dimension are specified, such as new SampleType[dim1, dim2] in C# or New SampleType(dim1, dim2) in Visual Basic.
    /// </summary>
    NewArrayBounds = 33,

    /// <summary>
    /// A bitwise complement or logical negation operation. In C#, it is equivalent to (~a) for integral types and to (!a) for Boolean values. In Visual Basic, it is equivalent to (Not a). The object a should not be modified in place.
    /// </summary>
    Not = 34,

    /// <summary>
    /// An inequality comparison, such as (a != b) in C# or (a &lt;&gt; b) in Visual Basic.
    /// </summary>
    NotEqual = 35,

    /// <summary>
    /// A bitwise or logical OR operation, such as (a | b) in C# or (a Or b) in Visual Basic.
    /// </summary>
    Or = 36,

    /// <summary>
    /// A short-circuiting conditional OR operation, such as (a || b) in C# or (a OrElse b) in Visual Basic.
    /// </summary>
    OrElse = 37,

    /// <summary>
    /// A reference to a parameter or variable that is defined in the context of the expression. For more information, see System.Linq.Expressions.ParameterExpression.
    /// </summary>
    Parameter = 38,

    /// <summary>
    /// A mathematical operation that raises a number to a power, such as (a ^ b) in Visual Basic.
    /// </summary>
    Power = 39,

    /// <summary>
    /// An expression that has a constant value of type System.Linq.Expressions.Expression. A System.Linq.Expressions.ExpressionType.Quote node can contain references to parameters that are defined in the context of the expression it represents.
    /// </summary>
    Quote = 40,

    /// <summary>
    /// A bitwise right-shift operation, such as (a &gt;&gt; b).
    /// </summary>
    RightShift = 41,

    /// <summary>
    /// A subtraction operation, such as (a - b), without overflow checking, for numeric operands.
    /// </summary>
    Subtract = 42,

    /// <summary>
    /// An arithmetic subtraction operation, such as (a - b), that has overflow checking, for numeric operands.
    /// </summary>
    SubtractChecked = 43,

    /// <summary>
    /// An explicit reference or boxing conversion in which null is supplied if the conversion fails, such as (obj as SampleType) in C# or TryCast(obj, SampleType) in Visual Basic.
    /// </summary>
    TypeAs = 44,

    /// <summary>
    /// A type test, such as obj is SampleType in C# or TypeOf obj is SampleType in Visual Basic.
    /// </summary>
    TypeIs = 45,

    /// <summary>
    /// An assignment operation, such as (a = b).
    /// </summary>
    Assign = 46,

    /// <summary>
    /// A block of expressions.
    /// </summary>
    Block = 47,

    /// <summary>
    /// Debugging information.
    /// </summary>
    DebugInfo = 48,

    /// <summary>
    /// A unary decrement operation, such as (a - 1) in C# and Visual Basic. The object a should not be modified in place.
    /// </summary>
    Decrement = 49,

    /// <summary>
    /// A dynamic operation.
    /// </summary>
    Dynamic = 50,

    /// <summary>
    /// A default value.
    /// </summary>
    Default = 51,

    /// <summary>
    /// An extension expression.
    /// </summary>
    Extension = 52,

    /// <summary>
    /// A "go to" expression, such as goto Label in C# or GoTo Label in Visual Basic.
    /// </summary>
    Goto = 53,

    /// <summary>
    /// A unary increment operation, such as (a + 1) in C# and Visual Basic. The object a should be modified in place.
    /// </summary>
    Increment = 54,

    /// <summary>
    /// An index operation or an operation that accesses a property that takes arguments.
    /// </summary>
    Index = 55,

    /// <summary>
    /// A label.
    /// </summary>
    Label = 56,

    /// <summary>
    /// A list of run-time variables. For more information, see System.Linq.Expressions.RuntimeVariablesExpression.
    /// </summary>
    RuntimeVariables = 57,

    /// <summary>
    /// A loop, such as for or while.
    /// </summary>
    Loop = 58,

    /// <summary>
    /// A switch operation, such as switch in C# or Select Case in Visual Basic.
    /// </summary>
    Switch = 59,

    /// <summary>
    /// An operation that throws an exception, such as throw new Exception().
    /// </summary>
    Throw = 60,

    /// <summary>
    /// A try-catch expression.
    /// </summary>
    Try = 61,

    /// <summary>
    /// An unbox value type operation, such as unbox and unbox.any instructions in MSIL.
    /// </summary>
    Unbox = 62,

    /// <summary>
    /// An addition compound assignment operation, such as (a += b), without overflow checking, for numeric operands.
    /// </summary>
    AddAssign = 63,

    /// <summary>
    /// A bitwise or logical AND compound assignment operation, such as (a &amp;= b) in C#.
    /// </summary>
    AndAssign = 64,

    /// <summary>
    /// An division compound assignment operation, such as (a /= b), for numeric operands.
    /// </summary>
    DivideAssign = 65,

    /// <summary>
    /// A bitwise or logical XOR compound assignment operation, such as (a ^= b) in C#.
    /// </summary>
    ExclusiveOrAssign = 66,

    /// <summary>
    /// A bitwise left-shift compound assignment, such as (a &lt;&lt;= b).
    /// </summary>
    LeftShiftAssign = 67,

    /// <summary>
    /// An arithmetic remainder compound assignment operation, such as (a %= b) in C#.
    /// </summary>
    ModuloAssign = 68,

    /// <summary>
    /// A multiplication compound assignment operation, such as (a *= b), without overflow checking, for numeric operands.
    /// </summary>
    MultiplyAssign = 69,

    /// <summary>
    /// A bitwise or logical OR compound assignment, such as (a |= b) in C#.
    /// </summary>
    OrAssign = 70,

    /// <summary>
    /// A compound assignment operation that raises a number to a power, such as (a ^= b) in Visual Basic.
    /// </summary>
    PowerAssign = 71,

    /// <summary>
    /// A bitwise right-shift compound assignment operation, such as (a &gt;&gt;= b).
    /// </summary>
    RightShiftAssign = 72,

    /// <summary>
    /// A subtraction compound assignment operation, such as (a -= b), without overflow checking, for numeric operands.
    /// </summary>
    SubtractAssign = 73,

    /// <summary>
    /// An addition compound assignment operation, such as (a += b), with overflow checking, for numeric operands.
    /// </summary>
    AddAssignChecked = 74,

    /// <summary>
    /// A multiplication compound assignment operation, such as (a *= b), that has overflow checking, for numeric operands.
    /// </summary>
    MultiplyAssignChecked = 75,

    /// <summary>
    /// A subtraction compound assignment operation, such as (a -= b), that has overflow checking, for numeric operands.
    /// </summary>
    SubtractAssignChecked = 76,

    /// <summary>
    /// A unary prefix increment, such as (++a). The object a should be modified in place.
    /// </summary>
    PreIncrementAssign = 77,

    /// <summary>
    /// A unary prefix decrement, such as (--a). The object a should be modified in place.
    /// </summary>
    PreDecrementAssign = 78,

    /// <summary>
    /// A unary postfix increment, such as (a++). The object a should be modified in place.
    /// </summary>
    PostIncrementAssign = 79,

    /// <summary>
    /// A unary postfix decrement, such as (a--). The object a should be modified in place.
    /// </summary>
    PostDecrementAssign = 80,

    /// <summary>
    /// An exact type test.
    /// </summary>
    TypeEqual = 81,

    /// <summary>
    /// A ones complement operation, such as (~a) in C#.
    /// </summary>
    OnesComplement = 82,

    /// <summary>
    /// A true condition value.
    /// </summary>
    IsTrue = 83,

    /// <summary>
    /// A false condition value.
    /// </summary>
    IsFalse = 84
}
