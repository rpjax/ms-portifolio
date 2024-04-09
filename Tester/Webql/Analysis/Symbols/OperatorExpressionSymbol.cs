using ModularSystem.Webql.Analysis.Components;
using System.Linq;

namespace ModularSystem.Webql.Analysis.Symbols;

public interface IOperatorExpressionSymbol : IExpressionSymbol
{
    OperatorType Operator { get; }
    ExpressionSymbol[] Operands { get; }
}

public interface IResultProducerOperatorExpressionSymbol : IOperatorExpressionSymbol
{
    ExpressionSymbol Destination { get; }
}

public abstract class OperatorExpressionSymbol : ExpressionSymbol, IOperatorExpressionSymbol
{
    public abstract OperatorType Operator { get; }
    public override ExpressionType ExpressionType { get; } = ExpressionType.Operator;
    public ExpressionSymbol[] Operands { get; init; }

    public int OperandsCount => Operands.Length;

    public ExpressionSymbol this[int index]
    {
        get => Operands[index];
        set => Operands[index] = value;
    }

    public OperatorExpressionSymbol(params ExpressionSymbol[] operands)
    {
        var nullOperands = operands.Where(x => x == null).ToArray();

        foreach (var item in nullOperands)
        {
            throw new ArgumentNullException(nameof(operands), "Operands cannot be null.");
        }

        Operands = operands;
    }

    public override string ToString()
    {
        var operands = string.Join(", ", Operands.Select(x => x.ToString()));

        return $"{Stringify(Operator)}({operands})";
    }

    //public override Symbol Accept(AstVisitor visitor)
    //{
    //    return visitor.VisitOperatorExpression(this);
    //}

    protected override IEnumerable<Symbol> GetChildren()
    {
        foreach (var item in Operands)
        {
            yield return item;
        }
    }

}

//*
//*
//* base classes, semantics oriented. 
//*
public abstract class UnaryOperatorExpressionSymbol : OperatorExpressionSymbol, IResultProducerOperatorExpressionSymbol
{
    public ExpressionSymbol Destination => Operands[0];
    public ExpressionSymbol Operand => Operands[1];

    protected UnaryOperatorExpressionSymbol(
        ExpressionSymbol destination, 
        ExpressionSymbol operand
    )
    : base(destination, operand)
    {
  
    }
}

public abstract class BinaryOperatorExpressionSymbol : OperatorExpressionSymbol, IResultProducerOperatorExpressionSymbol
{
    public ExpressionSymbol Destination => Operands[0];
    public ExpressionSymbol LeftOperand => Operands[1];
    public ExpressionSymbol RightOperand => Operands[2];

    protected BinaryOperatorExpressionSymbol(
        ExpressionSymbol destination, 
        ExpressionSymbol left, 
        ExpressionSymbol right
    )
    : base(destination, left, right )
    {

    }
}

public abstract class ArrayOperatorExpressionSymbol : OperatorExpressionSymbol, IResultProducerOperatorExpressionSymbol
{
    public ExpressionSymbol Destination => Operands[0];
    public IEnumerable<ExpressionSymbol> Expressions => GetValues();

    protected ArrayOperatorExpressionSymbol(
        ExpressionSymbol destination, 
        ExpressionSymbol[] expressions
    )
    : base(new ExpressionSymbol[] { destination }.Concat(expressions).ToArray())
    {

    }

    private IEnumerable<ExpressionSymbol> GetValues()
    {
        for (int i = 1; i < OperandsCount; i++)
        {
            yield return Operands[i];
        }
    }
}

public abstract class PredicateOperatorExpressionSymbol : OperatorExpressionSymbol, IResultProducerOperatorExpressionSymbol
{
    public ExpressionSymbol Destination => Operands[0];
    public ExpressionSymbol Source => Operands[1];
    public LambdaExpressionSymbol Lambda => (LambdaExpressionSymbol)Operands[2];

    protected PredicateOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol source,
        LambdaExpressionSymbol lambda
    )
    : base(destination, source, lambda)
    {

    }

    public override string ToString()
    {
        return $"{Stringify(Operator)}({Source}, {Lambda})";
    }
}

/*
 * 
 * 
 * arithmetic expression symbols:
 * 
 * add, 
 * subtract, 
 * divide,
 * multiply, 
 * modulo
 */

public interface IArithmeticOperatorExpressionSymbol : IOperatorExpressionSymbol
{

}

public abstract class ArithmeticOperatorExpressionSymbol : BinaryOperatorExpressionSymbol, IArithmeticOperatorExpressionSymbol
{
    protected ArithmeticOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class AddOperatorExpressionSymbol : ArithmeticOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Add;

    public AddOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteAddOperatorExpression(this);
    }
}

public class SubtractOperatorExpressionSymbol : ArithmeticOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Subtract;

    public SubtractOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteSubtractOperatorExpression(this);
    }
}

public class DivideOperatorExpressionSymbol : ArithmeticOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Divide;

    public DivideOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteDivideOperatorExpression(this);
    }
}

public class MultiplyOperatorExpressionSymbol : ArithmeticOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Multiply;

    public MultiplyOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteMultiplyOperatorExpression(this);
    }
}

public class ModuloOperatorExpressionSymbol : ArithmeticOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Modulo;

    public ModuloOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteModuloOperatorExpression(this);
    }
}

/*
 * 
 * 
 * relational expression symbols:
 * 
 * equals, 
 * not-equals,
 * less,
 * less-equals,
 * greater,
 * greater-equals
 */
public interface IRelationalOperatorExpressionSymbol : IOperatorExpressionSymbol
{

}

public abstract class RelationalOperatorExpressionSymbol : BinaryOperatorExpressionSymbol, IRelationalOperatorExpressionSymbol
{
    protected RelationalOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class EqualsOperatorExpressionSymbol : RelationalOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Equals;

    public EqualsOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteEqualsOperatorExpression(this);
    }
}

public class NotEqualsOperatorExpressionSymbol : RelationalOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.NotEquals;

    public NotEqualsOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteNotEqualsOperatorExpression(this);
    }
}

public class LessOperatorExpressionSymbol : RelationalOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Less;

    public LessOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteLessOperatorExpression(this);
    }
}

public class LessEqualsOperatorExpressionSymbol : RelationalOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.LessEquals;

    public LessEqualsOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteLessEqualsOperatorExpression(this);  
    }
}

public class GreaterOperatorExpressionSymbol : RelationalOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Greater;

    public GreaterOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteGreaterOperatorExpression(this);
    }
}

public class GreaterEqualsOperatorExpressionSymbol : RelationalOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.GreaterEquals;

    public GreaterEqualsOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
     
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteGreaterEqualsOperatorExpression(this);
    }
}

/*
 * 
 * 
 * string relational expression symbols:
 * 
 * like,
 * regex-match
 */
public interface IStringRelationalOperatorExpressionSymbol : IOperatorExpressionSymbol
{

}

public abstract class StringRelationalOperatorExpressionSymbol 
    : BinaryOperatorExpressionSymbol, IStringRelationalOperatorExpressionSymbol
{
    protected StringRelationalOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }
}

public class LikeOperatorExpressionSymbol : StringRelationalOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Like;

    public LikeOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteLikeOperatorExpression(this);
    }
}

public class RegexMatchOperatorExpressionSymbol : StringRelationalOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.RegexMatch;

    public RegexMatchOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol left,
        ExpressionSymbol right
    )
    : base(destination, left, right)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteRegexMatchOperatorExpression(this);
    }
}

/*
 * 
 * 
 * logical expression symbols:
 * 
 * and,
 * or,
 * not
 */
public interface ILogicalOperatorExpressionSymbol : IOperatorExpressionSymbol
{

}

public class AndOperatorExpressionSymbol : ArrayOperatorExpressionSymbol, ILogicalOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.And;

    public AndOperatorExpressionSymbol(
        ExpressionSymbol destination, 
        ExpressionSymbol[] expressions
    ) 
    : base(destination, expressions)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteAndOperatorExpression(this);
    }
}

public class OrOperatorExpressionSymbol : ArrayOperatorExpressionSymbol, ILogicalOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Or;

    public OrOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol[] expressions
    )
    : base(destination, expressions)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteOrOperatorExpression(this);
    }
}

public class NotOperatorExpressionSymbol : UnaryOperatorExpressionSymbol, ILogicalOperatorExpressionSymbol
{
    public override OperatorType Operator { get; } = OperatorType.Not;

    public NotOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol expression
    )
    : base(destination, expression)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteNotOperatorExpression(this);
    }
}

/*
 * 
 * 
 * logical expression symbols:
 * 
 * and,
 * or,
 * not
 */
public interface ISemanticOperatorExpression : IOperatorExpressionSymbol
{

}

//public class ExprOperatorExpressionSymbol : OperatorExpressionSymbol, ISemanticOperatorExpression
//{
//    public override OperatorType Operator { get; } = OperatorType.Expr;

//    public ExprOperatorExpressionSymbol(
//        ExpressionSymbol destination,
//        ExpressionSymbol left,
//        ExpressionSymbol right
//    )
//    : base(destination, left, right)
//    {
//    }

//    public override Symbol Accept(AstRewriter rewriter)
//    {
//        return rewriter.RewriteExprOperatorExpression(this);
//    }
//}

//public class ParseOperatorExpressionSymbol : BinaryOperatorExpressionSymbol, ISemanticOperatorExpression
//{
//    public override OperatorType Operator { get; } = OperatorType.Parse;

//    public ParseOperatorExpressionSymbol(
//        ExpressionSymbol destination, 
//        ExpressionSymbol left, 
//        ExpressionSymbol right
//    ) 
//    : base(destination, left, right)
//    {
//    }

//    public override Symbol Accept(AstRewriter rewriter)
//    {
//        return rewriter.RewriteParseOperatorExpression(this);
//    }
//}

public class TypeOperatorExpressionSymbol : OperatorExpressionSymbol, ISemanticOperatorExpression
{
    public override OperatorType Operator { get; } = OperatorType.Type;
    public ExpressionSymbol Destination => Operands[0];
    public AnonymousTypeExpressionSymbol TypeExpression => (AnonymousTypeExpressionSymbol)Operands[1];

    public TypeOperatorExpressionSymbol(
        ExpressionSymbol destination,
        AnonymousTypeExpressionSymbol typeExpression
    )
    : base(destination, typeExpression)
    {
    }

    public override string ToString()
    {
        return $"{Stringify(Operator)}({TypeExpression})";
    }

    override public Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteTypeOperatorExpression(this);
    }
}

public class MemberAccessOperatorExpressionSymbol : OperatorExpressionSymbol, ISemanticOperatorExpression
{
    public override OperatorType Operator { get; } = OperatorType.MemberAccess;
    public string MemberName { get; }
    public ExpressionSymbol Operand => Operands[0];

    public MemberAccessOperatorExpressionSymbol(
        string memberName,
        ExpressionSymbol operand
    )
    : base(operand)
    {
        MemberName = memberName;
    }

    public override string ToString()
    {
        return $"{Operand}.{MemberName}";
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteMemberAccessOperatorExpression(this);
    }
}

//*
//*
//* collection manipulation expression symbols.
//*
public interface ICollectionManipulationOperatorExpression : IOperatorExpressionSymbol
{

}

public class FilterOperatorExpressionSymbol : PredicateOperatorExpressionSymbol, ICollectionManipulationOperatorExpression
{
    public override OperatorType Operator { get; } = OperatorType.Filter;

    public FilterOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol source,
        LambdaExpressionSymbol lambda
    )
    : base(destination, source, lambda)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteFilterOperatorExpression(this);
    }
}

// [ string_literal, expression, ]
// "$anonymousType": [<destination>, <source>, <lambda>]
public class SelectOperatorExpressionSymbol : PredicateOperatorExpressionSymbol, ICollectionManipulationOperatorExpression
{
    public override OperatorType Operator { get; } = OperatorType.Select;

    public SelectOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol source,
        LambdaExpressionSymbol lambda
    )
    : base(destination, source, lambda)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteSelectOperatorExpression(this);
    }
}

public class SelectManyOperatorExpressionSymbol : PredicateOperatorExpressionSymbol, ICollectionManipulationOperatorExpression
{
    public override OperatorType Operator { get; } = OperatorType.SelectMany;

    public SelectManyOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol source,
        LambdaExpressionSymbol lambda
    )
    : base(destination, source, lambda)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteSelectManyOperatorExpression(this);
    }
}

public class LimitOperatorExpressionSymbol : OperatorExpressionSymbol, ICollectionManipulationOperatorExpression
{
    public override OperatorType Operator { get; } = OperatorType.Limit;
    public ExpressionSymbol Destination => Operands[0];
    public ExpressionSymbol Source => Operands[1];
    public ExpressionSymbol Value => Operands[2];

    public LimitOperatorExpressionSymbol(
        ExpressionSymbol destination, 
        ExpressionSymbol source, 
        ExpressionSymbol value
    )         
    : base(destination, source, value)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteLimitOperatorExpression(this);
    }
}

public class SkipOperatorExpressionSymbol : OperatorExpressionSymbol, ICollectionManipulationOperatorExpression
{
    public override OperatorType Operator { get; } = OperatorType.Skip;
    public ExpressionSymbol Destination => Operands[0];
    public ExpressionSymbol Source => Operands[1];
    public ExpressionSymbol Value => Operands[2];

    public SkipOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol source,
        ExpressionSymbol value
    )
    : base(destination, source, value)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteSkipOperatorExpression(this);
    }
}

//*
//*
//* collection aggregation expression symbols.
//*
public interface ICollectionAggregationOperatorExpression : IOperatorExpressionSymbol
{

}

public class CountOperatorExpressionSymbol : OperatorExpressionSymbol, ICollectionAggregationOperatorExpression
{
    public override OperatorType Operator { get; } = OperatorType.Count;
    public ExpressionSymbol Destination => Operands[0];
    public ExpressionSymbol Source => Operands[1];

    public CountOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol source
    )
    : base(destination, source)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteCountOperatorExpression(this);
    }
}

public class IndexOperatorExpressionSymbol : OperatorExpressionSymbol, ICollectionAggregationOperatorExpression
{
    public override OperatorType Operator { get; } = OperatorType.Index;
    public ExpressionSymbol Destination => Operands[0];
    public ExpressionSymbol Source => Operands[1];
    public ExpressionSymbol Index => Operands[2];

    public IndexOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol source,
        ExpressionSymbol index
    )
    : base(destination, source, index)
    {
        
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteIndexOperatorExpression(this);
    }
}

public class AnyOperatorExpressionSymbol : PredicateOperatorExpressionSymbol, ICollectionAggregationOperatorExpression
{
    public override OperatorType Operator { get; } = OperatorType.Any;

    public AnyOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol source,
        LambdaExpressionSymbol lambda
    )
    : base(destination, source, lambda)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteAnyOperatorExpression(this);
    }
}

public class AllOperatorExpressionSymbol : PredicateOperatorExpressionSymbol, ICollectionAggregationOperatorExpression
{
    public override OperatorType Operator { get; } = OperatorType.All;

    public AllOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol source,
        LambdaExpressionSymbol lambda
    )
    : base(destination, source, lambda)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteAllOperatorExpression(this);
    }
}

public class MinOperatorExpressionSymbol : PredicateOperatorExpressionSymbol, ICollectionAggregationOperatorExpression
{
    public override OperatorType Operator { get; } = OperatorType.Min;

    public MinOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol source,
        LambdaExpressionSymbol lambda
    )
    : base(destination, source, lambda)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteMinOperatorExpression(this);
    }
}

public class MaxOperatorExpressionSymbol : PredicateOperatorExpressionSymbol, ICollectionAggregationOperatorExpression
{
    public override OperatorType Operator { get; } = OperatorType.Max;

    public MaxOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol source,
        LambdaExpressionSymbol lambda
    )
    : base(destination, source, lambda)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteMaxOperatorExpression(this);
    }
}

public class SumOperatorExpressionSymbol : PredicateOperatorExpressionSymbol, ICollectionAggregationOperatorExpression
{
    public override OperatorType Operator { get; } = OperatorType.Sum;

    public SumOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol source,
        LambdaExpressionSymbol lambda
    )
    : base(destination, source, lambda)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteSumOperatorExpression(this);
    }
}

public class AverageOperatorExpressionSymbol : PredicateOperatorExpressionSymbol, ICollectionAggregationOperatorExpression
{
    public override OperatorType Operator { get; } = OperatorType.Average;

    public AverageOperatorExpressionSymbol(
        ExpressionSymbol destination,
        ExpressionSymbol source,
        LambdaExpressionSymbol lambda
    )
    : base(destination, source, lambda)
    {
    }

    public override Symbol Accept(AstRewriter rewriter)
    {
        return rewriter.RewriteAverageOperatorExpression(this);
    }
}
