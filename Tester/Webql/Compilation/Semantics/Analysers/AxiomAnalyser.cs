using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analyzers;

public class AxiomAnalyzer
{
    // axiom = [ lambda ]
    //
    // lambda = params , statement_block
    // statement_block = '{' [ statement, { statement } ] '}';
    //
    // params = '(' [ arg { ',' , arg  } ] ')' ;
    // arg = type identifier ;
    public AxiomSemantic AnalyzeAxiom(SemanticContextOld context, AxiomSymbol symbol)
    {
        return new AxiomSemantic();
    }
}
