using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Semantics.Analysers;

public class AxiomSemanticsAnalyser
{
    // axiom = [ lambda ]
    //
    // lambda = params , statement_block
    // statement_block = '{' [ statement, { statement } ] '}';
    //
    // params = '(' [ arg { ',' , arg  } ] ')' ;
    // arg = type identifier ;
    public AxiomSemantic AnalyseAxiom(SemanticContext context, AxiomSymbol symbol)
    {
        return new AxiomSemantic();
    }
}
