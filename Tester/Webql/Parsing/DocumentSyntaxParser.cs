using ModularSystem.Core.TextAnalysis.Gdef;
using ModularSystem.Core.TextAnalysis.Parsing;
using ModularSystem.Core.TextAnalysis.Parsing.Components;
using ModularSystem.Core.TextAnalysis.Parsing.Tools;

namespace Webql.DocumentSyntax.Parsing;

public static class DocumentSyntaxParser
{
    const string RawGrammarText = @"
/*
	Document Syntax for WebQL Version 3.0.0, created by Rodrigo Jacques.
*/

start
	: document 
	;

document
	: expression
	;

expression 
	: lteral_expression
	| reference_expression
	| scope_access_expression
	| block_expression
	| operation_expression
	;

lteral_expression
	: bool
	| null
	| $int
	| $float
	| $hex
	| $string
	;

bool
	: 'true'
	| 'false'
	;

null
	: 'null'
	;

reference_expression
	: $id
	| '$' $string
	;

scope_access_expression
	: reference_expression ':' expression
	;

block_expression
	: '{' [ expression ] { ',' expression } '}' 
	;

operation_expression
	: operator ':' expression
	;

operator 
	: arithmetic_operator
	| relational_operator
	| string_relational_operator
	| logical_operator
	| collection_manipulation_operator
	| collection_aggregation_operator
	;

arithmetic_operator
	: '$add'
	| '$subtract'
	| '$multiply'
	| '$divide'
	;

relational_operator
	: '$equals'
	| '$notEquals'
	| '$greater'
	| '$less'
	| '$greaterEquals'
	| '$lessEquals'
	;

string_relational_operator
	: '$like'
	;

logical_operator
	: '$and'
	| '$or'
	| '$not'
	;

collection_manipulation_operator
	: '$filter'
	| '$select'
	| '$group'
	| '$order'
	| '$limit'
	| '$skip'
	;

collection_aggregation_operator
	: '$count'
	| '$any'
	| '$all'
	| '$sum'
	| '$average'
	| '$min'
	| '$max'
	;
";

    private static string[] ReduceWhitelist { get; } = new string[]
    {
		// main constructs
		"document",
        //"expression",
        "operator",

		// expressions
        "lteral_expression",
        "reference_expression",
        "scope_access_expression",
        "block_expression",
        "operation_expression",
    };

    private static LR1Parser? ParserInstance { get; set; }

    public static void Init()
    {
        if (ParserInstance is not null)
        {
            return;
        }

        GetParser();
    }

    public static LR1Parser GetParser()
    {
        if (ParserInstance is null)
        {
            ParserInstance = new LR1Parser(
                grammar: GdefParser.ParseGrammar(RawGrammarText)
            );
        }

        return ParserInstance;
    }

    public static CstNode Parse(string text)
    {
        var cst = GetParser().Parse(text);
        var reducer = new CstReducer(cst, ReduceWhitelist);

        return reducer.Execute();
    }

}

