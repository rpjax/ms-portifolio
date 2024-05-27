namespace ModularSystem.Core.TextAnalysis.Parsing.Components;

public interface ICstReducer
{
    CstNode Reduce(CstNode[] children);
}
