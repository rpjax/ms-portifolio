namespace ModularSystem.TextAnalysis.Parsing.Components;

public interface ICstReducer
{
    CstNode Reduce(CstNode[] children);
}
