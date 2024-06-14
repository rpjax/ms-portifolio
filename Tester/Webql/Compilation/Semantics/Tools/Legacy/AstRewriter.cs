using ModularSystem.Webql.Analysis.Extensions;
using ModularSystem.Webql.Analysis.Symbols;
using System.Diagnostics.CodeAnalysis;

namespace ModularSystem.Webql.Analysis.Components;

public class AstRewriter : AstTraverser
{
    private Dictionary<string, Symbol> RewrittenSymbols { get; } = new();

    public AstRewriter()
    {
     
    }

    public Symbol Execute(Symbol root)
    {
        TraverseTree(root);
        return RewriteSymbol(root);
    }

    protected void RewriteSymbol(Symbol symbol, Symbol updatedSymbol)
    {
        if(RewrittenSymbols.ContainsKey(symbol.Hash))
        {
            throw new InvalidOperationException("Symbol already rewritten.");
        }

        RewrittenSymbols[symbol.Hash] = updatedSymbol;
    }

    [return: NotNullIfNotNull("symbol")]
    private Symbol? RewriteSymbol(Symbol? symbol)
    {
        if(symbol is null)
        {
            return null;
        }
       
        if(RewrittenSymbols.TryGetValue(symbol.Hash, out var rewrittenSymbol))
        {
            return rewrittenSymbol;
        }

        return symbol.Accept(this);
    }

    [return: NotNullIfNotNull("symbol")]
    private T? RewriteSymbol<T>(T? symbol) where T : Symbol
    {
        if (symbol is null)
        {
            return null;
        }

        if (RewrittenSymbols.TryGetValue(symbol.Hash, out var rewrittenSymbol))
        {
            return rewrittenSymbol.As<T>();
        }

        return symbol.Accept(this).As<T>();
    }

    internal AxiomSymbol RewriteAxiom(AxiomSymbol symbol)
    {
        return new AxiomSymbol(
            lambda: RewriteSymbol(symbol.Lambda)
        );
    }

    internal StatementBlockSymbol RewriteStatementBlock(StatementBlockSymbol symbol)
    {
        var statements = symbol.Statements
            .Select(x => RewriteSymbol(x))
            .ToArray();

        return new StatementBlockSymbol(statements);
    }

    /*
     * statements.
     */

    internal DeclarationStatementSymbol RewriteDeclarationStatement(DeclarationStatementSymbol symbol)
    {
        return new DeclarationStatementSymbol(
            type: symbol.Type,
            identifier: symbol.Identifier,
            modifiers: symbol.Modifiers,
            value: RewriteSymbol(symbol.Value)
        );
    }

    /*
     * expressions.
     */

    internal LiteralExpressionSymbol RewriteLiteralExpression(LiteralExpressionSymbol symbol)
    {
        return symbol;
    }

    internal ReferenceExpressionSymbol RewriteReferenceExpression(ReferenceExpressionSymbol symbol)
    {
        return symbol;
    }

    internal LambdaExpressionSymbol RewriteLambdaExpression(LambdaExpressionSymbol symbol)
    {
        var parameters = symbol.Parameters
            .Select(x => RewriteSymbol(x))
            .ToArray();

        return new LambdaExpressionSymbol(
            parameters: parameters,
            body: RewriteSymbol(symbol.Body)
        );
    }

    internal AnonymousTypeExpressionSymbol RewriteAnonymousTypeExpression(AnonymousTypeExpressionSymbol symbol)
    {
        var bindings = symbol.Bindings
            .Select(x => RewriteSymbol(x))
            .ToArray();

        return new AnonymousTypeExpressionSymbol(
            bindings: bindings
        );
    }

    /*
     * other symbols.
     */

    internal TypeBindingSymbol RewriteTypeBinding(TypeBindingSymbol symbol)
    {
        return new TypeBindingSymbol(
            name: symbol.Name, 
            value: RewriteSymbol(symbol.Value)
        );
    }

    /*
     * concrete operator expressions.
     */

    internal OperatorExpressionSymbol RewriteAddOperatorExpression(AddOperatorExpressionSymbol symbol)
    {
        return new AddOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            left: RewriteSymbol(symbol.LeftOperand),
            right: RewriteSymbol(symbol.RightOperand)
        );
    }

    internal OperatorExpressionSymbol RewriteSubtractOperatorExpression(SubtractOperatorExpressionSymbol symbol)
    {
        return new SubtractOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            left: RewriteSymbol(symbol.LeftOperand),
            right: RewriteSymbol(symbol.RightOperand)
        );
    }

    internal OperatorExpressionSymbol RewriteDivideOperatorExpression(DivideOperatorExpressionSymbol symbol)
    {
        return new DivideOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            left: RewriteSymbol(symbol.LeftOperand),
            right: RewriteSymbol(symbol.RightOperand)
        );
    }

    internal OperatorExpressionSymbol RewriteMultiplyOperatorExpression(MultiplyOperatorExpressionSymbol symbol)
    {
        return new MultiplyOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            left: RewriteSymbol(symbol.LeftOperand),
            right: RewriteSymbol(symbol.RightOperand)
        );
    }

    internal OperatorExpressionSymbol RewriteModuloOperatorExpression(ModuloOperatorExpressionSymbol symbol)
    {
        return new ModuloOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            left: RewriteSymbol(symbol.LeftOperand),
            right: RewriteSymbol(symbol.RightOperand)
        );
    }

    internal OperatorExpressionSymbol RewriteEqualsOperatorExpression(EqualsOperatorExpressionSymbol symbol)
    {
        return new EqualsOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            left: RewriteSymbol(symbol.LeftOperand),
            right: RewriteSymbol(symbol.RightOperand)
        );
    }

    internal OperatorExpressionSymbol RewriteNotEqualsOperatorExpression(NotEqualsOperatorExpressionSymbol symbol)
    {
        return new NotEqualsOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            left: RewriteSymbol(symbol.LeftOperand),
            right: RewriteSymbol(symbol.RightOperand)
        );
    }

    internal OperatorExpressionSymbol RewriteLessOperatorExpression(LessOperatorExpressionSymbol symbol)
    {
        return new LessOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            left: RewriteSymbol(symbol.LeftOperand),
            right: RewriteSymbol(symbol.RightOperand)
        );
    }

    internal OperatorExpressionSymbol RewriteLessEqualsOperatorExpression(LessEqualsOperatorExpressionSymbol symbol)
    {
        return new LessEqualsOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            left: RewriteSymbol(symbol.LeftOperand),
            right: RewriteSymbol(symbol.RightOperand)
        );
    }

    internal OperatorExpressionSymbol RewriteGreaterOperatorExpression(GreaterOperatorExpressionSymbol symbol)
    {
        return new GreaterOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            left: RewriteSymbol(symbol.LeftOperand),
            right: RewriteSymbol(symbol.RightOperand)
        );
    }

    internal OperatorExpressionSymbol RewriteGreaterEqualsOperatorExpression(GreaterEqualsOperatorExpressionSymbol symbol)
    {
        return new GreaterEqualsOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            left: RewriteSymbol(symbol.LeftOperand),
            right: RewriteSymbol(symbol.RightOperand)
        );
    }

    internal OperatorExpressionSymbol RewriteLikeOperatorExpression(LikeOperatorExpressionSymbol symbol)
    {
        return new LikeOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            left: RewriteSymbol(symbol.LeftOperand),
            right: RewriteSymbol(symbol.RightOperand)
        );
    }

    internal OperatorExpressionSymbol RewriteRegexMatchOperatorExpression(RegexMatchOperatorExpressionSymbol symbol)
    {
        return new RegexMatchOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            left: RewriteSymbol(symbol.LeftOperand),
            right: RewriteSymbol(symbol.RightOperand)
        );
    }

    internal OperatorExpressionSymbol RewriteOrOperatorExpression(OrOperatorExpressionSymbol symbol)
    {
        var expressions = symbol.Expressions
            .Select(x => RewriteSymbol(x))
            .ToArray();

        return new OrOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            expressions: expressions
        );
    }

    internal OperatorExpressionSymbol RewriteAndOperatorExpression(AndOperatorExpressionSymbol symbol)
    {
        var expressions = symbol.Expressions
          .Select(x => RewriteSymbol(x))
          .ToArray();

        return new AndOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            expressions: expressions
        );
    }

    internal OperatorExpressionSymbol RewriteNotOperatorExpression(NotOperatorExpressionSymbol symbol)
    {
        return new NotOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            expression: RewriteSymbol(symbol.Operand)
        );
    }

    internal OperatorExpressionSymbol RewriteTypeOperatorExpression(TypeOperatorExpressionSymbol symbol)
    {
        return new TypeOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            typeExpression: RewriteSymbol(symbol.TypeExpression)
        );
    }

    internal OperatorExpressionSymbol RewriteMemberAccessOperatorExpression(MemberAccessOperatorExpressionSymbol symbol)
    {
        return new MemberAccessOperatorExpressionSymbol(
            memberName: symbol.MemberName,
            operand: RewriteSymbol(symbol.Operand)
        );
    }

    internal OperatorExpressionSymbol RewriteFilterOperatorExpression(FilterOperatorExpressionSymbol symbol)
    {
        return new FilterOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            source: RewriteSymbol(symbol.Source),
            lambda: RewriteSymbol(symbol.Lambda)
        );
    }

    internal OperatorExpressionSymbol RewriteSelectOperatorExpression(SelectOperatorExpressionSymbol symbol)
    {
        return new SelectOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            source: RewriteSymbol(symbol.Source),
            lambda: RewriteSymbol(symbol.Lambda)
        );
    }

    internal OperatorExpressionSymbol RewriteSelectManyOperatorExpression(SelectManyOperatorExpressionSymbol symbol)
    {
        return new SelectManyOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            source: RewriteSymbol(symbol.Source),
            lambda: RewriteSymbol(symbol.Lambda)
        );
    }

    internal OperatorExpressionSymbol RewriteLimitOperatorExpression(LimitOperatorExpressionSymbol symbol)
    {
        return new LimitOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            source: RewriteSymbol(symbol.Source),
            value: RewriteSymbol(symbol.Value)
        );
    }

    internal OperatorExpressionSymbol RewriteSkipOperatorExpression(SkipOperatorExpressionSymbol symbol)
    {
        return new SkipOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            source: RewriteSymbol(symbol.Source),
            value: RewriteSymbol(symbol.Value)
        );
    }

    internal OperatorExpressionSymbol RewriteCountOperatorExpression(CountOperatorExpressionSymbol symbol)
    {
        return new CountOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            source: RewriteSymbol(symbol.Source)
        );
    }

    internal OperatorExpressionSymbol RewriteIndexOperatorExpression(IndexOperatorExpressionSymbol symbol)
    {
        return new IndexOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            source: RewriteSymbol(symbol.Source),
            index: RewriteSymbol(symbol.Index)
        );
    }

    internal OperatorExpressionSymbol RewriteAnyOperatorExpression(AnyOperatorExpressionSymbol symbol)
    {
        return new AnyOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            source: RewriteSymbol(symbol.Source),
            lambda: RewriteSymbol(symbol.Lambda)
        );
    }

    internal OperatorExpressionSymbol RewriteAllOperatorExpression(AllOperatorExpressionSymbol symbol)
    {
        return new AllOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            source: RewriteSymbol(symbol.Source),
            lambda: RewriteSymbol(symbol.Lambda)
        );
    }

    internal OperatorExpressionSymbol RewriteMinOperatorExpression(MinOperatorExpressionSymbol symbol)
    {
        return new MinOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            source: RewriteSymbol(symbol.Source),
            lambda: RewriteSymbol(symbol.Lambda)
        );
    }

    internal OperatorExpressionSymbol RewriteMaxOperatorExpression(MaxOperatorExpressionSymbol symbol)
    {
        return new MaxOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            source: RewriteSymbol(symbol.Source),
            lambda: RewriteSymbol(symbol.Lambda)
        );
    }

    internal OperatorExpressionSymbol RewriteSumOperatorExpression(SumOperatorExpressionSymbol symbol)
    {
        return new SumOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            source: RewriteSymbol(symbol.Source),
            lambda: RewriteSymbol(symbol.Lambda)
        );
    }

    // ... continue for the rest of the operator expressions

    internal OperatorExpressionSymbol RewriteAverageOperatorExpression(AverageOperatorExpressionSymbol symbol)
    {
        return new AverageOperatorExpressionSymbol(
            destination: RewriteSymbol(symbol.Destination),
            source: RewriteSymbol(symbol.Source),
            lambda: RewriteSymbol(symbol.Lambda)
        );
    }

}
