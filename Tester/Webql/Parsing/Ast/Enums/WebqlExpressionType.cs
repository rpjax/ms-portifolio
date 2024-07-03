namespace Webql.Parsing.Ast;

public enum WebqlExpressionType
{
    Literal,
    TemporaryDeclaration,
    Reference,
    MemberAccess,
    Block,
    Operation
}
